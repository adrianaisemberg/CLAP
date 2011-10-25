using System;

namespace CLAP
{
    public abstract class AbstractParser
    {
        /// <summary>
        /// Invoke a verb on an object
        /// </summary>
        public virtual void Run(string[] args, object obj) { }

        /// <summary>
        /// Invoke a static verb
        /// </summary>
        public virtual void Run(string[] args) { }

        /// <summary>
        /// Registers a parameter name to handle the help string
        /// </summary>
        public virtual void RegisterHelpHandler(string names, Action<string> helpHandler) { }

        /// <summary>
        /// Registers an empty help handler
        /// </summary>
        public virtual void RegisterEmptyHelpHandler(Action<string> handler) { }

        /// <summary>
        /// Registers an empty handler
        /// </summary>
        public virtual void RegisterEmptyHandler(Action handler) { }

        /// <summary>
        /// Registers an error handler
        /// </summary>
        public virtual void RegisterErrorHandler(Action<Exception> handler) { }

        /// <summary>
        /// Registers an error handler
        /// </summary>
        public virtual void RegisterErrorHandler(Action<Exception> handler, bool rethrow) { }

        /// <summary>
        /// Registers an action to a global parameter name
        /// </summary>
        public virtual void RegisterParameterHandler(string names, Action action) { }

        /// <summary>
        /// Registers an action to a global parameter name
        /// </summary>
        public virtual void RegisterParameterHandler(string names, Action action, string description) { }

        /// <summary>
        /// Registers an action to a global parameter name
        /// </summary>
        public virtual void RegisterParameterHandler<TParameter>(string names, Action<TParameter> action) { }

        /// <summary>
        /// Registers an action to a global parameter name
        /// </summary>
        public virtual void RegisterParameterHandler<TParameter>(string names, Action<TParameter> action, string description) { }

        /// <summary>
        /// Registers a verb interceptor
        /// </summary>
        public virtual void RegisterInterceptor(Action<IVerbInvocation> interceptor) { }
    }
}
