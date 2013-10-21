using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using CLAP.Interception;

#if !FW2
using System.Linq;
#endif

namespace CLAP
{
    internal class ParserRunner
    {
        #region Fields

        // The possible prefixes of a parameter
        //
        internal readonly static string[] ArgumentPrefixes = new[] { "/", "-" };
        private readonly static string s_fileInputSuffix = "@";

        private readonly ParserRegistration m_registration;
        private readonly HelpGeneratorBase m_helpGenerator;

        #endregion Fields

        #region Properties

        internal Type Type { get; private set; }

        internal ParserRegistration Register
        {
            get { return m_registration; }
        }

        #endregion Properties

        #region Constructors

        internal ParserRunner(Type type, ParserRegistration parserRegistration, HelpGeneratorBase helpGenerator)
        {
            Debug.Assert(type != null);

            Type = type;

            m_registration = parserRegistration;

            Validate(type, m_registration);

            m_helpGenerator = helpGenerator;
        }

        #endregion Constructors

        #region Public Methods

        public int Run(string[] args, object obj)
        {
            return TryRunInternal(args, obj);
        }

        #endregion Public Methods

        #region Private Methods

        private int TryRunInternal(string[] args, object obj)
        {
            try
            {
                return RunInternal(args, obj);
            }
            catch (Exception ex)
            {
                var rethrow = HandleError(ex, obj);

                if (rethrow)
                {
                    throw;
                }

                return MultiParser.ErrorCode;
            }
        }

        private int RunInternal(string[] args, object obj)
        {
            //
            // *** empty args are handled by the multi-parser
            //
            //

            Debug.Assert(args.Length > 0 && args.All(a => !string.IsNullOrEmpty(a)));

            // the first arg should be the verb, unless there is no verb and a default is used
            //
            var firstArg = args[0];

            if (HandleHelp(firstArg, obj))
            {
                return MultiParser.ErrorCode;
            }

            var verb = firstArg;

            // a flag, in case there is no verb
            //
            var noVerb = false;

            // find the method by the given verb
            //
            var typeVerbs = GetVerbs()
                .ToDictionary(v => v, v=>GetParameters(v.MethodInfo).ToList());

            // arguments
            var notVerbs = args
                .Where(a => a.StartsWith(ArgumentPrefixes))
                .ToList();
            
            var globals = GetDefinedGlobals()
                .Where(g => notVerbs.Any(a => a.Substring(1).StartsWith(g.Name.ToLowerInvariant())))
                .Select(g => g)
                .ToList();

            var notVerbsNotGlobals = notVerbs
                .Where(a => globals.All(g => !a.Substring(1).StartsWith(g.Name.ToLowerInvariant())))
                .Select(v => v)
                .ToList();

            // find the method by name, parameter count and parameter names
            var methods = (
                from v in typeVerbs
                where v.Key.Names.Contains(verb.ToLowerInvariant())
                where v.Value.Count == notVerbs.Count
                select v
                ).ToList();

            Method method = SelectMethod(methods, notVerbs);

            // if arguments do not match parameter names, exclude globals
            methods = (
                from v in typeVerbs
                where v.Key.Names.Contains(verb.ToLowerInvariant())
                where v.Value.Count == notVerbsNotGlobals.Count
                select v
                ).ToList();
            
            if (method == null)
            {
                method = SelectMethod(methods, notVerbsNotGlobals);
            }

            // if arguments do not match parameters names, exclude optional parameters
            if (method == null)
            {
                const bool avoidOptionalParameters = false;
                method = SelectMethod(methods, notVerbsNotGlobals, avoidOptionalParameters);
            }

            // if arguments do not match parameter names, use only argument count (without globals)
            if (method == null)
            {
                method = methods.FirstOrDefault().Key;
            }

            // if nothing matches...
            if (method == null)
            {
                method = typeVerbs.FirstOrDefault(v => v.Key.Names.Contains(verb.ToLowerInvariant())).Key;
            }

            // if no method is found - a default must exist
            if (method == null)
            {
                // if there is a verb input but no method was found
                // AND
                // the first arg is not an input argument (doesn't start with "-" etc)
                //
                if (verb != null && !verb.StartsWith(ArgumentPrefixes))
                {
                    throw new VerbNotFoundException(verb);
                }

                method = typeVerbs.FirstOrDefault(v => v.Key.IsDefault).Key;

                // no default - error
                //
                if (method == null)
                {
                    throw new MissingDefaultVerbException();
                }

                noVerb = true;
            }

            // if there is a verb - skip the first arg
            //
            var inputArgs = MapArguments(noVerb ? args : args.Skip(1));

            HandleGlobals(inputArgs, obj);

            // a list of the available parameters
            //
            var paremetersList = GetParameters(method.MethodInfo);

            // a list of values, used when invoking the method
            //
            var parameterValues = ValuesFactory.CreateParameterValues(method, obj, inputArgs, paremetersList);

            ValidateVerbInput(method, parameterValues.Select(kvp => kvp.Value).ToList());

            // if some args weren't handled
            //
            if (inputArgs.Any())
            {
                throw new UnhandledParametersException(inputArgs);
            }

            return Execute(obj, method, parameterValues);
        }

