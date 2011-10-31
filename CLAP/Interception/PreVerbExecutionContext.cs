using System.Collections.Generic;

namespace CLAP.Interception
{
    /// <summary>
    /// The context before a verb is to be executed
    /// </summary>
    public sealed class PreVerbExecutionContext : VerbExecutionContext
    {
        #region Properties

        /// <summary>
        /// Whether to cancel to verb execution.
        /// The post-interception will be called having the Cancelled property set to true.
        /// </summary>
        public bool Cancel { get; set; }

        #endregion Properties

        #region Constructors

        internal PreVerbExecutionContext(
            Method method,
            object target,
            ParameterAndValue[] parameters)
            : base(method, target, parameters, new Dictionary<object, object>())
        {
        }

        #endregion Constructors
    }
}