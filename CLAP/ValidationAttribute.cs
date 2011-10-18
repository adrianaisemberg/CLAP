using System;
using System.Reflection;

namespace CLAP
{
    /// <summary>
    /// Validation on the parameter's value
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
    public abstract class ValidationAttribute : Attribute
    {
        /// <summary>
        /// The validator
        /// </summary>
        public abstract IParameterValidator GetValidator();

        /// <summary>
        /// The description of this validation attribute, used when asking for help
        /// </summary>
        public abstract string Description { get; }
    }

    /// <summary>
    /// A value validator
    /// </summary>
    public interface IParameterValidator
    {
        /// <summary>
        /// Validate the value
        /// </summary>
        void Validate(ParameterInfo parameter, object value);
    }
}