using System;

namespace CLAP.Validation
{
    /// <summary>
    /// Less Or Equal-To validation
    /// </summary>
    [Serializable]
    public sealed class LessOrEqualToAttribute : NumberValidationAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public LessOrEqualToAttribute(double number)
            : base(new LessOrEqualToValidator(number))
        {
        }

        public override string Description
        {
            get
            {
                return "Less or equal to {0}".FormatWith(Number);
            }
        }

        /// <summary>
        /// Less Or Equal-To validator
        /// </summary>
        private class LessOrEqualToValidator : NumberValidator
        {
            /// <summary>
            /// Constructor
            /// </summary>
            public LessOrEqualToValidator(double number)
                : base(number)
            {
            }

            /// <summary>
            /// Validate
            /// </summary>
            public override void Validate(object value)
            {
                var doubleValue = (double)Convert.ChangeType(value, typeof(double));

                if (doubleValue > Number)
                {
                    throw new ValidationException("{0} is not less or equal to {1}".FormatWith(value, Number));
                }
            }
        }
    }
}