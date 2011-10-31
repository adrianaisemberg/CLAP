using System;
using System.Text.RegularExpressions;

namespace CLAP.Validation
{
    /// <summary>
    /// Regex validation:
    /// The string value of the marked parameter or property must match the specified regular expression
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class RegexMatchesAttribute : ValidationAttribute
    {
        public string Pattern { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pattern"></param>
        public RegexMatchesAttribute(string pattern)
        {
            Pattern = pattern;
        }

        public override IValueValidator GetValidator()
        {
            return new RegexMatchesValidator(Pattern);
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
        private class RegexMatchesValidator : IValueValidator
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
            public void Validate(ValueInfo info)
            {
                if (!Regex.IsMatch(info.Value.ToSafeString(string.Empty), Pattern))
                {
                    throw new ValidationException("{0}: '{1}' does not match regex '{2}'".FormatWith(
                        info.Name,
                        info.Value,
                        Pattern));
                }
            }
        }
    }
}