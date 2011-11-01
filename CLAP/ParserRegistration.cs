using System;
using System.Collections.Generic;
using CLAP.Interception;

namespace CLAP
{
    /// <summary>
    /// Provides registration features for parser instances
    /// </summary>
    public sealed class ParserRegistration
    {
        #region Properties

        internal Dictionary<string, GlobalParameterHandler> RegisteredGlobalHandlers { get; private set; }
        internal Dictionary<string, Action<string>> RegisteredHelpHandlers { get; private set; }
        internal Action RegisteredEmptyHandler { get; private set; }
        internal Action<ExceptionContext> RegisteredErrorHandler { get; private set; }
        internal Action<PreVerbExecutionContext> RegisteredPreVerbInterceptor { get; private set; }
        internal Action<PostVerbExecutionContext> RegisteredPostVerbInterceptor { get; private set; }

        internal Func<string> HelpGetter { get; private set; }
        internal Func<Type, string, string, object> ParameterValueGetter { get; private set; }

        #endregion Properties

        #region Constructors

        internal ParserRegistration(Func<string> helpGetter, Func<Type, string, string, object> parameterValueGetter)
        {
            RegisteredGlobalHandlers = new Dictionary<string, GlobalParameterHandler>();
            RegisteredHelpHandlers = new Dictionary<string, Action<string>>();

            HelpGetter = helpGetter;
            ParameterValueGetter = parameterValueGetter;
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
            RegisterHelpHandlerInternal(names, helpHandler);
        }

        /// <summary>
        /// Registers an empty help handler that is executed when there is no input
        /// </summary>
        /// <param name="handler">The action to be executed</param>
        public void EmptyHelpHandler(Action<string> handler)
        {
            if (RegisteredEmptyHandler != null)
            {
                throw new MoreThanOneEmptyHandlerException();
            }

            var help = HelpGetter();

            RegisteredEmptyHandler = delegate
            {
                handler(help);
            };
        }

        /// <summary>
        /// Registers an empty handler that is executed when there is no input
        /// </summary>
        /// <param name="handler">The action to be executed</param>
        public void EmptyHandler(Action handler)
        {
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
            ParameterHandler(
                names,
                new Action<bool>(delegate { action(); }),
                null);
        }

        /// <summary>
        /// Registers a global parameter handler
        /// </summary>
        /// <param name="names">The names (CSV) to be registered as boolean parameters (switches)</param>
        /// <param name="action">The action to execute</param>
        /// <param name="description">The parameter description (for help generation)</param>
        public void ParameterHandler(string names, Action action, string description)
        {
            ParameterHandler(
                names,
                new Action<bool>(delegate { action(); }),
                description);
        }

        /// <summary>
        /// Registers a global parameter handler
        /// </summary>
        /// <typeparam name="TParameter">The type of the parameter</typeparam>
        /// <param name="names">The names (CSV) to be registered as parameters</param>
        /// <param name="action">The action to execute</param>
        public void ParameterHandler<TParameter>(string names, Action<TParameter> action)
        {
            ParameterHandler(
                names,
                action,
                null);
        }

        /// <summary>
        /// Registers a global parameter handler
        /// </summary>
        /// <typeparam name="TParameter">The type of the parameter</typeparam>
        /// <param name="names">The names (CSV) to be registered as parameters</param>
        /// <param name="action">The action to execute</param>
        /// <param name="description">The parameter description (for help generation)</param>
        public void ParameterHandler<TParameter>(string names, Action<TParameter> action, string description)
        {
            RegisterParameterHandlerInternal(
                names,
                action,
                description);
        }

        #endregion Public Methods

        #region Private Methods

        private void RegisterParameterHandlerInternal<TParameter>(string names, Action<TParameter> action, string description)
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
                    v = ParameterValueGetter(typeof(TParameter), string.Empty, str);
                }

                action((TParameter)v);
            });

            RegisteredGlobalHandlers.Add(
                names,
                new GlobalParameterHandler
                {
                    Names = names.CommaSplit(),
                    Handler = objectAction,
                    Desription = description,
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

    internal class GlobalParameterHandler
    {
        internal IEnumerable<string> Names { get; set; }
        internal Action<string> Handler { get; set; }
        internal string Desription { get; set; }
        internal Type Type { get; set; }
    }
}