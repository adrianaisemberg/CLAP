using System;
using System.Collections.Generic;
using System.Reflection;

namespace CLAP
{
    /// <summary>
    /// A method descriptor
    /// </summary>
    internal sealed class Method
    {
        #region Properties

        /// <summary>
        /// The names of the method, as defined by the Verb attribute and the additional names
        /// </summary>
        public List<string> Names { get; private set; }

        /// <summary>
        /// The description of the method
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// The <see cref="MethodInfo"/> this method describes
        /// </summary>
        public MethodInfo MethodInfo { get; private set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="method">The <see cref="MethodInfo"/> to describe</param>
        public Method(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            Names = new List<string>();

            // Names are stored as lower-case.
            // The first available name is the method's original name.
            //
            Names.Add(method.Name.ToLowerInvariant());

            MethodInfo = method;

            var verbAttribute = method.GetAttribute<VerbAttribute>();

            Description = verbAttribute.Description;

            if (verbAttribute.Aliases != null)
            {
                Names.AddRange(
                    verbAttribute.Aliases.ToLowerInvariant().CommaSplit());
            }
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Overrides ToString
        /// </summary>
        public override string ToString()
        {
            return MethodInfo.ToString();
        }

        #endregion Methods
    }
}