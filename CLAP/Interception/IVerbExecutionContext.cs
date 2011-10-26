
namespace CLAP.Interception
{
    public interface IVerbExecutionContext
    {
        Method Method { get; }
        object Target { get; }
        ArgumentsCollection Arguments { get; }
    }
}