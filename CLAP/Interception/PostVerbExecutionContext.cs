using System;
using System.Collections.Generic;

namespace CLAP.Interception
{
    public sealed class PostVerbExecutionContext : IVerbExecutionContext
    {
        #region Properties

        public Method Method { get; private set; }
        public object Target { get; private set; }
        public ParameterAndValue[] Parameters { get; private set; }

        public bool Cancelled { get; private set; }
        public Exception Exception { get; private set; }

        public bool Failed
        {
            get { return Exception != null; }
        }

        public Dictionary<object, object> UserContext { get; private set; }

        #endregion Properties

        #region Constructors

        internal PostVerbExecutionContext(
            Method method,
            object target,
            ParameterAndValue[] parameters,
            bool cancelled,
            Exception ex,
            Dictionary<object, object> userContext)
        {
            Method = method;
            Target = target;
            Parameters = parameters;
            Cancelled = cancelled;
            Exception = ex;
            UserContext = userContext;
        }

        #endregion Constructors
    }
}