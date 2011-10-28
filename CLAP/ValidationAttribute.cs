using System;

namespace CLAP
{
    /// <summary>
    /// Validation on the parameter's value
    /// </summary>
    [Serializable]
    public abstract class ValidationAttribute : Attribute
    {
        /// <summary>
        /// The validator
        /// </summary>
        public abstract IValueValidator GetValidator();

        /// <summary>
        /// The description of this validation attribute, used when asking for help
        /// </summary>
        public abstract string Description { get; }
    }

    /// <summary>
    /// A value validator
    /// </summary>
    public interface IValueValidator
    {
        /// <summary>
        /// Validate the value
        /// </summary>
        void Validate(ValueInfo info);
    }
}