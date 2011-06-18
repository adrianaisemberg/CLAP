CLAP: A Kick-Ass .NET Command-Line Parser
=========================================

Documentation by samples
------------------------
Basic: One verb. Some parameters.

    class Program
    {
        public static void Main(string[] args)
        {
            Parser<MyApp>.Run(args);
        }
    }

    class MyApp
    {
        [Verb]
        public static void Print(string name, int count)
        {
            //
        }
    }

<pre>
>myexe print -name:adrian -count:10
</pre>