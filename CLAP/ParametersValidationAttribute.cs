using System;
using System.Reflection;

namespace CLAP
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public abstract class ParametersValidationAttribute : Attribute
    {
        public abstract IParametersValidator GetValidator();

        /// <summary>
        /// The description of this validation attribute, used when asking for help
        /// </summary>
        public abstract string Description { get; }
    }

    public interface IParametersValidator
    {
        void Validate(ParameterInfoAndValue[] parameters);
    }

    public class ParameterInfoAndValue
    {
        public ParameterInfoAndValue(ParameterInfo parameter, object value)
        {
            Parameter = parameter;
            Value = value;
        }

        public ParameterInfo Parameter { get; private set; }
        public object Value { get; private set; }
    }
}