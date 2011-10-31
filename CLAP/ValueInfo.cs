using System;
using System.Diagnostics;

namespace CLAP
{
    /// <summary>
    /// Information about a vluae
    /// </summary>
    [DebuggerDisplay("{Name}:{Value} [{Type}]")]
    public class ValueInfo
    {
        internal ValueInfo(string name, Type type, object value)
        {
            Name = name;
            Type = type;
            Value = value;
        }

        /// <summary>
        /// The name of the value. Either the parameter or property name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The type of the value
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// The value
        /// </summary>
        public object Value { get; private set; }
    }
}