using System.Collections.Generic;

namespace CLAP
{
    public interface IVerbInvocation
    {
        string Verb { get; set; }
        Method VerbMethod { get; set; }
        List<object> VerbParameters { get; set; }
        object TargetObject { get; set; }
        Dictionary<string, string> InputArgs { get; set; }
        void Proceed();
    }


    public class DefaultVerbInvocation : IVerbInvocation
    {
        public string Verb { get; set; }
        public Method VerbMethod { get; set; }
        public List<object> VerbParameters { get; set; }
        public object TargetObject { get; set; }
        public Dictionary<string, string> InputArgs { get; set; }

        public void Proceed()
        {
            VerbMethod.MethodInfo.Invoke(TargetObject, VerbParameters.ToArray());
        }
    }
}
