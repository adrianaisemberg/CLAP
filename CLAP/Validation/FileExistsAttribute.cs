using System;
using System.IO;
using System.Reflection;

namespace CLAP.Validation
{
    /// <summary>
    /// File exists validation
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class FileExistsAttribute : ValidationAttribute
    {
        public FileExistsAttribute()
        {

        }

        public override IParameterValidator GetValidator()
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

        private class FileExistsValidator : IParameterValidator
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

                if (!File.Exists(path))
                {
                    throw new ValidationException("{0}: '{1}' is not an existing file".FormatWith(
                        parameter.Name,
                        path));
                }
            }
        }
    }
}