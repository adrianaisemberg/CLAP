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
