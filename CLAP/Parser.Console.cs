using System.Diagnostics;
using System.Linq;

namespace CLAP
{
    public partial class Parser
    {
        /// <summary>
        /// Executes a console parser of instance-verbs based on the specified targets
        /// </summary>
        /// <param name="args">The user arguments</param>
        /// <param name="targets">The instances of the verb classes</param>
        public static int RunConsole(string[] args, params object[] targets)
        {
            Debug.Assert(targets.Any());
            Debug.Assert(targets.All(t => t != null));

            var p = new Parser(targets.Select(t => t.GetType()).ToArray()).Console();

            return ((MultiParser)p).RunTargets(args, targets);
        }

        /// <summary>
        /// Executes a generic console static parser of a specified type
        /// </summary>
        /// <typeparam name="T">The type of the parser</typeparam>
        /// <param name="args">The user arguments</param>
        public static int RunConsole<T>(string[] args)
        { return new Parser<T>().Console().RunStatic(args); }

        /// <summary>
        /// Executes a generic console parser of a specified type
        /// </summary>
        /// <typeparam name="T">The type of the parser</typeparam>
        /// <param name="args">The user arguments</param>
        /// <param name="t">An instance of the verb class</param>
        public static int RunConsole<T>(string[] args, T t)
        { return new Parser<T>().Console().RunTargets(args, t); }

        /// <summary>
        /// Executes a generic console static parser of some specified types
        /// </summary>
        /// <param name="args">The user arguments</param>
        public static int RunConsole<T1, T2>(string[] args)
        { return new Parser<T1, T2>().Console().RunStatic(args); }

        /// <summary>
        /// Executes a generic console parser of some specified types
        /// </summary>
        /// <typeparam name="T1">The type of the parser</typeparam>
        /// <typeparam name="T2">The type of the parser</typeparam>
        /// <param name="args">The user arguments</param>
        /// <param name="t1">An instance of the verb class</param>
        /// <param name="t2">An instance of the verb class</param>
        public static int RunConsole<T1, T2>(string[] args, T1 t1, T2 t2)
        { return new Parser<T1, T2>().Console().RunTargets(args, t1, t2); }

        /// <summary>
        /// Executes a generic console static parser of some specified types
        /// </summary>
        /// <param name="args">The user arguments</param>
        public static int RunConsole<T1, T2, T3>(string[] args)
        { return new Parser<T1, T2, T3>().Console().RunStatic(args); }

        /// <summary>
        /// Executes a generic console parser of some specified types
        /// </summary>
        /// <typeparam name="T1">The type of the parser</typeparam>
        /// <typeparam name="T2">The type of the parser</typeparam>
        /// <typeparam name="T3">The type of the parser</typeparam>
        /// <param name="args">The user arguments</param>
        /// <param name="t1">An instance of the verb class</param>
        /// <param name="t2">An instance of the verb class</param>
        /// <param name="t3">An instance of the verb class</param>
        public static int RunConsole<T1, T2, T3>(string[] args, T1 t1, T2 t2, T3 t3)
        { return new Parser<T1, T2, T3>().Console().RunTargets(args, t1, t2, t3); }

        /// <summary>
        /// Executes a generic console static parser of some specified types
        /// </summary>
        /// <param name="args">The user arguments</param>
        public static int RunConsole<T1, T2, T3, T4>(string[] args)
        { return new Parser<T1, T2, T3, T4>().Console().RunStatic(args); }

        /// <summary>
        /// Executes a generic console parser of some specified types
        /// </summary>
        /// <typeparam name="T1">The type of the parser</typeparam>
        /// <typeparam name="T2">The type of the parser</typeparam>
        /// <typeparam name="T3">The type of the parser</typeparam>
        /// <typeparam name="T4">The type of the parser</typeparam>
        /// <param name="args">The user arguments</param>
        /// <param name="t1">An instance of the verb class</param>
        /// <param name="t2">An instance of the verb class</param>
        /// <param name="t3">An instance of the verb class</param>
        /// <param name="t4">An instance of the verb class</param>
        public static int RunConsole<T1, T2, T3, T4>(string[] args, T1 t1, T2 t2, T3 t3, T4 t4)
        { return new Parser<T1, T2, T3, T4>().Console().RunTargets(args, t1, t2, t3, t4); }

