using System;
using System.Collections.Generic;

namespace CLAP.Interception
{
    /// <summary>
    /// The context after a verb was executed
    /// </summary>
    public sealed class PostVerbExecutionContext : VerbExecutionContext
    {
        #region Properties

        /// <summary>
        /// Whether the verb execution was cancelled by the pre-execution interception
        /// </summary>
        public bool Cancelled { get; private set; }

        /// <summary>
        /// If the verb failed to execute - this contains the exception that was thrown
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Whether the verb failed to execute
        /// </summary>
        public bool Failed
        {
            get { return Exception != null; }
        }

        #endregion Properties

        #region Constructors

        internal PostVerbExecutionContext(
            Method method,
            object target,
            ParameterAndValue[] parameters,
            bool cancelled,
            Exception exception,
            Dictionary<object, object> userContext)
            : base(method, target, parameters, userContext)
        {
            Cancelled = cancelled;
            Exception = exception;
        }

        #endregion Constructors
    }
}