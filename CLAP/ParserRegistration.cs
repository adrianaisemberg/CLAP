using System;
using System.Collections.Generic;
using CLAP.Interception;
using System.Reflection;

#if !NET20
using System.Linq;
#endif

namespace CLAP
{
    /// <summary>
    /// Provides registration features for parser instances
    /// </summary>
    public sealed class ParserRegistration
    {
        #region Fields

        private readonly Type[] m_types;
        private IDictionary<string, Type> m_registeredParsersByAlias;

        #endregion Fields

        #region Properties

        internal Dictionary<string, GlobalParameterHandler> RegisteredGlobalHandlers { get; private set; }
        internal Dictionary<string, Action<string>> RegisteredHelpHandlers { get; private set; }
        internal Action RegisteredEmptyHandler { get; private set; }
        internal Action<ExceptionContext> RegisteredErrorHandler { get; private set; }
        internal Action<PreVerbExecutionContext> RegisteredPreVerbInterceptor { get; private set; }
        internal Action<PostVerbExecutionContext> RegisteredPostVerbInterceptor { get; private set; }

        internal Func<string> HelpGetter { get; private set; }
        internal Func<ParameterInfo, Type, string, string, object> ParameterValueGetter { get; private set; }

        

        #endregion Properties

        #region Constructors

        public ParserRegistration(
            Type[] types,
            Func<string> helpGetter)
        {
            m_types = types;

            RegisterParserTypeAliases();
            RegisteredGlobalHandlers = new Dictionary<string, GlobalParameterHandler>();
            RegisteredHelpHandlers = new Dictionary<string, Action<string>>();

            HelpGetter = helpGetter;
            ParameterValueGetter = ValuesFactory.GetValueForParameter;
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Registers a help handler that is executed when the user requests for help
        /// </summary>
        /// <param name="names">The names (CSV) to be registered as help parameters. For example: "?,h,help"</param>
        /// <param name="helpHandler">The action to be executed</param>
        public void HelpHandler(string names, Action<string> helpHandler)
        {
            if (helpHandler == null)
            {
                throw new ArgumentNullException("helpHandler");
            }

            RegisterHelpHandlerInternal(names, helpHandler);
        }

        /// <summary>
        /// Registers an empty help handler that is executed when there is no input
        /// </summary>
        /// <param name="handler">The action to be executed</param>
        public void EmptyHelpHandler(Action<string> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            EmptyHandler(delegate
            {
                var help = HelpGetter();

                handler(help);
            });
        }

        /// <summary>
        /// Registers an empty handler that is executed when there is no input
        /// </summary>
        /// <param name="handler">The action to be executed</param>
        public void EmptyHandler(Action handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            if (RegisteredEmptyHandler != null)
            {
                throw new MoreThanOneEmptyHandlerException();
            }

            RegisteredEmptyHandler = handler;
        }

        /// <summary>
        /// Registers an error handler that is executed when an exception is thrown
        /// </summary>
        /// <param name="handler">The action to be executed</param>
        public void ErrorHandler(Action<ExceptionContext> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            if (RegisteredErrorHandler != null)
            {
                throw new MoreThanOneErrorHandlerException();
            }

            RegisteredErrorHandler = handler;
        }

        /// <summary>
        /// Registers a pre-verb execution interceptor
        /// </summary>
        /// <param name="interceptor">The action to be executed before each verb is executed</param>
        public void PreVerbInterceptor(Action<PreVerbExecutionContext> interceptor)
        {
            if (interceptor == null)
            {
                throw new ArgumentNullException("interceptor");
            }

            if (RegisteredPreVerbInterceptor != null)
            {
                throw new MoreThanOnePreVerbInterceptorException();
            }

            RegisteredPreVerbInterceptor = interceptor;
        }

        /// <summary>
        /// Registers a post-verb execution interceptor
        /// </summary>
        /// <param name="interceptor">The action to be executed after each verb is executed</param>
        public void PostVerbInterceptor(Action<PostVerbExecutionContext> interceptor)
        {
            if (interceptor == null)
            {
                throw new ArgumentNullException("interceptor");
            }

            if (RegisteredPostVerbInterceptor != null)
            {
                throw new MoreThanOnePostVerbInterceptorException();
            }

            RegisteredPostVerbInterceptor = interceptor;
        }

        /// <summary>
        /// Registers a global parameter handler
        /// </summary>
        /// <param name="names">The names (CSV) to be registered as boolean parameters (switches)</param>
        /// <param name="action">The action to execute</param>
        public void ParameterHandler(string names, Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            ParameterHandler(
                names,
                new Action<bool>(delegate { action(); }),
                new ParameterOptions());
        }

        /// <summary>
        /// Registers a global parameter handler
        /// </summary>
        /// <param name="names">The names (CSV) to be registered as boolean parameters (switches)</param>
        /// <param name="action">The action to execute</param>
        /// <param name="description">The parameter description (for help generation)</param>
        public void ParameterHandler(string names, Action action, string description)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            ParameterHandler(
                names,
                new Action<bool>(delegate { action(); }),
                new ParameterOptions
                {
                    Description = description,
                });
        }