        private static Method SelectMethod(List<KeyValuePair<Method, List<Parameter>>> methods, List<string> args, bool allowOptionalParameters = true)
        {
            Method method = null;

            foreach (var m in methods)
            {
                bool methodFound = false;
                var parameters = m.Value;

                if (!allowOptionalParameters)
                {
                    parameters = m.Value
                        .Where(p => p.Default != null)
                        .Select(p => p)
                        .ToList();
                }

                foreach (var p in parameters)
                {
                    bool parameterMatches = false;

                    foreach (var name in p.Names)
                    {
                        if (args.Any(a => a.Substring(1).Equals(name)))
                        {
                            parameterMatches = true;
                            break;
                        }
                    }
                    if (!parameterMatches)
                    {
                        break;
                    }
                    methodFound = true;
                }

                if (methodFound)
                {
                    method = m.Key;
                    break;
                }
            }

            return method;
        }

        private int Execute(
            object target,
            Method method,
            ParameterAndValue[] parameters)
        {
            // pre-interception
            //
            var preVerbExecutionContext = PreInterception(target, method, parameters);

            Exception verbException = null;

            try
            {
                // actual verb execution
                //
                if (!preVerbExecutionContext.Cancel)
                {
                    // invoke the method with the list of parameters
                    //
                    MethodInvoker.Invoke(method.MethodInfo, target, parameters.Select(p => p.Value).ToArray());
                }
            }
            catch (TargetInvocationException tex)
            {
                verbException = tex.InnerException;
            }
            catch (Exception ex)
            {
                verbException = ex;
            }
            finally
            {
                try
                {
                    PostInterception(target, method, parameters, preVerbExecutionContext, verbException);
                }
                finally
                {
                    if (verbException != null)
                    {
                        var rethrow = HandleError(verbException, target);

                        if (rethrow)
                        {
                            throw verbException;
                        }
                    }
                }
            }

            return verbException == null ? MultiParser.SuccessCode : MultiParser.ErrorCode;
        }

