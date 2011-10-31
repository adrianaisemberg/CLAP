using System.Collections.Generic;

namespace CLAP.Interception
{
    /// <summary>
    /// A verb execution context
    /// </summary>
    public abstract class VerbExecutionContext
    {
        /// <summary>
        /// The method that is executed
        /// </summary>
        public Method Method { get; private set; }

        /// <summary>
        /// The target object, if any, that the verb is executed on.
        /// If the verb is static, this is null.
        /// </summary>
        public object Target { get; private set; }

        /// <summary>
        /// The list of parameters and their values
        /// </summary>
        public ParameterAndValue[] Parameters { get; private set; }

        /// <summary>
        /// A user-context that can be filled with custom keys and values.
        /// Once filled in the pre-execution context - it is available in the post-execution context.
        /// </summary>
        public Dictionary<object, object> UserContext { get; private set; }

        protected VerbExecutionContext(
            Method method,
            object target,
            ParameterAndValue[] parameters,
            Dictionary<object, object> userContext)
        {
            Method = method;
            Target = target;
            Parameters = parameters;
            UserContext = userContext;
        }
    }
}