using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace CLAP
{
    /// <summary>
    /// A parameter descriptor
    /// </summary>
    [DebuggerDisplay("{ParameterInfo}")]
    public sealed class Parameter
    {
        #region Properties

        /// <summary>
        /// The default value
        /// </summary>
        public object Default { get; private set; }

        /// <summary>
        /// The default value provider
        /// </summary>
        public DefaultProvider DefaultProvider { get; private set; }

        /// <summary>
        /// Whether this parameter is required
        /// </summary>
        public Boolean Required { get; private set; }

        /// <summary>
        /// The names of the parameter, as defined by the Parameter attribute and the additional names
        /// </summary>
        public List<string> Names { get; private set; }

        /// <summary>
        /// The parameter description
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// The <see cref="ParameterInfo"/> this parameter describes
        /// </summary>
        public ParameterInfo ParameterInfo { get; private set; }

        #endregion Properties

        #region Constructors

        internal Parameter(ParameterInfo parameter)
        {
            Debug.Assert(parameter != null);

            Names = new List<string>();

            // Names are stored as lower-case.
            // The first available name is the parameters's original name.
            //
            Names.Add(parameter.Name.ToLowerInvariant());

            ParameterInfo = parameter;


            var parameterAttribute = parameter.GetAttribute<ParameterAttribute>();

            // a parameter doesn't have to be marked with [ParameterAttribute]
            //
            if (parameterAttribute != null)
            {
                Default = parameterAttribute.Default;

                if (parameterAttribute.DefaultProvider != null)
                {
                    DefaultProvider = (DefaultProvider)Activator.CreateInstance(parameterAttribute.DefaultProvider);
                }

                Required = parameterAttribute.Required;
                Description = parameterAttribute.Description;

                if (parameterAttribute.Aliases != null)
                {
                    Names.AddRange(
                        parameterAttribute.Aliases.ToLowerInvariant().CommaSplit());
                }
            }
        }

        #endregion Constructors
    }
}