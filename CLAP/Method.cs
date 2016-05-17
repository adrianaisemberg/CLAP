using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace CLAP
{
    /// <summary>
    /// A method descriptor
    /// </summary>
    [DebuggerDisplay("{MethodInfo}")]
    public sealed class Method
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

        /// <summary>
        /// Whether this verb is the default verb of the class
        /// </summary>
        public bool IsDefault { get; set; }

        #endregion Properties

        #region Constructors

        internal Method(MethodInfo method)
        {
            Debug.Assert(method != null);

            Names = new List<string>();

            // Names are stored as lower-case.
            // The first available name is the method's original name.
            //
            Names.Add(method.Name);

            MethodInfo = method;

            var verbAttribute = method.GetAttribute<VerbAttribute>();

            Description = verbAttribute.Description;
            IsDefault = verbAttribute.IsDefault;

            if (verbAttribute.Aliases != null)
            {
                Names.AddRange(verbAttribute.Aliases.CommaSplit());
            }
        }

        #endregion Constructors
    }
}