        private void PostInterception(
            object target,
            Method method,
            ParameterAndValue[] parameters,
            PreVerbExecutionContext preVerbExecutionContext,
            Exception verbException)
        {
            var postVerbExecutionContext = new PostVerbExecutionContext(
                method,
                target,
                parameters,
                preVerbExecutionContext.Cancel,
                verbException,
                preVerbExecutionContext.UserContext);

            // registered interceptors get top priority
            //
            if (m_registration.RegisteredPostVerbInterceptor != null)
            {
                m_registration.RegisteredPostVerbInterceptor(postVerbExecutionContext);
            }
            else
            {
                var postInterceptionMethods = Type.GetMethodsWith<PostVerbExecutionAttribute>();

                // try a defined interceptor type
                //
                if (postInterceptionMethods.Any())
                {
                    Debug.Assert(postInterceptionMethods.Count() == 1);

                    var postInterceptionMethod = postInterceptionMethods.First();

                    MethodInvoker.Invoke(postInterceptionMethod, target, new[] { postVerbExecutionContext });
                }
                else
                {
                    // try a defined interceptor type
                    //
                    if (Type.HasAttribute<VerbInterception>())
                    {
                        var interception = Type.GetAttribute<VerbInterception>();

                        var interceptor = (IPostVerbInterceptor)Activator.CreateInstance(interception.InterceptorType);

                        interceptor.AfterVerbExecution(postVerbExecutionContext);
                    }
                }
            }
        }

        private PreVerbExecutionContext PreInterception(
            object target,
            Method method,
            ParameterAndValue[] parameters)
        {
            var preVerbExecutionContext = new PreVerbExecutionContext(method, target, parameters);

            // registered interceptors get top priority
            //
            if (m_registration.RegisteredPreVerbInterceptor != null)
            {
                m_registration.RegisteredPreVerbInterceptor(preVerbExecutionContext);
            }
            else
            {
                // try a defined verb interceptor
                //
                var preInterceptionMethods = Type.GetMethodsWith<PreVerbExecutionAttribute>();

                if (preInterceptionMethods.Any())
                {
                    Debug.Assert(preInterceptionMethods.Count() == 1);

                    var preInterceptionMethod = preInterceptionMethods.First();

                    MethodInvoker.Invoke(preInterceptionMethod, target, new[] { preVerbExecutionContext });
                }
                else
                {
                    // try a defined interceptor type
                    //
                    if (Type.HasAttribute<VerbInterception>())
                    {
                        var interception = Type.GetAttribute<VerbInterception>();

                        var interceptor = (IPreVerbInterceptor)Activator.CreateInstance(interception.InterceptorType);

                        interceptor.BeforeVerbExecution(preVerbExecutionContext);
                    }
                }
            }

            return preVerbExecutionContext;
        }

        private static void ValidateVerbInput(Method method, List<object> parameterValues)
        {
            var methodParameters = method.MethodInfo.GetParameters();

            // validate all parameters
            //
            var validators = method.MethodInfo.GetInterfaceAttributes<ICollectionValidation>().Select(a => a.GetValidator());

            if (validators.Any())
            {
                var parametersAndValues = new List<ValueInfo>();

                methodParameters.Each((p, i) =>
                {
                    parametersAndValues.Add(new ValueInfo(p.Name, p.ParameterType, parameterValues[i]));
                });

                // all validators must pass
                //
                foreach (var validator in validators)
                {
                    validator.Validate(parametersAndValues.ToArray());
                }
            }
        }

        internal static void Validate(Type type, ParserRegistration registration)
        {
            // no more than one default verb
            //
            var verbMethods = type.GetMethodsWith<VerbAttribute>().
                Select(m => new Method(m));

            var defaultVerbs = verbMethods.Where(m => m.IsDefault);

            if (defaultVerbs.Count() > 1)
            {
                throw new MoreThanOneDefaultVerbException(defaultVerbs.Select(m => m.MethodInfo.Name));
            }

            // no more than one error handler
            //
            ValidateDefinedErrorHandlers(type);

            // validate empty handlers (and empty help handlers)
            //
            ValidateDefinedEmptyHandlers(type);

            // validate pre/post interceptors
            //
            ValidateDefinedPreInterceptors(type);
            ValidateDefinedPostInterceptors(type);

            // parameters can't have both Default and DefaultProvider
            //
            ValidateParameterDefaults(verbMethods);

            // [Separator] can be applied only to array parameters
            //
            ValidateSeparators(verbMethods, registration);

            // no duplicate globals
            //
            ValidateDuplicateGlobals(type, registration);
        }

