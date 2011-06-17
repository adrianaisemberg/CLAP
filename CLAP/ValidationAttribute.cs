using System;

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
        public IParameterValidator Validator { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="validator">The validator</param>
        protected ValidationAttribute(IParameterValidator validator)
        {
            Validator = validator;
        }

        /// <summary>
        /// The description of this validation attribute, used when asking for help
        /// </summary>
        public virtual string Description
        {
            get { return GetType().Name; }
        }
    }

    /// <summary>
    /// A value validator
    /// </summary>
    public interface IParameterValidator
    {
        /// <summary>
        /// Validate the value
        /// </summary>
        void Validate(object value);
    }
}