using System.Collections.Generic;

namespace CLAP.Interception
{
    public sealed class PostVerbExecutionContext : IVerbExecutionContext
    {
        #region Properties

        public Method Method { get; private set; }
        public object Target { get; private set; }
        public ArgumentsCollection Arguments { get; private set; }

        public bool Cancelled { get; private set; }

        public Dictionary<object, object> UserContext { get; private set; }

        #endregion Properties

        #region Constructors

        internal PostVerbExecutionContext(
            Method method,
            object target,
            ArgumentsCollection arguments,
            bool cancelled,
            Dictionary<object, object> userContext)
        {
            Method = method;
            Target = target;
            Arguments = arguments;
            Cancelled = cancelled;
            UserContext = userContext;
        }

        #endregion Constructors
    }
}