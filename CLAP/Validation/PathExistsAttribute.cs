using System;
using System.IO;

namespace CLAP.Validation
{
    /// <summary>
    /// Path exists validation (file or directory)
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class PathExistsAttribute : ValidationAttribute
    {
        public PathExistsAttribute()
        {

        }

        public override IValueValidator GetValidator()
        {
            return new PathExistsValidator();
        }

        public override string Description
        {
            get
            {
                return "Path exists (file or directory)";
            }
        }

        private class PathExistsValidator : IValueValidator
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

                if (!(File.Exists(path) || Directory.Exists(path)))
                {
                    throw new ValidationException("{0}: '{1}' is not an existing file or directory".FormatWith(
                        info.Name,
                        path));
                }
            }
        }
    }
}