
namespace CLAP
{
    internal class Pair<TFirst, TSecond>
    {
        public TFirst First { get; private set; }
        public TSecond Second { get; private set; }

        public Pair(TFirst first, TSecond second)
        {
            First = first;
            Second = second;
        }
    }

    internal static class Pair
    {
        internal static Pair<TFirst, TSecond> Create<TFirst, TSecond>(TFirst first, TSecond second)
        {
            return new Pair<TFirst, TSecond>(first, second);
        }
    }
}