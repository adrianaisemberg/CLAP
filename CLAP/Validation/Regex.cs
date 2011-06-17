using System;
using System.Text.RegularExpressions;

namespace CLAP.Validation
{
    /// <summary>
    /// Regex validation
    /// </summary>
    [Serializable]
    public sealed class RegexMatchesAttribute : ValidationAttribute
    {
        public string Pattern { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pattern"></param>
        public RegexMatchesAttribute(string pattern)
            : base(new RegexMatchesValidator(pattern))
        {
            Pattern = pattern;
        }

        public override string Description
        {
            get
            {
                return "Matches regex: '{0}'".FormatWith(Pattern);
            }
        }

        /// <summary>
        /// Regex validator
        /// </summary>
        private class RegexMatchesValidator : IParameterValidator
        {
            /// <summary>
            /// The regex pattern
            /// </summary>
            public string Pattern { get; private set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="pattern"></param>
            public RegexMatchesValidator(string pattern)
            {
                Pattern = pattern;
            }

            /// <summary>
            /// Validate
            /// </summary>
            /// <param name="value"></param>
            public void Validate(object value)
            {
                if (!Regex.IsMatch(value.ToString(), Pattern))
                {
                    throw new ValidationException("'{0}' does not match regex '{1}'".FormatWith(value, Pattern));
                }
            }
        }
    }
}