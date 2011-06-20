using System;

namespace CLAP
{
    /// <summary>
    /// The default verb, in case the user doesn't specify one
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DefaultVerbAttribute : Attribute
    {
        /// <summary>
        /// The verb
        /// </summary>
        public string Verb { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="verb">The verb</param>
        public DefaultVerbAttribute(string verb)
        {
            Verb = verb;
        }
    }
}