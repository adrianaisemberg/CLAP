using System;

namespace CLAP
{
    /// <summary>
    /// Marks a method to be executed when an exception occurs.
    /// The method may only accept one parameter of type CLAP.ExceptionContext
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ErrorAttribute : Attribute
    {
    }
}