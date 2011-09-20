using System;

namespace CLAP
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ErrorAttribute : Attribute
    {
        public bool ReThrow { get; set; }
    }
}