        /// <summary>
        /// Registers a global parameter handler
        /// </summary>
        /// <typeparam name="TParameter">The type of the parameter</typeparam>
        /// <param name="names">The names (CSV) to be registered as parameters</param>
        /// <param name="action">The action to execute</param>
        public void ParameterHandler<TParameter>(string names, Action<TParameter> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            ParameterHandler(
                names,
                action,
                new ParameterOptions());
        }

        /// <summary>
        /// Registers a global parameter handler
        /// </summary>
        /// <typeparam name="TParameter">The type of the parameter</typeparam>
        /// <param name="names">The names (CSV) to be registered as parameters</param>
        /// <param name="action">The action to execute</param>
        /// <param name="description">The parameter description (for help generation)</param>
        [Obsolete("Use ParameterOptions")]
        public void ParameterHandler<TParameter>(string names, Action<TParameter> action, string description)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            RegisterParameterHandlerInternal(
                names,
                action,
                new ParameterOptions
                {
                    Description = description,
                });
        }

        public void ParameterHandler<TParameter>(string names, Action<TParameter> action, ParameterOptions options)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            RegisterParameterHandlerInternal(
                names,
                action,
                options);
        }

        /// <summary>
        /// Attempts to get a registered type by the provided value. The value will first be checked against all of the
        /// registered type names, and then if no match is found, by the 'TargetAlias' values applied to the types (if any).
        /// </summary>
        /// <param name="typeNameOrAlias">The name of the requested type or 'null'</param>
        /// <returns>The matching type, or 'null' if no match is found.</returns>
        public Type GetTargetType(string typeNameOrAlias)
        {
            var matchByTypeName = m_types.FirstOrDefault(t => t.Name.Equals(typeNameOrAlias, StringComparison.OrdinalIgnoreCase));

            if (matchByTypeName != null)
                return matchByTypeName;

            return m_registeredParsersByAlias.ContainsKey(typeNameOrAlias)
                       ? m_registeredParsersByAlias[typeNameOrAlias]
                       : null;
        }

        #endregion Public Methods

        #region Private Methods

        private void RegisterParserTypeAliases()
        {
            foreach (var type in m_types)
            {
                var parserTypeTargetAlias = GetTargetAliasAttributeValue(type);
                if (string.IsNullOrEmpty(parserTypeTargetAlias))
                    continue;

                if (m_registeredParsersByAlias == null)
                    m_registeredParsersByAlias = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

                if (m_registeredParsersByAlias.ContainsKey(parserTypeTargetAlias))
                    throw new DuplicateTargetAliasException(parserTypeTargetAlias);

                m_registeredParsersByAlias.Add(parserTypeTargetAlias, type);
            }
        }

        private string GetTargetAliasAttributeValue(Type targetType)
        {
            var aliasAttribute = targetType.GetCustomAttributes(typeof(TargetAliasAttribute), false).FirstOrDefault() as TargetAliasAttribute;
            return (aliasAttribute != null) ? aliasAttribute.Alias : string.Empty;
        }

        private void RegisterParameterHandlerInternal<TParameter>(string names, Action<TParameter> action, ParameterOptions options)
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
                    v = ParameterValueGetter(null, typeof(TParameter), string.Empty, str);
                }

                action((TParameter)v);
            });

            RegisteredGlobalHandlers.Add(
                names,
                new GlobalParameterHandler
                {
                    Names = names.CommaSplit(),
                    Handler = objectAction,
                    Description = options.Description,
                    Separator = options.Separator,
                    Type = typeof(TParameter),
                });
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

                if (RegisteredHelpHandlers.ContainsKey(key))
                {
                    throw new InvalidOperationException("'{0}' is already registered as a help handler".FormatWith(key));
                }

                RegisteredHelpHandlers.Add(key, helpHandler);
            }
        }

        #endregion Private Methods
    }

    public sealed class ParameterOptions
    {
        /// <summary>
        /// The parameter description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// In case of an array type - the values separator.
        /// Unless specified, the default is a comma (",")
        /// </summary>
        public string Separator { get; set; }
    }

    internal class GlobalParameterHandler
    {
        internal IEnumerable<string> Names { get; set; }
        internal Action<string> Handler { get; set; }
        internal string Description { get; set; }
        internal string Separator { get; set; }
        internal Type Type { get; set; }
    }
}