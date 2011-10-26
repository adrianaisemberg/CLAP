using System;

namespace CLAP.Interception
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class PreVerbExecutionAttribute : Attribute
    {
    }
}