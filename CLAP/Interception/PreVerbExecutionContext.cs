using System.Collections.Generic;

namespace CLAP.Interception
{
    public sealed class PreVerbExecutionContext : IVerbExecutionContext
    {
        #region Properties

        public Method Method { get; private set; }
        public object Target { get; private set; }
        public ArgumentsCollection Arguments { get; private set; }

        public bool Cancel { get; set; }

        public Dictionary<object, object> UserContext { get; set; }

        #endregion Properties

        #region Constructors

        internal PreVerbExecutionContext(
            Method method,
            object target,
            ArgumentsCollection arguments)
        {
            Method = method;
            Target = target;
            Arguments = arguments;

            UserContext = new Dictionary<object, object>();
        }

        #endregion Constructors
    }
}