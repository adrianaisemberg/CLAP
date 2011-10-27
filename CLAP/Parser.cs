using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

#if !FW2
using System.Linq;
using CLAP.Interception;

#endif

namespace CLAP
{
    /// <summary>
    /// A command-line arguments parser
    /// </summary>
    public sealed class Parser<T>
    {
        #region Fields

        // The possible prefixes of a parameter
        //
        private readonly static string[] s_prefixes = new[] { "/", "-" };
        private readonly static string s_fileInputSuffix = "@";

        private readonly ParserRegistration m_registration;

        #endregion Fields

        #region Properties

        public ParserRegistration Register
        {
            get { return m_registration; }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Creates a parser for the given type
        /// </summary>
        public Parser()
        {
            m_registration = new ParserRegistration(GetHelpString, GetValueForParameter);

            Validate();
        }

        #endregion Constructors

        #region Public Methods

        public void Run(string[] args)
        {
            TryRunInternal(args, null);
        }

        public void Run(string[] args, object obj)
        {
            TryRunInternal(args, obj);
        }

        /// <summary>
        /// Returns a help string
        /// </summary>
        public string GetHelpString()
        {
            var verbs = GetVerbs();

            if (verbs.None())
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            foreach (var verb in verbs)
            {
                sb.AppendLine();

                sb.Append(verb.Names.StringJoin("/")).Append(":");

                if (verb.IsDefault)
                {
                    sb.Append(" [Default]");
                }

                if (!string.IsNullOrEmpty(verb.Description))
                {
                    sb.AppendFormat(" {0}", verb.Description);
                }

                var validators = verb.MethodInfo.GetInterfaceAttributes<ICollectionValidation>();

                if (validators.Any())
                {
                    sb.AppendLine();
                    sb.AppendLine("Validation:");

                    foreach (var validator in validators)
                    {
                        sb.AppendLine("- {0}".FormatWith(validator.Description));
                    }
                }

                sb.AppendLine();

                var parameters = GetParameters(verb.MethodInfo);

                foreach (var p in parameters)
                {
                    sb.AppendLine(" -{0}: {1}".FormatWith(p.Names.StringJoin("/"), GetParameterOption(p)));
                }
            }

            var definedGlobals = GetDefinedGlobals();

            if (m_registration.RegisteredGlobalHandlers.Any() || definedGlobals.Any())
            {
                sb.AppendLine();
                sb.AppendLine("Global parameters:");

                foreach (var handler in m_registration.RegisteredGlobalHandlers.Values)
                {
                    sb.AppendLine(" -{0}: {1} [{2}]".FormatWith(handler.Name, handler.Desription, handler.Type.Name));
                }

                foreach (var handler in definedGlobals)
                {
                    sb.AppendLine(" -{0}".FormatWith(GetDefinedGlobalHelpString(handler)));

                    var validators = handler.GetInterfaceAttributes<ICollectionValidation>();

                    if (validators.Any())
                    {
                        sb.AppendLine("  Validation:");

                        foreach (var validator in validators)
                        {
                            sb.AppendLine("  {0}".FormatWith(validator.Description));
                        }
                    }
                }
            }

            return sb.ToString();
        }

        #endregion Public Methods

        #region Private Methods

        private void TryRunInternal(string[] args, object obj)
        {
            try
            {
                RunInternal(args, obj);
            }
            catch (Exception ex)
            {
                var rethrow = HandleError(ex, obj);

                if (rethrow)
                {
                    throw;
                }
            }
        }

        private void RunInternal(string[] args, object obj)
        {
            // no args
            //
            if (args.None() | args.All(a => string.IsNullOrEmpty(a)))
            {
                HandleEmptyArguments(obj);

                return;
            }

            // the first arg should be the verb, unless there is no verb and a default is used
            //
            var firstArg = args[0];

            if (HandleHelp(firstArg, obj))
            {
                return;
            }

            var verb = firstArg;

            // a flag, in case there is no verb
            //
            var noVerb = false;

            // find the method by the given verb
            //
            var typeVerbs = GetVerbs();

            var method = typeVerbs.FirstOrDefault(v => v.Names.Contains(verb.ToLowerInvariant()));

            // if no method is found - a default must exist
            //
            if (method == null)
            {
                // if there is a verb input but no method was found
                // AND
                // the first arg is not an input argument (doesn't start with "-" etc)
                //
                if (verb != null && !s_prefixes.Any(p => p.Equals(verb[0].ToString())))
                {
                    throw new MissingVerbException(verb);
                }

                method = typeVerbs.FirstOrDefault(v => v.IsDefault);

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
            var parameterValues = CreateParameterValues(verb, inputArgs, paremetersList);

            ValidateVerbInput(method, parameterValues.Select(kvp => kvp.Value).ToList());

            // if some args weren't handled
            //
            if (inputArgs.Any())
            {
                throw new UnhandledParametersException(inputArgs);
            }

            Execute(obj, method, parameterValues);
        }

        private void Execute(
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
                    method.MethodInfo.Invoke(target, parameters.Select(p => p.Value).ToArray());
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

            if (m_registration.RegisteredPostVerbInterceptor != null)
            {
                m_registration.RegisteredPostVerbInterceptor(postVerbExecutionContext);
            }
            else
            {
                var postInterceptionMethods = typeof(T).GetMethodsWith<PostVerbExecutionAttribute>();

                if (postInterceptionMethods.Any())
                {
                    Debug.Assert(postInterceptionMethods.Count() == 1);

                    var postInterceptionMethod = postInterceptionMethods.First();

                    postInterceptionMethod.Invoke(target, new[] { postVerbExecutionContext });
                }
            }
        }

        private PreVerbExecutionContext PreInterception(
            object target,
            Method method,
            ParameterAndValue[] parameters)
        {
            var preVerbExecutionContext = new PreVerbExecutionContext(method, target, parameters);

            if (m_registration.RegisteredPreVerbInterceptor != null)
            {
                m_registration.RegisteredPreVerbInterceptor(preVerbExecutionContext);
            }
            else
            {
                var preInterceptionMethods = typeof(T).GetMethodsWith<PreVerbExecutionAttribute>();

                if (preInterceptionMethods.Any())
                {
                    Debug.Assert(preInterceptionMethods.Count() == 1);

                    var preInterceptionMethod = preInterceptionMethods.First();

                    preInterceptionMethod.Invoke(target, new[] { preVerbExecutionContext });
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

        private ParameterAndValue[] CreateParameterValues(
            string verb,
            Dictionary<string, string> inputArgs,
            IEnumerable<Parameter> list)
        {
            var parameters = new List<ParameterAndValue>();

            foreach (var p in list)
            {
                var parameterInfo = p.ParameterInfo;

                var names = p.Names;

                // according to the parameter names, try to find a match from the input
                //
                var inputKey = names.FirstOrDefault(n => inputArgs.ContainsKey(n));

                // the input value
                //
                string stringValue = null;

                // the actual value, converted to the relevant parameter type
                //
                object value = null;

                // if no input was found that matches this parameter
                //
                if (inputKey == null)
                {
                    if (p.Required)
                    {
                        throw new MissingRequiredArgumentException(verb, parameterInfo.Name);
                    }

                    // the default is the value
                    //
                    value = p.Default;

                    // convert the default value, if different from parameter's value (guid, for example)
                    //
                    if (value is string && !(parameterInfo.ParameterType == typeof(string)))
                    {
                        value = GetValueForParameter(parameterInfo.ParameterType, "{DEFAULT}", (string)value);
                    }
                }
                else
                {
                    stringValue = inputArgs[inputKey];

                    // remove it so later we'll see which ones were not handled
                    //
                    inputArgs.Remove(inputKey);
                }

                if (value == null && inputKey != null)
                {
                    value = GetValueForParameter(parameterInfo.ParameterType, inputKey, stringValue);
                }

                // validate each parameter
                //
                if (value != null && parameterInfo.HasAttribute<ValidationAttribute>())
                {
                    var validators = parameterInfo.GetAttributes<ValidationAttribute>().Select(a => a.GetValidator());

                    // all validators must pass
                    //
                    foreach (var validator in validators)
                    {
                        validator.Validate(new ValueInfo(parameterInfo.Name, parameterInfo.ParameterType, value));
                    }
                }

                // we have a valid value - add it to the list of parameters
                //
                parameters.Add(new ParameterAndValue(p, value));
            }

            return parameters.ToArray();
        }

        private void Validate()
        {
            // no more than one default verb
            var verbMethods = typeof(T).GetMethodsWith<VerbAttribute>().
                Select(m => new Method(m));

            var defaultVerbs = verbMethods.Where(m => m.IsDefault);

            if (defaultVerbs.Count() > 1)
            {
                throw new MoreThanOneDefaultVerbException(defaultVerbs.Select(m => m.MethodInfo.Name));
            }

            // no more than one error handler
            //
            ValidateDefinedErrorHandlers();

            // no more than one empty handler
            //
            var definedEmptyHandlers = typeof(T).GetMethodsWith<EmptyAttribute>();

            if (definedEmptyHandlers.Count() > 1)
            {
                throw new MoreThanOneEmptyHandlerException();
            }

            // no more that one pre/post interception methods
            //
            var preInterceptionMethods = typeof(T).GetMethodsWith<PreVerbExecutionAttribute>();

            if (preInterceptionMethods.Count() > 1)
            {
                throw new MoreThanOnePreVerbInterceptorException();
            }

            var postInterceptionMethods = typeof(T).GetMethodsWith<PostVerbExecutionAttribute>();

            if (postInterceptionMethods.Count() > 1)
            {
                throw new MoreThanOnePostVerbInterceptorException();
            }
        }

        private string GetDefinedGlobalHelpString(MethodInfo method)
        {
            var sb = new StringBuilder();

            // name
            //
            var globalAtt = method.GetAttribute<GlobalAttribute>();
            var name = globalAtt.Name ?? method.Name;
            sb.Append(name);

            foreach (var alias in globalAtt.Aliases)
            {
                sb.AppendFormat("/{0}", alias);
            }

            sb.Append(": ");

            // description
            //
            sb.Append(globalAtt.Description);
            sb.Append(" ");

            // type
            //
            var parameters = GetParameters(method);
            if (parameters.Any())
            {
                var p = parameters.First();

                sb.Append(GetParameterOption(p));
            }
            else
            {
                sb.AppendFormat("[Boolean]");
            }

            return sb.ToString();
        }

        private Action<string> GetHelpHandler(string input)
        {
            Debug.Assert(!string.IsNullOrEmpty(input));

            Action<string> action = null;

            if (m_registration.RegisteredHelpHandlers.TryGetValue(input.ToLowerInvariant(), out action))
            {
                return action;
            }

            return null;
        }

        private object GetValueForParameter(Type parameterType, string inputKey, string stringValue)
        {
            // a string doesn't need convertion
            //
            if (parameterType == typeof(string) && stringValue != null)
            {
                return stringValue;
            }

            // in case of a switch - the default is true/false according to the switch
            //
            if (parameterType == typeof(Boolean) && stringValue == null)
            {
                return inputKey != null;
            }
            else
            {
                // try JSON/XML deserializing it
                //
                try
                {
                    object obj = null;

                    if (Serialization.Deserialize(stringValue, parameterType, ref obj))
                    {
                        TypeValidator.Validate(obj);

                        return obj;
                    }
                    else
                    {
                        // if can't deserialize - try converting it
                        //
                        return ConvertParameterValue(inputKey, stringValue, parameterType);
                    }
                }
                catch (ValidationException)
                {
                    // validation exceptions are good to throw out
                    //
                    throw;
                }
                catch (Exception ex)
                {
                    // tried deserialize but failed - try converting
                    //
                    return ConvertParameterValue(inputKey, stringValue, parameterType, ex);
                }
            }

            throw new MissingArgumentValueException(inputKey);
        }

        private object ConvertParameterValue(string inputKey, string stringValue, Type parameterType)
        {
            return ConvertParameterValue(inputKey, stringValue, parameterType, null);
        }

        private object ConvertParameterValue(
            string inputKey,
            string stringValue,
            Type parameterType,
            Exception deserializationException)
        {
            try
            {
                // if array
                if (parameterType.IsArray)
                {
                    var stringValues = stringValue.CommaSplit();

                    // The type of the array element
                    //
                    var type = parameterType.GetElementType();

                    // Create a generic instance of the ConvertToArray method
                    //
                    var convertToArrayMethod = GetType().GetMethod(
                            "ConvertToArray",
                            BindingFlags.NonPublic | BindingFlags.Static).
                        MakeGenericMethod(type);

                    // Run the array converter
                    //
                    return convertToArrayMethod.Invoke(null, new[] { stringValues });
                }
                // if there is an input value
                else if (stringValue != null)
                {
                    // convert the string value to the relevant parameter type
                    //
                    return ConvertString(stringValue, parameterType);
                }
            }
            catch
            {
                throw new TypeConvertionException(stringValue, parameterType, deserializationException);
            }

            throw new MissingArgumentValueException(inputKey);
        }

        /// <summary>
        /// This method is called via reflection
        /// </summary>
        private static TConvert[] ConvertToArray<TConvert>(string[] values)
        {
            return values.Select(c => ConvertString(c, typeof(TConvert))).Cast<TConvert>().ToArray();
        }

        /// <summary>
        /// Convert a string to a type
        /// </summary>
        private static object ConvertString(string value, Type type)
        {
            if (type.IsEnum)
            {
                return Enum.Parse(type, value);
            }
            else if (type == typeof(Guid))
            {
                return string.IsNullOrEmpty(value) ? (object)null : new Guid(value);
            }
            else if (type == typeof(Uri))
            {
                return string.IsNullOrEmpty(value) ? (object)null : new Uri(Environment.ExpandEnvironmentVariables(value));
            }
            else
            {
                return Convert.ChangeType(value, type);
            }
        }

        /// <summary>
        /// Used for GetHelpString
        /// </summary>
        private static string GetParameterOption(Parameter p)
        {
            var sb = new StringBuilder();

            sb.AppendFormat("{0} [", p.Description);

            sb.Append(p.ParameterInfo.ParameterType.Name);

            if (p.ParameterInfo.ParameterType.IsEnum)
            {
                var values = Enum.GetNames(p.ParameterInfo.ParameterType);

                sb.AppendFormat(":[{0}]", values.StringJoin(","));
            }

            if (p.Required)
            {
                sb.Append(", Required");
            }
            if (p.Default != null)
            {
                sb.AppendFormat(", Default = {0}", p.Default);
            }

            var validationAttributes = p.ParameterInfo.GetAttributes<ValidationAttribute>();

            foreach (var validationAttribute in validationAttributes)
            {
                sb.AppendFormat(", {0}", validationAttribute.Description);
            }

            sb.Append("]");

            return sb.ToString();
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
                if (!s_prefixes.Any(p => arg.StartsWith(p)))
                {
                    throw new MissingArgumentPrefixException(arg, string.Join(",", s_prefixes));
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
        private static IEnumerable<Parameter> GetParameters(MethodInfo method)
        {
            var parameters = method.GetParameters().Select(p => new Parameter(p));

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

            return parameters;
        }

        /// <summary>
        /// Create a list of methods (verbs) for the given type
        /// </summary>
        private IEnumerable<Method> GetVerbs()
        {
            var verbMethods = typeof(T).GetMethodsWith<VerbAttribute>().
                Select(m => new Method(m));

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
                GlobalParameterHandler handler = null;

                if (m_registration.RegisteredGlobalHandlers.TryGetValue(kvp.Key, out handler))
                {
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

        private IEnumerable<MethodInfo> GetDefinedGlobals()
        {
            var globals = typeof(T).GetMethodsWith<GlobalAttribute>();

            return globals;
        }

        private IEnumerable<ErrorHandler> GetDefinedErrorHandlers()
        {
            var errorHandlers = typeof(T).GetMethodsWith<ErrorAttribute>();

            return errorHandlers.Select(m => new ErrorHandler
            {
                Method = m,
                ReThrow = m.GetAttribute<ErrorAttribute>().ReThrow,
            });
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
                        method.Invoke(obj, null);
                    }
                    else if (parameters.Length == 1)
                    {
                        string stringValue;

                        if (args.TryGetValue(key, out stringValue))
                        {
                            var p = parameters.First();

                            var value = GetValueForParameter(p.ParameterType, key, stringValue);

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

                            method.Invoke(obj, new[] { value });
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

            if (s_prefixes.Contains(firstArg[0].ToString()))
            {
                arg = firstArg.Substring(1);
            }

            Action<string> helpHandler = GetHelpHandler(arg);

            string help = null;

            var helpHandled = false;

            if (helpHandler != null)
            {
                help = GetHelpString();

                helpHandler(help);

                helpHandled = true;
            }

            var definedHelpMethods = typeof(T).GetMethodsWith<HelpAttribute>();

            foreach (var method in definedHelpMethods)
            {
                var att = method.GetAttribute<HelpAttribute>();

                var name = att.Name ?? method.Name;

                if (name.Equals(arg, StringComparison.InvariantCultureIgnoreCase) ||
                    att.Aliases.CommaSplit().Any(s => s.Equals(arg, StringComparison.InvariantCultureIgnoreCase)))
                {
                    try
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

                        var obj = method.IsStatic ? null : target;

                        if (help == null)
                        {
                            help = GetHelpString();
                        }

                        method.Invoke(obj, new[] { help });

                        helpHandled = true;
                    }
                    catch (TargetParameterCountException ex)
                    {
                        throw new ArgumentMismatchException(
                            "Method '{0}' is marked as [Help] but it does not have a single string parameter".FormatWith(method.Name),
                            ex);
                    }
                    catch (ArgumentException ex)
                    {
                        throw new ArgumentMismatchException(
                            "Method '{0}' is marked as [Help] but it does not have a single string parameter".FormatWith(method.Name),
                            ex);
                    }
                }
            }

            return helpHandled;
        }

        private void HandleEmptyArguments(object target)
        {
            if (m_registration.RegisteredEmptyHandler != null)
            {
                m_registration.RegisteredEmptyHandler();
            }

            var definedEmptyHandlers = typeof(T).GetMethodsWith<EmptyAttribute>();
            var definedEmptyHandlersCount = definedEmptyHandlers.Count();

            Debug.Assert(definedEmptyHandlersCount <= 1);

            if (definedEmptyHandlersCount == 1)
            {
                var method = definedEmptyHandlers.First();

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

                var obj = method.IsStatic ? null : target;

                // if it is a [Help] handler
                if (method.HasAttribute<HelpAttribute>())
                {
                    try
                    {
                        var help = GetHelpString();

                        method.Invoke(obj, new[] { help });
                    }
                    catch (TargetParameterCountException ex)
                    {
                        throw new ArgumentMismatchException(
                            "Method '{0}' is marked as [Help] so it should have a single string parameter".FormatWith(method),
                            ex);
                    }
                    catch (ArgumentException ex)
                    {
                        throw new ArgumentMismatchException(
                            "Method '{0}' is marked as [Help] so it should have a single string parameter".FormatWith(method),
                            ex);
                    }
                }
                else
                {
                    try
                    {
                        method.Invoke(obj, null);
                    }
                    catch (TargetParameterCountException ex)
                    {
                        throw new ArgumentMismatchException(
                            "Method '{0}' is marked as [Empty] so it should not have any parameters".FormatWith(method),
                            ex);
                    }
                }
            }

            var defaultVerb = GetDefaultVerb();

            // if there is a default verb - execute it
            //
            if (defaultVerb != null)
            {
                // create an array of (null) arguments that matches the method
                //
                var parameters = defaultVerb.MethodInfo.GetParameters().
                    Select(p => new ParameterAndValue(new Parameter(p), (object)null)).ToArray();

                Execute(target, defaultVerb, parameters);
            }
        }

        private bool HandleError(Exception ex, object target)
        {
            if (m_registration.RegisteredErrorHandler != null)
            {
                var rethrow = m_registration.RegisteredErrorHandler(ex);

                return rethrow;
            }
            else
            {
                var definedErrorHandlers = GetDefinedErrorHandlers();

                Debug.Assert(definedErrorHandlers.Count() <= 1);

                var handler = definedErrorHandlers.FirstOrDefault();

                if (handler != null)
                {
                    var parameters = handler.Method.GetParameters();

                    if (parameters.None())
                    {
                        handler.Method.Invoke(target, null);
                    }
                    else
                    {
                        Debug.Assert(parameters.Length == 1);
                        Debug.Assert(parameters[0].ParameterType == typeof(Exception));

                        handler.Method.Invoke(target, new[] { ex });
                    }

                    return handler.ReThrow;
                }

                // no handler - rethrow
                //
                return true;
            }
        }

        private void ValidateDefinedErrorHandlers()
        {
            // check for too many defined global error handlers
            //
            var definedErrorHandlers = GetDefinedErrorHandlers();

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
            var method = definedErrorHandlers.First().Method;

            var parameters = method.GetParameters();

            // no parameters - good
            //
            if (parameters.None())
            {
                return;
            }
            else if (parameters.Length > 1)
            {
                throw new ArgumentMismatchException(
                    "Method '{0}' is marked as [Error] so it should have a single Exception parameter or none".FormatWith(method));
            }
            else
            {
                var parameter = parameters.First();

                if (parameter.ParameterType != typeof(Exception))
                {
                    throw new ArgumentMismatchException(
                        "Method '{0}' is marked as [Error] so it should have a single Exception parameter or none".FormatWith(method));
                }
            }
        }

        private Method GetDefaultVerb()
        {
            var verbs = GetVerbs();

            return verbs.FirstOrDefault(v => v.IsDefault);
        }

        #endregion Private Methods

        #region Types

        private class ErrorHandler
        {
            internal MethodInfo Method { get; set; }
            internal bool ReThrow { get; set; }
        }

        #endregion Types
    }

    /// <summary>
    /// A command-line arguments parser
    /// </summary>
    public static class Parser
    {
        public static void Run<T>(string[] args, T t)
        {
            var p = new Parser<T>();

            p.Run(args, t);
        }

        public static void Run<T>(string[] args)
        {
            var p = new Parser<T>();

            p.Run(args);
        }
    }
}