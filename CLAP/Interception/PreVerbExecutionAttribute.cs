using System;

namespace CLAP.Interception
{
    /// <summary>
    /// Marks a method to run before each verb is executed
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class PreVerbExecutionAttribute : Attribute
    {
    }
}