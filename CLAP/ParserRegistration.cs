using System;
using System.Collections.Generic;
using CLAP.Interception;

namespace CLAP
{
    public sealed class ParserRegistration
    {
        #region Fields

        private readonly Func<string> m_helpGetter;
        private readonly Func<Type, string, string, object> m_parameterValueGetter;

        #endregion Fields

        #region Properties

        internal Dictionary<string, GlobalParameterHandler> RegisteredGlobalHandlers { get; private set; }

#warning TODO: only one help handler
        internal Dictionary<string, Action<string>> RegisteredHelpHandlers { get; private set; }
        internal Action RegisteredEmptyHandler { get; private set; }
        internal Action<Exception> RegisteredErrorHandler { get; private set; }
        internal Action<PreVerbExecutionContext> RegisteredPreVerbInterceptor { get; private set; }
        internal Action<PostVerbExecutionContext> RegisteredPostVerbInterceptor { get; private set; }
        internal bool RegisteredErrorHandlerRethrow { get; private set; }

        #endregion Properties

        #region Constructors

        public ParserRegistration(Func<string> helpGetter, Func<Type, string, string, object> parameterValueGetter)
        {
            m_helpGetter = helpGetter;
            m_parameterValueGetter = parameterValueGetter;

            RegisteredGlobalHandlers = new Dictionary<string, GlobalParameterHandler>();
            RegisteredHelpHandlers = new Dictionary<string, Action<string>>();
        }

        #endregion Constructors

        #region Public Methods

        public void HelpHandler(string names, Action<string> helpHandler)
        {
            RegisterHelpHandlerInternal(names, helpHandler);
        }

        public void EmptyHelpHandler(Action<string> handler)
        {
            if (RegisteredEmptyHandler != null)
            {
                throw new MoreThanOneEmptyHandlerException();
            }

            var help = m_helpGetter();

            RegisteredEmptyHandler = delegate
            {
                handler(help);
            };
        }

        public void EmptyHandler(Action handler)
        {
            if (RegisteredEmptyHandler != null)
            {
                throw new MoreThanOneEmptyHandlerException();
            }

            RegisteredEmptyHandler = handler;
        }

        public void ErrorHandler(Action<Exception> handler)
        {
            ErrorHandler(handler, false);
        }

        public void ErrorHandler(Action<Exception> handler, bool rethrow)
        {
            if (RegisteredErrorHandler != null)
            {
                throw new MoreThanOneErrorHandlerException();
            }

            RegisteredErrorHandler = handler;
            RegisteredErrorHandlerRethrow = rethrow;
        }

        public void PreVerbInterceptor(Action<PreVerbExecutionContext> interceptor)
        {
            if (RegisteredPreVerbInterceptor != null)
            {
                throw new MoreThanOnePreVerbInterceptorException();
            }

            RegisteredPreVerbInterceptor = interceptor;
        }

        public void PostVerbInterceptor(Action<PostVerbExecutionContext> interceptor)
        {
            if (RegisteredPostVerbInterceptor != null)
            {
                throw new MoreThanOnePostVerbInterceptorException();
            }

            RegisteredPostVerbInterceptor = interceptor;
        }

        public void ParameterHandler(string names, Action action)
        {
            ParameterHandler(
                names,
                new Action<bool>(delegate { action(); }),
                null);
        }

        public void ParameterHandler(string names, Action action, string description)
        {
            ParameterHandler(
                names,
                new Action<bool>(delegate { action(); }),
                description);
        }

        public void ParameterHandler<TParameter>(string names, Action<TParameter> action)
        {
            ParameterHandler(
                names,
                action,
                null);
        }

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
                    v = m_parameterValueGetter(typeof(TParameter), string.Empty, str);
                }

                action((TParameter)v);
            });

            foreach (var name in names)
            {
                RegisteredGlobalHandlers.Add(
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
        internal string Name { get; set; }
        internal Action<string> Handler { get; set; }
        internal string Desription { get; set; }
        internal Type Type { get; set; }
    }
}