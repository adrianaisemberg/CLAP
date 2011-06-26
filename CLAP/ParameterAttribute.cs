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