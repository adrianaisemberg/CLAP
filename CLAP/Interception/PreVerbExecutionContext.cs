using System.Collections.Generic;

namespace CLAP.Interception
{
    public sealed class PreVerbExecutionContext : VerbExecutionContext, IVerbExecutionContext
    {
        #region Properties

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