using System;

namespace CLAP.Validation
{
    /// <summary>
    /// More-Than validation:
    /// The numeric value of the marked parameter or property must be a more than the specified number
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class MoreThanAttribute : NumberValidationAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="number"></param>
        public MoreThanAttribute(double number)
            : base(number)
        {
        }

        public override IValueValidator GetValidator()
        {
            return new MoreThanValidator(Number);
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
            public override void Validate(ValueInfo info)
            {
                var doubleValue = (double)Convert.ChangeType(info.Value, typeof(double));

                if (doubleValue <= Number)
                {
                    throw new ValidationException("{0}: {1} is not more than {2}".FormatWith(
                        info.Name,
                        info.Value,
                        Number));
                }
            }
        }
    }
}