CLAP: A Kick-Ass .NET Command-Line Parser
=========================================
Basic example: One verb, Some parameters
----------------------------------------
```c#
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
    public static void Print(string text, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Console.WriteLine(text);
        }
    }
}
```

<pre>
>myexe print -text:"Hello World" -count:10
Hello World
Hello World
Hello World
Hello World
Hello World
Hello World
Hello World
Hello World
Hello World
Hello World
</pre>

Complex sample: Various verbs, global handlers, help handler, validation, default values, switches, arrays
----------------------------------------------------------------------------------------------------------
```c#
class MyApp
{
    [Global(Aliases = "d", Description = "Launch a debugger")]
    public static void Debug()
    {
        // this is a global parameter handler.
        // it works for any verb.

        Debugger.Launch();
    }

    [Empty, Help]
    public static void Help(string help)
    {
        // this is an empty handler that prints
        // the automatic help string to the console.

        Console.WriteLine(help);
    }

    [Verb]
    public static void Print(string text, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Console.WriteLine(text);
        }
    }

    [Verb]
    public static void Foo(
        [Parameter(Aliases = "t", Description = "A string parameter with an additional alias")]
        string text,

        [Parameter(Default = 5, Description = "An int parameter with a default")]
        int number,

        [MoreThan(10)]
        [LessThan(100)]
        [Parameter(Default = 42.3, Description = "A double parameter with validation and a default value")]
        double percent,

        [Parameter(Description = "A bool parameter, which can be used as a switch")]
        bool verbose,

        [Parameter(Description = "An enum parameter")]
        MyEnum option,

        [Parameter(Description = "An array of strings")]
        string[] array)
    {
        // text = "hello"
        // number = 88
        // percent = 12.446
        // verbose = true
        // option = MyEnumValue2
        // array = { "a", "b", "c", "d" }
    }
}
```

<pre>
>myexe foo -t:hello -number:88 -percent:12.446 -verbose -option:MyEnumValue2 -array:a,b,c,d
</pre>