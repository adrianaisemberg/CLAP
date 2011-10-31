using System;

namespace CLAP.Interception
{
    /// <summary>
    /// Marks a class to allow verb-interception by a defined IVerbInterceptor
    /// (or either IPreVerbInterceptor or IPostVerbInterceptor) type
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class VerbInterception : Attribute
    {
        /// <summary>
        /// The interceptor type
        /// </summary>
        public Type InterceptorType { get; private set; }

        /// <summary>
        /// Marks a class to allow verb-interception by a defined IVerbInterceptor
        /// (or either IPreVerbInterceptor or IPostVerbInterceptor) type
        /// </summary>
        /// <param name="interceptorType">An interceptor type that implements either IVerbInterceptor, IPreVerbInterceptor or IPostVerbInterceptor</param>
        public VerbInterception(Type interceptorType)
        {
            InterceptorType = interceptorType;
        }
    }
}