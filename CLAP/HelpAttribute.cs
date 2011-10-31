using System;

namespace CLAP
{
    /// <summary>
    /// Marks a method to be called when the user asks for help
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class HelpAttribute : Attribute
    {
        /// <summary>
        /// The additional aliases (as CSV) of the parameter
        /// </summary>
        public string Aliases { get; set; }

        /// <summary>
        /// The name of this parameter
        /// </summary>
        public string Name { get; private set; }

        public HelpAttribute()
            : this(null)
        {

        }

        public HelpAttribute(string name)
        {
            Aliases = string.Empty;

            Name = name;
        }
    }
}