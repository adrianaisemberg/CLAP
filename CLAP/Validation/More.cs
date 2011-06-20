using System;

namespace CLAP.Validation
{
    /// <summary>
    /// More-Than validation
    /// </summary>
    [Serializable]
    public sealed class MoreThanAttribute : NumberValidationAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="number"></param>
        public MoreThanAttribute(double number)
            : base(new MoreThanValidator(number))
        {
        }

        public override string Description
        {
            get
            {
                return "More than {0}".FormatWith(Number);
            }
        }

        /// <summary>
        /// More-Than validator
        /// </summary>
        private class MoreThanValidator : NumberValidator
        {
            /// <summary>
            /// Constructor
            /// </summary>
            public MoreThanValidator(double number)
                : base(number)
            {
            }

            /// <summary>
            /// Validate
            /// </summary>
            public override void Validate(object value)
            {
                var doubleValue = (double)Convert.ChangeType(value, typeof(double));

                if (doubleValue <= Number)
                {
                    throw new ValidationException("{0} is not more than {1}".FormatWith(value, Number));
                }
            }
        }
    }
}