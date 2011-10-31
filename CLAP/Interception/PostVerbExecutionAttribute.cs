using System;

namespace CLAP.Interception
{
    /// <summary>
    /// Marks a method to run after each verb is executed
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class PostVerbExecutionAttribute : Attribute
    {
    }
}