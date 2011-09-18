using System;
using System.Reflection;

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
        public LessThanAttribute(double number)
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
            public LessThanValidator(double number)
                : base(number)
            {
            }

            /// <summary>
            /// Validate
            /// </summary>
            public override void Validate(ParameterInfo parameter, object value)
            {
                var doubleValue = (double)Convert.ChangeType(value, typeof(double));

                if (doubleValue >= Number)
                {
                    throw new ValidationException("{0}: {1} is not less than {2}".FormatWith(
                        parameter.Name,
                        value,
                        Number));
                }
            }
        }
    }
}