        private static void ValidateDuplicateGlobals(Type type, ParserRegistration registration)
        {
            var definedGlobals = type.
                GetMethodsWith<GlobalAttribute>().
                SelectMany(m =>
                {
                    var att = m.GetAttribute<GlobalAttribute>();
                    var name = att.Name ?? m.Name;

                    return att.Aliases.CommaSplit().Union(new[] { name }).Select(s => s.ToLowerInvariant());
                }).
                ToList();

            var globals = registration.RegisteredGlobalHandlers.Keys.Select(k => k.ToLowerInvariant()).ToList();

            globals.AddRange(definedGlobals);

            var counts = globals.Distinct().ToDictionary(g => g, g => globals.Count(name => name == g));

            var duplicate = counts.Where(c => c.Value > 1);

            if (duplicate.Any())
            {
                throw new DuplicateGlobalHandlerException(duplicate.First().Key);
            }
        }

        private static void ValidateParameterDefaults(IEnumerable<Method> verbs)
        {
            var parameters = verbs.SelectMany(v => v.MethodInfo.GetParameters()).ToList();

            // find one with both a Default and a DefaultProvider
            //
            var hasBothDefaults = parameters.Where(p =>
                p.HasAttribute<DefaultValueAttribute>() &&
                p.HasAttribute<DefaultProviderAttribute>());

            if (hasBothDefaults.Any())
            {
                throw new AmbiguousParameterDefaultException(hasBothDefaults.First());
            }


            // make sure all default providers are DefaultProvider
            //
            var hasInvalidDefaultProvider = parameters.Where(p =>
                p.HasAttribute<DefaultProviderAttribute>() &&
                !typeof(DefaultProvider).IsAssignableFrom(p.GetAttribute<DefaultProviderAttribute>().DefaultProviderType));

            if (hasInvalidDefaultProvider.Any())
            {
                throw new InvalidParameterDefaultProviderException(hasInvalidDefaultProvider.First());
            }
        }

        private static void ValidateSeparators(IEnumerable<Method> verbs, ParserRegistration registration)
        {
            // check non-arrays
            //
            var parameters = verbs.SelectMany(v => v.MethodInfo.GetParameters()).ToList();

            var nonArrayWithSeparator = parameters.Where(p => !p.ParameterType.IsArray && p.HasAttribute<SeparatorAttribute>());

            if (nonArrayWithSeparator.Any())
            {
                throw new NonArrayParameterWithSeparatorException(nonArrayWithSeparator.First());
            }

            // check invalid separators
            //
            var separators = parameters.
                Where(p => p.HasAttribute<SeparatorAttribute>()).
                Select(p => Pair.Create(p, p.GetAttribute<SeparatorAttribute>().Separator));

            var invalidSeparator = separators.
                FirstOrDefault(pair => string.IsNullOrEmpty(pair.Second) || pair.Second.Contains(" "));

            if (invalidSeparator != null)
            {
                throw new InvalidSeparatorException(invalidSeparator.First);
            }


            var invalidRegisteredHandlers = registration.RegisteredGlobalHandlers.
                FirstOrDefault(a =>
                    a.Value.Type.IsArray &&
                    (string.IsNullOrEmpty(a.Value.Separator) || a.Value.Separator.Contains(" ")));
        }

