using System;

namespace CLAP
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class GlobalAttribute : Attribute
    {
        public string Aliases { get; set; }
        public string Description { get; set; }
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