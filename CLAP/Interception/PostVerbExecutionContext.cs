using System;
using System.Collections.Generic;

namespace CLAP.Interception
{
    public sealed class PostVerbExecutionContext : VerbExecutionContext, IVerbExecutionContext
    {
        #region Properties

        public bool Cancelled { get; private set; }
        public Exception Exception { get; private set; }

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