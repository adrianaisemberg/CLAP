using System;

namespace CLAP.Validation
{
    [Serializable]
    public abstract class NumberValidationAttribute : ValidationAttribute
    {
        protected double Number { get; private set; }

        protected NumberValidationAttribute(NumberValidator validator)
            : base(validator)
        {
            Number = validator.Number;
        }
    }

    /// <summary>
    /// Number validation
    /// </summary>
    public abstract class NumberValidator : IParameterValidator
    {
        /// <summary>
        /// The number to validate with
        /// </summary>
        public double Number { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        protected NumberValidator(double number)
        {
            Number = number;
        }

        /// <summary>
        /// Validate
        /// </summary>
        /// <param name="value"></param>
        public abstract void Validate(object value);
    }
}