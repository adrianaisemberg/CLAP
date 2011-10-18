using System;
using System.Reflection;

namespace CLAP.Validation
{
    /// <summary>
    /// Less Or Equal-To validation
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class LessOrEqualToAttribute : NumberValidationAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public LessOrEqualToAttribute(double number)
            : base(number)
        {
        }

        public override IParameterValidator GetValidator()
        {
            return new LessOrEqualToValidator(Number);
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
            public override void Validate(ParameterInfo parameter, object value)
            {
                var doubleValue = (double)Convert.ChangeType(value, typeof(double));

                if (doubleValue > Number)
                {
                    throw new ValidationException("{0}: {1} is not less or equal to {2}".FormatWith(
                        parameter.Name,
                        value,
                        Number));
                }
            }
        }
    }
}