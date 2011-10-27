
namespace CLAP.Interception
{
    public sealed class ParameterAndValue
    {
        public Parameter Parameter { get; private set; }
        public object Value { get; private set; }

        public ParameterAndValue(Parameter parameter, object value)
        {
            Parameter = parameter;
            Value = value;
        }
    }
}