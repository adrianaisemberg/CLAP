using System;
using System.Collections.Generic;
using CLAP.Interception;

namespace CLAP
{
    public sealed class ParserRegistration
    {
        #region Properties

        internal Dictionary<string, GlobalParameterHandler> RegisteredGlobalHandlers { get; private set; }
        internal Dictionary<string, Action<string>> RegisteredHelpHandlers { get; private set; }
        internal Action RegisteredEmptyHandler { get; private set; }
        internal Func<Exception, bool> RegisteredErrorHandler { get; private set; }
        internal Action<PreVerbExecutionContext> RegisteredPreVerbInterceptor { get; private set; }
        internal Action<PostVerbExecutionContext> RegisteredPostVerbInterceptor { get; private set; }

        internal Func<string> HelpGetter { get; private set; }
        internal Func<Type, string, string, object> ParameterValueGetter { get; private set; }

        #endregion Properties

        #region Constructors

        public ParserRegistration(Func<string> helpGetter, Func<Type, string, string, object> parameterValueGetter)
        {
            RegisteredGlobalHandlers = new Dictionary<string, GlobalParameterHandler>();
            RegisteredHelpHandlers = new Dictionary<string, Action<string>>();

            HelpGetter = helpGetter;
            ParameterValueGetter = parameterValueGetter;
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

            var help = HelpGetter();

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
            ErrorHandler(new Func<Exception, bool>(ex =>
            {
                handler(ex);

                return false;
            }));
        }

        public void ErrorHandler(Func<Exception, bool> handler)
        {
            if (RegisteredErrorHandler != null)
            {
                throw new MoreThanOneErrorHandlerException();
            }

            RegisteredErrorHandler = handler;
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
                    v = ParameterValueGetter(typeof(TParameter), string.Empty, str);
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