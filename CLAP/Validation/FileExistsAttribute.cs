using System;
using System.IO;

namespace CLAP.Validation
{
    /// <summary>
    /// File exists validation:
    /// The string value of the marked parameter or property must be a path to an existing file
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class FileExistsAttribute : ValidationAttribute
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public FileExistsAttribute()
        {

        }

        /// <summary>
        /// Gets a validator instance
        /// </summary>
        public override IValueValidator GetValidator()
        {
            return new FileExistsValidator();
        }

        /// <summary>
        /// The validation description
        /// </summary>
        public override string Description
        {
            get
            {
                return "File exists";
            }
        }

        private class FileExistsValidator : IValueValidator
        {
            public void Validate(ValueInfo info)
            {
                string path = string.Empty;

                if (info.Value is Uri)
                {
                    path = ((Uri)info.Value).LocalPath;
                }
                else
                {
                    path = Environment.ExpandEnvironmentVariables((string)info.Value);
                }

                if (!File.Exists(path))
                {
                    throw new ValidationException("{0}: '{1}' is not an existing file".FormatWith(
                        info.Name,
                        path));
                }
            }
        }
    }
}