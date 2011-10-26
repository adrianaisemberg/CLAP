
namespace CLAP.Interception
{
    public interface IVerbExecutionContext
    {
        Method Method { get; }
        object Target { get; }
        ParameterAndValue[] Parameters { get; }
    }

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