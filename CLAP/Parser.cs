using System;
using System.Diagnostics;
using System.Linq;

namespace CLAP
{
    /// <summary>
    /// A command-line arguments parser
    /// </summary>
    public class Parser : MultiParser
    {
        public Parser(params Type[] types)
            : base(types)
        {
        }

        public static void Run(string[] args, params object[] targets)
        {
            Debug.Assert(targets.Any());
            Debug.Assert(targets.All(t => t != null));

            var p = new Parser(targets.Select(t => t.GetType()).ToArray());

            ((MultiParser)p).RunTargets(args, targets);
        }

        public static void Run<T>(string[] args)
        { new Parser<T>().RunStatic(args); }
        public static void Run<T>(string[] args, T t)
        { new Parser<T>().RunTargets(args, t); }

        public static void Run<T1, T2>(string[] args)
        { new Parser<T1, T2>().RunStatic(args); }
        public static void Run<T1, T2>(string[] args, T1 t1, T2 t2)
        { new Parser<T1, T2>().RunTargets(args, t1, t2); }

        public static void Run<T1, T2, T3>(string[] args)
        { new Parser<T1, T2, T3>().RunStatic(args); }
        public static void Run<T1, T2, T3>(string[] args, T1 t1, T2 t2, T3 t3)
        { new Parser<T1, T2, T3>().RunTargets(args, t1, t2, t3); }

        public static void Run<T1, T2, T3, T4>(string[] args)
        { new Parser<T1, T2, T3, T4>().RunStatic(args); }
        public static void Run<T1, T2, T3, T4>(string[] args, T1 t1, T2 t2, T3 t3, T4 t4)
        { new Parser<T1, T2, T3, T4>().RunTargets(args, t1, t2, t3, t4); }

        public static void Run<T1, T2, T3, T4, T5>(string[] args)
        { new Parser<T1, T2, T3, T4, T5>().RunStatic(args); }
        public static void Run<T1, T2, T3, T4, T5>(string[] args, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        { new Parser<T1, T2, T3, T4, T5>().RunTargets(args, t1, t2, t3, t4, t5); }

        public static void Run<T1, T2, T3, T4, T5, T6>(string[] args)
        { new Parser<T1, T2, T3, T4, T5, T6>().RunStatic(args); }
        public static void Run<T1, T2, T3, T4, T5, T6>(string[] args, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        { new Parser<T1, T2, T3, T4, T5, T6>().RunTargets(args, t1, t2, t3, t4, t5, t6); }

        public static void Run<T1, T2, T3, T4, T5, T6, T7>(string[] args)
        { new Parser<T1, T2, T3, T4, T5, T6, T7>().RunStatic(args); }
        public static void Run<T1, T2, T3, T4, T5, T6, T7>(string[] args, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        { new Parser<T1, T2, T3, T4, T5, T6, T7>().RunTargets(args, t1, t2, t3, t4, t5, t6, t7); }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8>(string[] args)
        { new Parser<T1, T2, T3, T4, T5, T6, T7, T8>().RunStatic(args); }
        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8>(string[] args, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        { new Parser<T1, T2, T3, T4, T5, T6, T7, T8>().RunTargets(args, t1, t2, t3, t4, t5, t6, t7, t8); }

        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string[] args)
        { new Parser<T1, T2, T3, T4, T5, T6, T7, T8, T9>().RunStatic(args); }
        public static void Run<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string[] args, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9)
        { new Parser<T1, T2, T3, T4, T5, T6, T7, T8, T9>().RunTargets(args, t1, t2, t3, t4, t5, t6, t7, t8, t9); }
    }

    public class Parser<T> : MultiParser
    { public void Run(string[] args, T t) { base.RunTargets(args, t); } }

    public class Parser<T1, T2> : MultiParser
    { public void Run(string[] args, T1 t1, T2 t2) { base.RunTargets(args, t1, t2); } }

    public class Parser<T1, T2, T3> : MultiParser
    { public void Run(string[] args, T1 t1, T2 t2, T3 t3) { base.RunTargets(args, t1, t2, t3); } }

    public class Parser<T1, T2, T3, T4> : MultiParser
    { public void Run(string[] args, T1 t1, T2 t2, T3 t3, T4 t4) { base.RunTargets(args, t1, t2, t3, t4); } }

    public class Parser<T1, T2, T3, T4, T5> : MultiParser
    { public void Run(string[] args, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5) { base.RunTargets(args, t1, t2, t3, t4, t5); } }

    public class Parser<T1, T2, T3, T4, T5, T6> : MultiParser
    { public void Run(string[] args, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6) { base.RunTargets(args, t1, t2, t3, t4, t5, t6); } }

    public class Parser<T1, T2, T3, T4, T5, T6, T7> : MultiParser
    { public void Run(string[] args, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7) { base.RunTargets(args, t1, t2, t3, t4, t5, t6, t7); } }

    public class Parser<T1, T2, T3, T4, T5, T6, T7, T8> : MultiParser
    { public void Run(string[] args, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8) { base.RunTargets(args, t1, t2, t3, t4, t5, t6, t7, t8); } }

    public class Parser<T1, T2, T3, T4, T5, T6, T7, T8, T9> : MultiParser
    { public void Run(string[] args, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9) { base.RunTargets(args, t1, t2, t3, t4, t5, t6, t7, t8, t9); } }
}