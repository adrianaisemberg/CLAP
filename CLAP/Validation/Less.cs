using System;

namespace CLAP.Validation
{
    /// <summary>
    /// Less-Than validation
    /// </summary>
    [Serializable]
    public sealed class LessThanAttribute : NumberValidationAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public LessThanAttribute(int number)
            : base(new LessThanValidator(number))
        {
        }

        public override string Description
        {
            get
            {
                return "Less than {0}".FormatWith(Number);
            }
        }

        /// <summary>
        /// Less-Than validator
        /// </summary>
        private class LessThanValidator : NumberValidator
        {
            /// <summary>
            /// Constructor
            /// </summary>
            public LessThanValidator(int number)
                : base(number)
            {
            }

            /// <summary>
            /// Validate
            /// </summary>
            public override void Validate(object value)
            {
                if ((int)value >= Number)
                {
                    throw new ValidationException("{0} is not less than {1}".FormatWith(value, Number));
                }
            }
        }
    }
}