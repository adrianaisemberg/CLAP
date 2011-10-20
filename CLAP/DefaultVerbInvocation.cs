using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace CLAP
{
    internal sealed class DefaultVerbInvocation : IVerbInvocation
    {
        public string Verb { get; private set; }
        public Method VerbMethod { get; private set; }
        public ReadOnlyCollection<object> VerbParameters { get; private set; }
        public object TargetObject { get; private set; }

        public DefaultVerbInvocation(string verb, Method verbMethod, List<object> parms, object target)
        {
            Debug.Assert(verb != null);
            Debug.Assert(verbMethod != null);
            var verbParms = parms ?? new List<object>();

            Verb = verb;
            VerbMethod = verbMethod;
            VerbParameters = verbParms.AsReadOnly();
            TargetObject = target;
        }

        public void Proceed()
        {
            Debug.Assert(VerbMethod.MethodInfo != null);
            VerbMethod.MethodInfo.Invoke(TargetObject, VerbParameters.ToArray());
        }
    }
}
