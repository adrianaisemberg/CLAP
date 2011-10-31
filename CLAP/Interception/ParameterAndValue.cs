
namespace CLAP.Interception
{
    /// <summary>
    /// A parameter and its value
    /// </summary>
    public sealed class ParameterAndValue
    {
        /// <summary>
        /// The parameter
        /// </summary>
        public Parameter Parameter { get; private set; }

        /// <summary>
        /// The value of the parameter
        /// </summary>
        public object Value { get; private set; }

        internal ParameterAndValue(Parameter parameter, object value)
        {
            Parameter = parameter;
            Value = value;
        }
    }
}