        private static void ValidateDefinedEmptyHandlers(Type type)
        {
            var definedEmptyHandlers = type.GetMethodsWith<EmptyAttribute>();
            var definedEmptyHandlersCount = definedEmptyHandlers.Count();

            if (definedEmptyHandlersCount > 0)
            {
                // empty handler without parameters
                //
                var definedEmptyHandler = definedEmptyHandlers.First();

                var definedEmptyHandlerParameters = definedEmptyHandler.GetParameters();

                if (definedEmptyHandlerParameters.Any())
                {
                    if (definedEmptyHandler.HasAttribute<HelpAttribute>())
                    {
                        // if empty help handler is not Action<string> - throw
                        //
                        if (definedEmptyHandlerParameters.Length > 1 ||
                            definedEmptyHandlerParameters.First().ParameterType != typeof(string))
                        {
                            throw new InvalidHelpHandlerException(definedEmptyHandler);
                        }
                    }
                    else
                    {
                        throw new ArgumentMismatchException(
                            "Method '{0}' is marked as [Empty] so it should not have any parameters".FormatWith(definedEmptyHandler));
                    }
                }

                if (definedEmptyHandlersCount > 1)
                {
                    // no more than one empty handler
                    //
                    throw new MoreThanOneEmptyHandlerException();
                }
            }
        }

        private Action<string> GetRegisteredHelpHandler(string input)
        {
            Debug.Assert(!string.IsNullOrEmpty(input));

            Action<string> action = null;

            if (m_registration.RegisteredHelpHandlers.TryGetValue(input.ToLowerInvariant(), out action))
            {
                return action;
            }

            return null;
        }

