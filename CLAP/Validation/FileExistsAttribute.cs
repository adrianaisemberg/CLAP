using System;
using System.IO;

namespace CLAP.Validation
{
    /// <summary>
    /// File exists validation
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class FileExistsAttribute : ValidationAttribute
    {
        public FileExistsAttribute()
        {

        }

        public override IValueValidator GetValidator()
        {
            return new FileExistsValidator();
        }

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