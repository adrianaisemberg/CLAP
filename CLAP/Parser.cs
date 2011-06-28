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
    public class Parser
    {
        #region Fields

        // The possible prefixes of a parameter
        //
        private readonly static string[] s_prefixes = new[] { "/", "-" };

        // The type to reflect it's verbs
        //
        private readonly Type m_type;

        private readonly Dictionary<string, GlobalParameterHandler> m_globalRegisteredHandlers;

        private Dictionary<string, Action<string>> m_registeredHelpHandlers;
        private Action m_registeredEmptyHandler;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Creates a parser for the given type
        /// </summary>
        public Parser(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            m_type = type;
            m_globalRegisteredHandlers = new Dictionary<string, GlobalParameterHandler>();
            m_registeredHelpHandlers = new Dictionary<string, Action<string>>();
        }

        #endregion Constructors

        #region Public Methods

        public static Parser Create<T>()
        {
            return new Parser(typeof(T));
        }

        /// <summary>
        /// Executes a static verb on the type
        /// </summary>
        public void Run(string[] args)
        {
            Run(args, null);
        }

        /// <summary>
        /// Executes an instance verb on the object
        /// </summary>
        public void Run(string[] args, object obj)
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
            var method = GetMethod(m_type, verb);

            // if no method is found - a default has to exist
            //
            if (method == null)
            {
                // if there is a verb input but no method was found - throw
                if (verb != null && !s_prefixes.Any(p => p.Equals(verb[0].ToString())))
                {
                    throw new MissingVerbException(verb);
                }

                // no default - error
                //
                if (!m_type.HasAttribute<DefaultVerbAttribute>())
                {
                    throw new MissingDefaultVerbException();
                }

                verb = m_type.GetAttribute<DefaultVerbAttribute>().Verb;

                method = GetMethod(m_type, verb);

                // there is a verb, but no method matches it
                //
                if (method == null)
                {
                    throw new MissingVerbException(verb);
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
                        throw new MissingRequiredArgumentException(p.Name);
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
                }

                if (value == null && inputKey != null)
                {
                    value = GetValueForParameter(p.ParameterType, inputKey, stringValue);
                }

                // validation
                //
                if (value != null && Attribute.IsDefined(p, typeof(ValidationAttribute)))
                {
                    var validators = p.GetAttributes<ValidationAttribute>().Select(a => a.Validator);

                    // all validators must pass
                    //
                    foreach (var validator in validators)
                    {
                        validator.Validate(value);
                    }
                }

                // we have a valid value - add it to the list of parameters
                //
                parameters.Add(value);
            }

            // invoke the method with the list of parameters
            //
            method.MethodInfo.Invoke(obj, parameters.ToArray());
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
        public void RegisterParameterHandler<T>(string names, Action<T> action)
        {
            RegisterParameterHandler(
                names,
                action,
                null);
        }

        /// <summary>
        /// Registers an action to a global parameter name
        /// </summary>
        public void RegisterParameterHandler<T>(string names, Action<T> action, string description)
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
            var verbs = GetVerbs(m_type);

            if (verbs.None())
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            foreach (var verb in verbs)
            {
                sb.AppendLine();

                sb.AppendLine("{0}: {1}".FormatWith(verb.Names.StringJoin("/"), verb.Description));

                var parameters = GetParameters(verb.MethodInfo);

                foreach (var p in parameters)
                {
                    sb.AppendLine(" -{0}: {1}".FormatWith(p.Names.StringJoin("/"), GetParameterOption(p)));
                }
            }

            if (m_globalRegisteredHandlers.Any() || GetDefinedGlobals().Any())
            {
                sb.AppendLine();
                sb.AppendLine("Global parameters:");

                foreach (var handler in m_globalRegisteredHandlers.Values)
                {
                    sb.AppendLine(" -{0}: {1} [{2}]".FormatWith(handler.Name, handler.Desription, handler.Type.Name));
                }

                var definedGlobals = GetDefinedGlobals();

                foreach (var handler in definedGlobals)
                {
                    sb.AppendLine(" -{0}".FormatWith(GetDefinedGlobalHelpString(handler)));
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
                throw new InvalidOperationException("Empty handler is already registered");
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
                throw new InvalidOperationException("Empty handler is already registered");
            }

            m_registeredEmptyHandler = handler;
        }

        #endregion Public Methods

        #region Private Methods

        private void RegisterParameterHandlerInternal<T>(string names, Action<T> action, string description)
        {
            RegisterParameterHandlerInternal(
                names.CommaSplit(),
                action,
                description);
        }

        private void RegisterParameterHandlerInternal<T>(IEnumerable<string> names, Action<T> action, string description)
        {
            var objectAction = new Action<string>(str =>
            {
                object v = null;

                if (typeof(T) == typeof(bool) && str == null)
                {
                    v = true;
                }
                else
                {
                    v = GetValueForParameter(typeof(T), string.Empty, str);
                }

                action((T)v);
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
                        Type = typeof(T),
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

        private static object GetValueForParameter(Type parameterType, string inputKey, string stringValue)
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
                var convertToArrayMethod = typeof(Parser).GetMethod(
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
        private static T[] ConvertToArray<T>(string[] values)
        {
            return values.Select(c => ConvertString(c, typeof(T))).Cast<T>().ToArray();
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
        private static IEnumerable<Method> GetVerbs(Type type)
        {
            var verbMethods = type.GetMethodsWith<VerbAttribute>().
                Select(m => new Method(m));

            return verbMethods;
        }

        /// <summary>
        /// Get the method that matches the given verb
        /// </summary>
        private static Method GetMethod(Type type, string verb)
        {
            var verbs = GetVerbs(type);

            var method = verbs.FirstOrDefault(v => v.Names.Contains(verb.ToLowerInvariant()));

            return method;
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
            foreach (var kvp in args)
            {
                GlobalParameterHandler handler = null;

                if (m_globalRegisteredHandlers.TryGetValue(kvp.Key, out handler))
                {
                    handler.Handler(kvp.Value);
                }
            }
        }

        private IEnumerable<MethodInfo> GetDefinedGlobals()
        {
            var globals = m_type.GetMethodsWith<GlobalAttribute>();

            return globals;
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
                                var validators = p.GetAttributes<ValidationAttribute>().Select(a => a.Validator);

                                // all validators must pass
                                //
                                foreach (var validator in validators)
                                {
                                    validator.Validate(value);
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

            var definedHelpMethods = m_type.GetMethodsWith<HelpAttribute>();

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

            var definedEmptyHandlers = m_type.GetMethodsWith<EmptyAttribute>();

            if (definedEmptyHandlers.Count() > 1)
            {
                throw new MoreThanOneEmptyHandlerException();
            }

            foreach (var method in definedEmptyHandlers)
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

        #endregion Types
    }

    #region Parser<T>

    /// <summary>
    /// A command-line arguments parser
    /// </summary>
    public static class Parser<T>
    {
        /// <summary>
        /// Executes a verb and arguments
        /// </summary>
        public static void Run(string[] args)
        {
            var p = new Parser(typeof(T));

            p.Run(args);
        }

        /// <summary>
        /// Executes a verb and arguments on the object
        /// </summary>
        public static void Run(string[] args, T t)
        {
            var p = new Parser(typeof(T));

            p.Run(args, t);
        }
    }

    #endregion Parser<T>
}