        /// <summary>
        /// Creates a map of the input arguments and their string values
        /// </summary>
        private static Dictionary<string, string> MapArguments(IEnumerable<string> args)
        {
            var map = new Dictionary<string, string>();

            foreach (var arg in args)
            {
                // all arguments must start with a valid prefix
                //
                if (!arg.StartsWith(ArgumentPrefixes))
                {
                    throw new MissingArgumentPrefixException(arg, string.Join(",", ArgumentPrefixes));
                }

                var prefix = arg.Substring(1);

                var parts = prefix.Split(new[] { '=', ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
                var name = parts[0].ToLowerInvariant();

                string valueString = null;

                // a switch (a boolean parameter) doesn't need to have a separator,
                // in that case, a null string value is mapped
                //
                if (parts.Length > 1)
                {
                    valueString = parts[1];

                    // if it has a file input suffix - remove it
                    //
                    if (name.EndsWith(s_fileInputSuffix))
                    {
                        name = name.Substring(0, name.Length - 1);

                        // the value is replaced with the content of the input file
                        //
                        valueString = FileSystemHelper.ReadAllText(valueString);
                    }
                }

                map.Add(name, valueString);
            }

            return map;
        }

        /// <summary>
        /// Create a list of parameters for the given method
        /// </summary>
        internal static IEnumerable<Parameter> GetParameters(MethodInfo method)
        {
            var parameters = method.GetParameters().Select(p => new Parameter(p)).ToList();

            // detect duplicates
            //
            var allNames = parameters.SelectMany(p => p.Names);
            var duplicates = allNames.Where(name =>
                allNames.Count(n => n.Equals(name, StringComparison.InvariantCultureIgnoreCase)) > 1);

            if (duplicates.Any())
            {
                throw new InvalidOperationException(
                    "Duplicate parameter names found in {0}: {1}".FormatWith(
                        method.Name,
                        duplicates.Distinct().StringJoin(", ")));
            }

            // try to find short aliases
            //
            var paramsToFirstCharDict = parameters.ToDictionary(p => p, p => p.ParameterInfo.Name.ToLowerInvariant()[0].ToString());

            // if there are no duplicate first-chars
            //
            if (paramsToFirstCharDict.Values.Distinct().Count() == parameters.Count())
            {
                foreach (var p in parameters)
                {
                    var c = paramsToFirstCharDict[p];

                    if (!p.Names.Contains(c))
                    {
                        p.Names.Add(c);
                    }
                }
            }


            return parameters;
        }

        /// <summary>
        /// Create a list of methods (verbs) for the given type
        /// </summary>
        internal IEnumerable<Method> GetVerbs()
        {
            var verbMethods = Type.
                GetMethodsWith<VerbAttribute>().
                Select(m => new Method(m)).
                ToList();

            if (verbMethods.Select(v => v.MethodInfo.Name[0]).Distinct().Count() == verbMethods.Count())
            {
                foreach (var v in verbMethods)
                {
                    var c = v.MethodInfo.Name[0].ToString().ToLowerInvariant();

                    if (!v.Names.Contains(c))
                    {
                        v.Names.Add(c);
                    }
                }
            }

            var defaultVerbs = verbMethods.Where(m => m.IsDefault);

            Debug.Assert(defaultVerbs.Count() <= 1);

            return verbMethods;
        }

        /// <summary>
        /// Handles any global parameter that has any input
        /// </summary>
        private void HandleGlobals(Dictionary<string, string> args, object obj)
        {
            HandleRegisteredGlobals(args);
            HandleDefinedGlobals(args, obj);
        }

        /// <summary>
        /// Handles any registered global parameter that has any input
        /// </summary>
        private void HandleRegisteredGlobals(Dictionary<string, string> args)
        {
            var handled = new List<string>();

            foreach (var kvp in args)
            {
                var key = m_registration.RegisteredGlobalHandlers.Keys.FirstOrDefault(
                    k => k.CommaSplit().Contains(kvp.Key));

                if (key != null)
                {
                    var handler = m_registration.RegisteredGlobalHandlers[key];

                    handler.Handler(kvp.Value);

                    handled.Add(kvp.Key);
                }
            }

            // remove them so later we'll see which ones were not handled
            //
            foreach (var h in handled)
            {
                args.Remove(h);
            }
        }

        internal IEnumerable<MethodInfo> GetDefinedGlobals()
        {
            var globals = Type.GetMethodsWith<GlobalAttribute>();

            return globals;
        }

        internal static IEnumerable<MethodInfo> GetDefinedErrorHandlers(Type type)
        {
            var errorHandlers = type.GetMethodsWith<ErrorAttribute>();

            return errorHandlers;
        }

        /// <summary>
        /// Handles any defined global parameter that has any input
        /// </summary>
        private void HandleDefinedGlobals(Dictionary<string, string> args, object obj)
        {
            var globals = GetDefinedGlobals();

            foreach (var method in globals)
            {
                var att = method.GetAttribute<GlobalAttribute>();

                var name = att.Name ?? method.Name;

                var allNames = new[] { name }.Union(att.Aliases.CommaSplit());

                var key = args.Keys.FirstOrDefault(
                    k => allNames.Any(
                        n => n.Equals(k, StringComparison.InvariantCultureIgnoreCase)));

                if (key != null)
                {
                    var parameters = method.GetParameters();

                    if (parameters.Length == 0)
                    {
                        MethodInvoker.Invoke(method, obj, null);
                    }
                    else if (parameters.Length == 1)
                    {
                        string stringValue;

                        if (args.TryGetValue(key, out stringValue))
                        {
                            var p = parameters.First();

                            var value = ValuesFactory.GetValueForParameter(p, p.ParameterType, key, stringValue);

                            // validation
                            //
                            if (value != null && p.HasAttribute<ValidationAttribute>())
                            {
                                var parameterValidators = p.GetAttributes<ValidationAttribute>().Select(a => a.GetValidator());

                                // all validators must pass
                                //
                                foreach (var validator in parameterValidators)
                                {
                                    validator.Validate(new ValueInfo(p.Name, p.ParameterType, value));
                                }
                            }

                            var validators = method.GetInterfaceAttributes<ICollectionValidation>().
                                Select(a => a.GetValidator());

                            foreach (var validator in validators)
                            {
                                validator.Validate(new[]
                                {
                                    new ValueInfo(p.Name, p.ParameterType, value),
                                });
                            }

                            MethodInvoker.Invoke(method, obj, new[] { value });
                        }
                    }
                    else
                    {
                        throw new NotSupportedException(
                            "Method {0} has more than one parameter and cannot be handled as a global handler".FormatWith(method.Name));
                    }

                    // remove it so later we'll see which ones were not handled
                    //
                    args.Remove(key);
                }
            }
        }

        private bool HandleHelp(string firstArg, object target)
        {
            var arg = firstArg;

            if (ArgumentPrefixes.Contains(firstArg[0].ToString()))
            {
                arg = firstArg.Substring(1);
            }

            Action<string> helpHandler = GetRegisteredHelpHandler(arg);

            if (helpHandler != null)
            {
                helpHandler(m_helpGenerator.GetHelp(this));

                return true;
            }

            var definedHelpMethods = Type.GetMethodsWith<HelpAttribute>();

            foreach (var method in definedHelpMethods)
            {
                var att = method.GetAttribute<HelpAttribute>();

                var name = att.Name ?? method.Name;

                if (name.Equals(arg, StringComparison.InvariantCultureIgnoreCase) ||
                    att.Aliases.CommaSplit().Any(s => s.Equals(arg, StringComparison.InvariantCultureIgnoreCase)))
                {
                    try
                    {
                        VerifyMethodAndTarget(method, target);

                        var obj = method.IsStatic ? null : target;

                        MethodInvoker.Invoke(method, obj, new[] { m_helpGenerator.GetHelp(this) });

                        return true;
                    }
                    catch (TargetParameterCountException ex)
                    {
                        throw new InvalidHelpHandlerException(method, ex);
                    }
                    catch (ArgumentException ex)
                    {
                        throw new InvalidHelpHandlerException(method, ex);
                    }
                }
            }

            return false;
        }

        internal void HandleEmptyArguments(object target)
        {
            //
            // *** registered empty handlers are called by the multi-parser
            //

            var definedEmptyHandlers = Type.GetMethodsWith<EmptyAttribute>();
            var definedEmptyHandlersCount = definedEmptyHandlers.Count();

            Debug.Assert(definedEmptyHandlersCount <= 1);

            if (definedEmptyHandlersCount == 1)
            {
                var method = definedEmptyHandlers.First();

                VerifyMethodAndTarget(method, target);

                var obj = method.IsStatic ? null : target;

                // if it is a [Help] handler
                //
                if (method.HasAttribute<HelpAttribute>())
                {
                    var help = m_helpGenerator.GetHelp(this);

                    // method should execute because it was already passed validation
                    //
                    MethodInvoker.Invoke(method, obj, new[] { help });
                }
                else
                {
                    MethodInvoker.Invoke(method, obj, null);
                }

                // don't handle the default verb
                //
                return;
            }

            var defaultVerb = GetDefaultVerb();

            // if there is a default verb - execute it
            //
            if (defaultVerb != null)
            {
                // create an array of arguments that matches the method
                //
                var parameters = ValuesFactory.CreateParameterValues(
                    defaultVerb,
                    target,
                    new Dictionary<string, string>(),
                    GetParameters(defaultVerb.MethodInfo));

                Execute(target, defaultVerb, parameters);
            }
        }

        private static void VerifyMethodAndTarget(MethodInfo method, object target)
        {
            if (!method.IsStatic && target == null)
            {
                throw new ParserExecutionTargetException(
                    "Method '{0}' is not static but the target object is null".FormatWith(method));
            }

            if (method.IsStatic && target != null)
            {
                throw new ParserExecutionTargetException(
                    "Method '{0}' is static but the target object is not null".FormatWith(method));
            }
        }

        private bool HandleError(Exception ex, object target)
        {
            var context = new ExceptionContext(ex);

            if (m_registration.RegisteredErrorHandler != null)
            {
                m_registration.RegisteredErrorHandler(context);

                return context.ReThrow;
            }
            else
            {
                var definedErrorHandlers = GetDefinedErrorHandlers(Type);

                Debug.Assert(definedErrorHandlers.Count() <= 1);

                var handler = definedErrorHandlers.FirstOrDefault();

                if (handler != null)
                {
                    MethodInvoker.Invoke(handler, target, new[] { context });

                    return context.ReThrow;
                }

                // no handler - rethrow
                //
                return true;
            }
        }

        private static void ValidateDefinedErrorHandlers(Type type)
        {
            // check for too many defined global error handlers
            //
            var definedErrorHandlers = GetDefinedErrorHandlers(type);

            var count = definedErrorHandlers.Count();

            // good
            //
            if (count == 0)
            {
                return;
            }

            if (count > 1)
            {
                throw new MoreThanOneErrorHandlerException();
            }

            // there is only one defined handler
            //
            var method = definedErrorHandlers.First();

            var parameters = method.GetParameters();

            if (parameters.Length > 1)
            {
                throw new ArgumentMismatchException(
                    "Method '{0}' is marked as [Error] so it should have a single parameter of type CLAP.ExceptionContext".FormatWith(method));
            }
            else
            {
                var parameter = parameters.First();

                if (parameter.ParameterType != typeof(ExceptionContext))
                {
                    throw new ArgumentMismatchException(
                        "Method '{0}' is marked as [Error] so it should have a single parameter of type CLAP.ExceptionContext".FormatWith(method));
                }
            }
        }

        private static void ValidateDefinedPreInterceptors(Type type)
        {
            // no more that one pre/post interception methods
            //
            var preInterceptionMethods = type.GetMethodsWith<PreVerbExecutionAttribute>();

            var preInterceptionMethodsCount = preInterceptionMethods.Count();

            if (preInterceptionMethodsCount == 0)
            {
                return;
            }
            else if (preInterceptionMethodsCount > 1)
            {
                throw new MoreThanOnePreVerbInterceptorException();
            }

            // there is only one defined interceptor
            //
            var method = preInterceptionMethods.First();

            var parameters = method.GetParameters();

            if (parameters.Length > 1)
            {
                throw new ArgumentMismatchException(
                    "Method '{0}' is marked as [PreVerbExecution] so it should have a single parameter of type CLAP.PreVerbExecutionContext".FormatWith(method));
            }
            else
            {
                var parameter = parameters.First();

                if (parameter.ParameterType != typeof(PreVerbExecutionContext))
                {
                    throw new ArgumentMismatchException(
                        "Method '{0}' is marked as [PreVerbExecution] so it should have a single parameter of type CLAP.PreVerbExecutionContext".FormatWith(method));
                }
            }
        }

        private static void ValidateDefinedPostInterceptors(Type type)
        {
            // no more that one pre/post interception methods
            //
            var postInterceptionMethods = type.GetMethodsWith<PostVerbExecutionAttribute>();

            var postInterceptionMethodsCount = postInterceptionMethods.Count();

            if (postInterceptionMethodsCount == 0)
            {
                return;
            }
            else if (postInterceptionMethodsCount > 1)
            {
                throw new MoreThanOnePostVerbInterceptorException();
            }

            // there is only one defined interceptor
            //
            var method = postInterceptionMethods.First();

            var parameters = method.GetParameters();

            if (parameters.Length > 1)
            {
                throw new ArgumentMismatchException(
                    "Method '{0}' is marked as [PostVerbExecution] so it should have a single parameter of type CLAP.PostVerbExecutionContext".FormatWith(method));
            }
            else
            {
                var parameter = parameters.First();

                if (parameter.ParameterType != typeof(PostVerbExecutionContext))
                {
                    throw new ArgumentMismatchException(
                        "Method '{0}' is marked as [PostVerbExecution] so it should have a single parameter of type CLAP.PostVerbExecutionContext".FormatWith(method));
                }
            }
        }

        private Method GetDefaultVerb()
        {
            var verbs = GetVerbs();

            return verbs.FirstOrDefault(v => v.IsDefault);
        }

        #endregion Private Methods
    }
}