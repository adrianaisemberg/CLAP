using System;
using System.Reflection;

namespace CLAP.Validation
{
    /// <summary>
    /// More-Than validation
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Parameter)]
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
            public override void Validate(ParameterInfo parameter, object value)
            {
                var doubleValue = (double)Convert.ChangeType(value, typeof(double));

                if (doubleValue <= Number)
                {
                    throw new ValidationException("{0}: {1} is not more than {2}".FormatWith(
                        parameter.Name,
                        value,
                        Number));
                }
            }
        }
    }
}