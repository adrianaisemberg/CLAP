using System.Collections.Generic;

namespace CLAP.Interception
{
    public abstract class VerbExecutionContext
    {
        public Method Method { get; private set; }
        public object Target { get; private set; }
        public ParameterAndValue[] Parameters { get; private set; }
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