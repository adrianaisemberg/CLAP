using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CLAP.Validation
{
    /// <summary>
    /// File exists validation
    /// </summary>
    [Serializable]
    public sealed class FileExistsAttribute : ValidationAttribute
    {
        public FileExistsAttribute()
            : base(new FileExistsValidator())
        {

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

                if (!File.Exists(path))
                {
                    throw new ValidationException("'{0}' is not an existing file".FormatWith(path));
                }
            }
        }
    }
}