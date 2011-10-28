
namespace CLAP
{
    /// <summary>
    /// A command-line arguments parser
    /// </summary>
    public static class Parser
    {
        public static void Run<T>(string[] args, T t)
        {
            var p = new Parser<T>();

            p.Run(args, t);
        }

        public static void Run<T>(string[] args)
        {
            var p = new Parser<T>();

            p.Run(args);
        }
    }

    public class Parser<T> : MultiParser
    {
    }
}