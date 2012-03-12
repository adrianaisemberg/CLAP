using System.Collections.Generic;

namespace CLAP
{
    /// <summary>
    /// A verb execution context
    /// </summary>
    public class VerbExecutionContext
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
        /// The input arguments
        /// </summary>
        public Dictionary<string, string> Input { get; set; }

        internal VerbExecutionContext(
            Method method,
            object target,
            Dictionary<string, string> input)
        {
            Method = method;
            Target = target;
            Input = input;
        }
    }
}