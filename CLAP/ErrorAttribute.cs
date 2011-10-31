using System;

namespace CLAP
{
    /// <summary>
    /// Marks a method to be executed when an exception occurs.
    /// The method may accept no parameters or only a single System.Exception
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ErrorAttribute : Attribute
    {
#warning TODO: change to return a bool instead
        /// <summary>
        /// Whether to rethrow the execption after handling it
        /// </summary>
        public bool ReThrow { get; set; }
    }
}