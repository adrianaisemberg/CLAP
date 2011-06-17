using System;

namespace CLAP
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class HelpAttribute : Attribute
    {
        public string Aliases { get; set; }
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