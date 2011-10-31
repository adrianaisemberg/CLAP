using System;

namespace CLAP
{
    /// <summary>
    /// Marks a method as a global parameter.
    /// The method can either accept any single allowed parameter type or accept 
    /// no parameters and be treated as a boolean switch.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class GlobalAttribute : Attribute
    {
        /// <summary>
        /// The additional aliases (as CSV) of the parameter
        /// </summary>
        public string Aliases { get; set; }

        /// <summary>
        /// The description of this parameter
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The name of this parameter
        /// </summary>
        public string Name { get; private set; }

        public GlobalAttribute()
            : this(null)
        {

        }

        public GlobalAttribute(string name)
        {
            Aliases = string.Empty;

            Name = name;
        }
    }
}