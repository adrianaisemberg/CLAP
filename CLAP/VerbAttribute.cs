using System;

namespace CLAP
{
    /// <summary>
    /// A verb
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class VerbAttribute : Attribute
    {
        /// <summary>
        /// Additional names for the verb
        /// </summary>
        public string Aliases { get; set; }

        /// <summary>
        /// The description of the verb. Used to generate the help string
        /// </summary>
        public string Description { get; set; }
    }
}