        /// <summary>
        /// Executes a generic console static parser of some specified types
        /// </summary>
        /// <param name="args">The user arguments</param>
        public static int RunConsole<T1, T2, T3, T4, T5>(string[] args)
        { return new Parser<T1, T2, T3, T4, T5>().Console().RunStatic(args); }

        /// <summary>
        /// Executes a generic console parser of some specified types
        /// </summary>
        /// <typeparam name="T1">The type of the parser</typeparam>
        /// <typeparam name="T2">The type of the parser</typeparam>
        /// <typeparam name="T3">The type of the parser</typeparam>
        /// <typeparam name="T4">The type of the parser</typeparam>
        /// <typeparam name="T5">The type of the parser</typeparam>
        /// <param name="args">The user arguments</param>
        /// <param name="t1">An instance of the verb class</param>
        /// <param name="t2">An instance of the verb class</param>
        /// <param name="t3">An instance of the verb class</param>
        /// <param name="t4">An instance of the verb class</param>
        /// <param name="t5">An instance of the verb class</param>
        public static int RunConsole<T1, T2, T3, T4, T5>(string[] args, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
        { return new Parser<T1, T2, T3, T4, T5>().Console().RunTargets(args, t1, t2, t3, t4, t5); }

        /// <summary>
        /// Executes a generic console static parser of some specified types
        /// </summary>
        /// <param name="args">The user arguments</param>
        public static int RunConsole<T1, T2, T3, T4, T5, T6>(string[] args)
        { return new Parser<T1, T2, T3, T4, T5, T6>().Console().RunStatic(args); }

        /// <summary>
        /// Executes a generic console parser of some specified types
        /// </summary>
        /// <typeparam name="T1">The type of the parser</typeparam>
        /// <typeparam name="T2">The type of the parser</typeparam>
        /// <typeparam name="T3">The type of the parser</typeparam>
        /// <typeparam name="T4">The type of the parser</typeparam>
        /// <typeparam name="T5">The type of the parser</typeparam>
        /// <typeparam name="T6">The type of the parser</typeparam>
        /// <param name="args">The user arguments</param>
        /// <param name="t1">An instance of the verb class</param>
        /// <param name="t2">An instance of the verb class</param>
        /// <param name="t3">An instance of the verb class</param>
        /// <param name="t4">An instance of the verb class</param>
        /// <param name="t5">An instance of the verb class</param>
        /// <param name="t6">An instance of the verb class</param>
        public static int RunConsole<T1, T2, T3, T4, T5, T6>(string[] args, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6)
        { return new Parser<T1, T2, T3, T4, T5, T6>().Console().RunTargets(args, t1, t2, t3, t4, t5, t6); }

        /// <summary>
        /// Executes a generic console static parser of some specified types
        /// </summary>
        /// <param name="args">The user arguments</param>
        public static int RunConsole<T1, T2, T3, T4, T5, T6, T7>(string[] args)
        { return new Parser<T1, T2, T3, T4, T5, T6, T7>().Console().RunStatic(args); }

        /// <summary>
        /// Executes a generic console parser of some specified types
        /// </summary>
        /// <typeparam name="T1">The type of the parser</typeparam>
        /// <typeparam name="T2">The type of the parser</typeparam>
        /// <typeparam name="T3">The type of the parser</typeparam>
        /// <typeparam name="T4">The type of the parser</typeparam>
        /// <typeparam name="T5">The type of the parser</typeparam>
        /// <typeparam name="T6">The type of the parser</typeparam>
        /// <typeparam name="T7">The type of the parser</typeparam>
        /// <param name="args">The user arguments</param>
        /// <param name="t1">An instance of the verb class</param>
        /// <param name="t2">An instance of the verb class</param>
        /// <param name="t3">An instance of the verb class</param>
        /// <param name="t4">An instance of the verb class</param>
        /// <param name="t5">An instance of the verb class</param>
        /// <param name="t6">An instance of the verb class</param>
        /// <param name="t7">An instance of the verb class</param>
        public static int RunConsole<T1, T2, T3, T4, T5, T6, T7>(string[] args, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7)
        { return new Parser<T1, T2, T3, T4, T5, T6, T7>().Console().RunTargets(args, t1, t2, t3, t4, t5, t6, t7); }

        /// <summary>
        /// Executes a generic console static parser of some specified types
        /// </summary>
        /// <param name="args">The user arguments</param>
        public static int RunConsole<T1, T2, T3, T4, T5, T6, T7, T8>(string[] args)
        { return new Parser<T1, T2, T3, T4, T5, T6, T7, T8>().Console().RunStatic(args); }

        /// <summary>
        /// Executes a generic console parser of some specified types
        /// </summary>
        /// <typeparam name="T1">The type of the parser</typeparam>
        /// <typeparam name="T2">The type of the parser</typeparam>
        /// <typeparam name="T3">The type of the parser</typeparam>
        /// <typeparam name="T4">The type of the parser</typeparam>
        /// <typeparam name="T5">The type of the parser</typeparam>
        /// <typeparam name="T6">The type of the parser</typeparam>
        /// <typeparam name="T7">The type of the parser</typeparam>
        /// <typeparam name="T8">The type of the parser</typeparam>
        /// <param name="args">The user arguments</param>
        /// <param name="t1">An instance of the verb class</param>
        /// <param name="t2">An instance of the verb class</param>
        /// <param name="t3">An instance of the verb class</param>
        /// <param name="t4">An instance of the verb class</param>
        /// <param name="t5">An instance of the verb class</param>
        /// <param name="t6">An instance of the verb class</param>
        /// <param name="t7">An instance of the verb class</param>
        /// <param name="t8">An instance of the verb class</param>
        public static int RunConsole<T1, T2, T3, T4, T5, T6, T7, T8>(string[] args, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8)
        { return new Parser<T1, T2, T3, T4, T5, T6, T7, T8>().Console().RunTargets(args, t1, t2, t3, t4, t5, t6, t7, t8); }

        /// <summary>
        /// Executes a generic console static parser of some specified types
        /// </summary>
        /// <param name="args">The user arguments</param>
        public static int RunConsole<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string[] args)
        { return new Parser<T1, T2, T3, T4, T5, T6, T7, T8, T9>().Console().RunStatic(args); }

