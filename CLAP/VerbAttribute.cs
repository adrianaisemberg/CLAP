using System;

namespace CLAP
{
    /// <summary>
    /// Marks a method as a verb
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class VerbAttribute : Attribute
    {
        /// <summary>
        /// Additional names for the verb
        /// </summary>
        public string Aliases { get; set; }

        /// <summary>
        /// The description of the verb. Used to generate the help string
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Whether this verb is the default verb of the class
        /// </summary>
        public bool IsDefault { get; set; }
    }

    [Serializable]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class TargetAliasAttribute : Attribute
    {
        public TargetAliasAttribute(string alias)
        {
            Alias = alias;
        }

        public string Alias { get; private set; }
    }
}