using System;
using System.Diagnostics;

namespace CLAP
{
    public interface IValidation
    {
        IInfoValidator GetValidator();

        /// <summary>
        /// The description of this validation attribute, used when asking for help
        /// </summary>
        string Description { get; }
    }

    public interface IInfoValidator
    {
        void Validate(ValueInfo[] properties);
    }

    [DebuggerDisplay("{Name}:{Value} [{Type}]")]
    public class ValueInfo
    {
        public ValueInfo(string name, Type type, object value)
        {
            Name = name;
            Type = type;
            Value = value;
        }

        public string Name { get; private set; }
        public Type Type { get; private set; }
        public object Value { get; private set; }
    }
}