using System;

namespace CLAP
{
    /// <summary>
    /// A parameter
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class ParameterAttribute : Attribute
    {
        /// <summary>
        /// The default value
        /// </summary>
        public object Default { get; set; }

        /// <summary>
        /// The default provider type
        /// </summary>
        /// <remarks>
        /// The type must derive from CLAP.DefaultProvider.
        /// A parameter cannot have both a Default and a DefaultProvider defined.
        /// </remarks>
        public Type DefaultProvider { get; set; }

        /// <summary>
        /// Whether this parameter is required
        /// </summary>
        public Boolean Required { get; set; }

        /// <summary>
        /// The parameter additional names
        /// </summary>
        public string Aliases { get; set; }

        /// <summary>
        /// The description of the verb. Used to generate the help string
        /// </summary>
        public string Description { get; set; }
    }
}