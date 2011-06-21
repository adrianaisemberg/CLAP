//
// Since downgrading from .NET 4.0 to 2.0,
// Action and Funct delegates are no longer available.
// These are good enough.
//

namespace CLAP
{
    public delegate void Action();

    public delegate TResult Func<T, TResult>(T arg);
}