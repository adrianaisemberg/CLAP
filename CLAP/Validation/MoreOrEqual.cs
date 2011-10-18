using System;
using System.Reflection;

namespace CLAP.Validation
{
    /// <summary>
    /// More Or Equal-To validation
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class MoreOrEqualToAttribute : NumberValidationAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MoreOrEqualToAttribute(double number)
            : base(number)
        {
        }

        public override IParameterValidator GetValidator()
        {
            return new MoreOrEqualToValidator(Number);
        }

        public override string Description
        {
            get
            {
                return "More or equal to {0}".FormatWith(Number);
            }
        }

        /// <summary>
        /// More Or Equal-To validator
        /// </summary>
        private class MoreOrEqualToValidator : NumberValidator
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="number"></param>
            public MoreOrEqualToValidator(double number)
                : base(number)
            {
            }

            /// <summary>
            /// Validate
            /// </summary>
            public override void Validate(ParameterInfo parameter, object value)
            {
                var doubleValue = (double)Convert.ChangeType(value, typeof(double));

                if (doubleValue < Number)
                {
                    throw new ValidationException("{0}: {1} is not more or equal to {2}".FormatWith(
                        parameter.Name,
                        value,
                        Number));
                }
            }
        }
    }
}