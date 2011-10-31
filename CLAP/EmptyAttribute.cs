using System;

namespace CLAP
{
    /// <summary>
    /// Marks a method to be executed when there is no input.
    /// The method must not accept any parameter except if marked along with [Help].
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class EmptyAttribute : Attribute
    {
    }
}