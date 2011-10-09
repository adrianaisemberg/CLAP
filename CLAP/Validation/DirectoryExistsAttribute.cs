using System;
using System.IO;
using System.Reflection;

namespace CLAP.Validation
{
    /// <summary>
    /// Directory exists validation
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class DirectoryExistsAttribute : ValidationAttribute
    {
        public DirectoryExistsAttribute()
            : base(new DirectoryExistsValidator())
        {

        }

        public override string Description
        {
            get
            {
                return "Directory exists";
            }
        }

        private class DirectoryExistsValidator : IParameterValidator
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

                if (!Directory.Exists(path))
                {
                    throw new ValidationException("{0}: '{1}' is not an existing directory".FormatWith(
                        parameter.Name,
                        path));
                }
            }
        }
    }
}