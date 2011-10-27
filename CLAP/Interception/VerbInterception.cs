using System;

namespace CLAP.Interception
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class VerbInterception : Attribute
    {
        public Type InterceptorType { get; private set; }

        public VerbInterception(Type interceptorType)
        {
            InterceptorType = interceptorType;
        }
    }
}