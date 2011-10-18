using System;
using System.IO;
using System.Reflection;

namespace CLAP.Validation
{
    /// <summary>
    /// Path exists validation (file or directory)
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class PathExistsAttribute : ValidationAttribute
    {
        public PathExistsAttribute()
        {

        }

        public override IParameterValidator GetValidator()
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

        private class PathExistsValidator : IParameterValidator
        {
            public void Validate(ParameterInfo parameter, object value)
            {
                string path = string.Empty;

                if (value is Uri)
                {
                    path = ((Uri)value).LocalPath;
                }
                else
                {
                    path = Environment.ExpandEnvironmentVariables((string)value);
                }

                if (!(File.Exists(path) || Directory.Exists(path)))
                {
                    throw new ValidationException("{0}: '{1}' is not an existing file or directory".FormatWith(
                        parameter.Name,
                        path));
                }
            }
        }
    }
}