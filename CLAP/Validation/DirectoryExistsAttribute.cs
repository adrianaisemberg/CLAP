using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CLAP.Validation
{
    /// <summary>
    /// Directory exists validation
    /// </summary>
    [Serializable]
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
            public void Validate(object value)
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
                    throw new ValidationException("'{0}' is not an existing directory".FormatWith(path));
                }
            }
        }
    }
}