        /// <summary>
        /// Executes a generic console parser of some specified types
        /// </summary>
        /// <typeparam name="T1">The type of the parser</typeparam>
        /// <typeparam name="T2">The type of the parser</typeparam>
        /// <typeparam name="T3">The type of the parser</typeparam>
        /// <typeparam name="T4">The type of the parser</typeparam>
        /// <typeparam name="T5">The type of the parser</typeparam>
        /// <typeparam name="T6">The type of the parser</typeparam>
        /// <typeparam name="T7">The type of the parser</typeparam>
        /// <typeparam name="T8">The type of the parser</typeparam>
        /// <typeparam name="T9">The type of the parser</typeparam>
        /// <param name="args">The user arguments</param>
        /// <param name="t1">An instance of the verb class</param>
        /// <param name="t2">An instance of the verb class</param>
        /// <param name="t3">An instance of the verb class</param>
        /// <param name="t4">An instance of the verb class</param>
        /// <param name="t5">An instance of the verb class</param>
        /// <param name="t6">An instance of the verb class</param>
        /// <param name="t7">An instance of the verb class</param>
        /// <param name="t8">An instance of the verb class</param>
        /// <param name="t9">An instance of the verb class</param>
        public static int RunConsole<T1, T2, T3, T4, T5, T6, T7, T8, T9>(string[] args, T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7, T8 t8, T9 t9)
        { return new Parser<T1, T2, T3, T4, T5, T6, T7, T8, T9>().Console().RunTargets(args, t1, t2, t3, t4, t5, t6, t7, t8, t9); }
    }
}