using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;

#if !FW2
using System.Linq;
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

        private readonly Dictionary<string, GlobalParameterHandler> m_globalRegisteredHandlers;

        private Dictionary<string, Action<string>> m_registeredHelpHandlers;
        private Action m_registeredEmptyHandler;
        private Action<Exception> m_registeredErrorHandler;
        private bool m_registeredErrorHandlerRethrow;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a parser for the given type
        /// </summary>
        public Parser()
        {
            m_globalRegisteredHandlers = new Dictionary<string, GlobalParameterHandler>();
            m_registeredHelpHandlers = new Dictionary<string, Action<string>>();

            Validate();
        }

        #endregion Constructors

        #region Public Methods

        public void Run(string[] args)
        {
            RunInternal(args, null);
        }

        public void Run(string[] args, object obj)
        {
            RunInternal(args, obj);
        }

        private void RunInternal(string[] args, object obj)
        {
            try
            {
                // no args
                //
                if (args.None())
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
                    // if there is a verb input but no method was found - throw
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
                var list = GetParameters(method.MethodInfo);

                // the actual refelcted parameters
                //
                var methodParameters = method.MethodInfo.GetParameters();

                // a list of values, used when invoking the method
                //
                var parameters = new List<object>();

                foreach (var p in methodParameters)
                {
                    var parameter = list.First(param => param.ParameterInfo == p);
                    var names = parameter.Names;

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
                        if (parameter.Required)
                        {
                            throw new MissingRequiredArgumentException(verb, p.Name);
                        }

                        // the default is the value
                        //
                        value = parameter.Default;

                        // convert the default value, if different from parameter's value (guid, for example)
                        //
                        if (value is string && !(p.ParameterType == typeof(string)))
                        {
                            value = GetValueForParameter(p.ParameterType, "{DEFAULT}", (string)value);
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
                        value = GetValueForParameter(p.ParameterType, inputKey, stringValue);
                    }

                    // validation
                    //
                    // each parameter:
                    //
                    if (value != null && p.HasAttribute<ValidationAttribute>())
                    {
                        var validators = p.GetAttributes<ValidationAttribute>().Select(a => a.GetValidator());

                        // all validators must pass
                        //
                        foreach (var validator in validators)
                        {
                            validator.Validate(p, value);
                        }
                    }

                    // we have a valid value - add it to the list of parameters
                    //
                    parameters.Add(value);
                }

                // validate all parameters
                //
                if (method.MethodInfo.HasAttribute<ParametersValidationAttribute>())
                {
                    var validators = method.MethodInfo.GetAttributes<ParametersValidationAttribute>().Select(a => a.GetValidator());

                    var parametersAndValues = new List<ParameterInfoAndValue>();

                    methodParameters.Each((p, i) =>
                    {
                        parametersAndValues.Add(new ParameterInfoAndValue(p, parameters[i]));
                    });

                    // all validators must pass
                    //
                    foreach (var validator in validators)
                    {
                        validator.Validate(parametersAndValues.ToArray());
                    }
                }

                if (inputArgs.Any())
                {
                    throw new UnhandledParametersException(inputArgs);
                }


                // invoke the method with the list of parameters
                //
                method.MethodInfo.Invoke(obj, parameters.ToArray());
            }
            catch (TargetInvocationException tex)
            {
                if (tex.InnerException != null)
                {
                    var rethrow = HandleError(tex.InnerException, obj);

                    if (rethrow)
                    {
                        throw;
                    }
                }
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

        /// <summary>
        /// Registers an action to a global parameter name
        /// </summary>
        public void RegisterParameterHandler(string names, Action action)
        {
            RegisterParameterHandler(
                names,
                new Action<bool>(delegate { action(); }),
                null);
        }

        /// <summary>
        /// Registers an action to a global parameter name
        /// </summary>
        public void RegisterParameterHandler(string names, Action action, string description)
        {
            RegisterParameterHandler(
                names,
                new Action<bool>(delegate { action(); }),
                description);
        }

        /// <summary>
        /// Registers an action to a global parameter name
        /// </summary>
        public void RegisterParameterHandler<TParameter>(string names, Action<TParameter> action)
        {
            RegisterParameterHandler(
                names,
                action,
                null);
        }

        /// <summary>
        /// Registers an action to a global parameter name
        /// </summary>
        public void RegisterParameterHandler<TParameter>(string names, Action<TParameter> action, string description)
        {
            RegisterParameterHandlerInternal(
                names,
                action,
                description);
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

                if (verb.MethodInfo.HasAttribute<ParametersValidationAttribute>())
                {
                    sb.AppendLine();
                    sb.AppendLine("Validation:");

                    var validators = verb.MethodInfo.GetAttributes<ParametersValidationAttribute>();

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

            if (m_globalRegisteredHandlers.Any() || definedGlobals.Any())
            {
                sb.AppendLine();
                sb.AppendLine("Global parameters:");

                foreach (var handler in m_globalRegisteredHandlers.Values)
                {
                    sb.AppendLine(" -{0}: {1} [{2}]".FormatWith(handler.Name, handler.Desription, handler.Type.Name));
                }

                foreach (var handler in definedGlobals)
                {
                    sb.AppendLine(" -{0}".FormatWith(GetDefinedGlobalHelpString(handler)));

                    if (handler.HasAttribute<ParametersValidationAttribute>())
                    {
                        var validators = handler.GetAttributes<ParametersValidationAttribute>();

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

        /// <summary>
        /// Registers a parameter name to handle the help string
        /// </summary>
        /// <param name="names">The parameter name</param>
        /// <param name="helpHandler">The help string handler. For example: help => Console.WriteLine(help)</param>
        public void RegisterHelpHandler(string names, Action<string> helpHandler)
        {
            RegisterHelpHandlerInternal(names, helpHandler);
        }

        public void RegisterEmptyHelpHandler(Action<string> handler)
        {
            if (m_registeredEmptyHandler != null)
            {
                throw new MoreThanOneEmptyHandlerException();
            }

            var help = GetHelpString();

            m_registeredEmptyHandler = delegate
            {
                handler(help);
            };
        }

        public void RegisterEmptyHandler(Action handler)
        {
            if (m_registeredEmptyHandler != null)
            {
                throw new MoreThanOneEmptyHandlerException();
            }

            m_registeredEmptyHandler = handler;
        }

        public void RegisterErrorHandler(Action<Exception> handler)
        {
            RegisterErrorHandler(handler, false);
        }

        public void RegisterErrorHandler(Action<Exception> handler, bool rethrow)
        {
            if (m_registeredErrorHandler != null)
            {
                throw new MoreThanOneErrorHandlerException();
            }

            m_registeredErrorHandler = handler;
            m_registeredErrorHandlerRethrow = rethrow;
        }

        #endregion Public Methods

        #region Private Methods

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

        }

        private void RegisterParameterHandlerInternal<TParameter>(string names, Action<TParameter> action, string description)
        {
            RegisterParameterHandlerInternal(
                names.CommaSplit(),
                action,
                description);
        }

        private void RegisterParameterHandlerInternal<TParameter>(IEnumerable<string> names, Action<TParameter> action, string description)
        {
            var objectAction = new Action<string>(str =>
            {
                object v = null;

                if (typeof(TParameter) == typeof(bool) && str == null)
                {
                    v = true;
                }
                else
                {
                    v = GetValueForParameter(typeof(TParameter), string.Empty, str);
                }

                action((TParameter)v);
            });

            foreach (var name in names)
            {
                m_globalRegisteredHandlers.Add(
                    name,
                    new GlobalParameterHandler
                    {
                        Name = name,
                        Handler = objectAction,
                        Desription = description,
                        Type = typeof(TParameter),
                    });
            }
        }

        private void RegisterHelpHandlerInternal(string names, Action<string> helpHandler)
        {
            RegisterHelpHandlerInternal(
                names.CommaSplit(),
                helpHandler);
        }

        private void RegisterHelpHandlerInternal(IEnumerable<string> names, Action<string> helpHandler)
        {
            foreach (var name in names)
            {
                var key = name.ToLowerInvariant();

                if (m_registeredHelpHandlers.ContainsKey(key))
                {
                    throw new InvalidOperationException("'{0}' is already registered as a help handler".FormatWith(key));
                }

                m_registeredHelpHandlers.Add(key, helpHandler);
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

            if (m_registeredHelpHandlers.TryGetValue(input.ToLowerInvariant(), out action))
            {
                return action;
            }

            return null;
        }

        private object GetValueForParameter(Type parameterType, string inputKey, string stringValue)
        {
            // in case of a switch - the default is true/false according to the switch
            //
            if (parameterType == typeof(Boolean) && stringValue == null)
            {
                return inputKey != null;
            }
            // if array
            else if (parameterType.IsArray)
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
            try
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
            catch (Exception ex)
            {
                throw new TypeConvertionException(value, type, ex);
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

                var parts = arg.Substring(1).Split(new[] { '=', ':' }, 2, StringSplitOptions.RemoveEmptyEntries);
                var name = parts[0].ToLowerInvariant();

                string valueString = null;

                // a switch (a boolean parameter) doesn't need to have a separator,
                // in that case, a null string value is mapped
                //
                if (parts.Length > 1)
                {
                    valueString = parts[1];
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

                if (m_globalRegisteredHandlers.TryGetValue(kvp.Key, out handler))
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
                                var validators = p.GetAttributes<ValidationAttribute>().Select(a => a.GetValidator());

                                // all validators must pass
                                //
                                foreach (var validator in validators)
                                {
                                    validator.Validate(p, value);
                                }
                            }

                            if (method.HasAttribute<ParametersValidationAttribute>())
                            {
                                var validators = method.GetAttributes<ParametersValidationAttribute>().
                                    Select(a => a.GetValidator());

                                foreach (var validator in validators)
                                {
                                    validator.Validate(new[]
                                    {
                                        new ParameterInfoAndValue(p, value),
                                    });
                                }
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
            if (m_registeredEmptyHandler != null)
            {
                m_registeredEmptyHandler();
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
                var parameters = defaultVerb.MethodInfo.GetParameters().Select(p => (object)null).ToArray();

                defaultVerb.MethodInfo.Invoke(target, parameters);
            }
        }

        private bool HandleError(Exception ex, object target)
        {
            if (m_registeredErrorHandler != null)
            {
                m_registeredErrorHandler(ex);

                return m_registeredErrorHandlerRethrow;
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

        internal class GlobalParameterHandler
        {
            internal string Name { get; set; }
            internal Action<string> Handler { get; set; }
            internal string Desription { get; set; }
            internal Type Type { get; set; }
        }

        internal class ErrorHandler
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