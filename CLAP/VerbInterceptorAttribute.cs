using System;

namespace CLAP
{
    /// <summary>
    /// Attribute for identifying an interceptor method.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class VerbInterceptorAttribute : Attribute
    {
    }
}
