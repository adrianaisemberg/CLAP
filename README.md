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
        OptionEnum option,

        [Parameter(Description = "An array of strings")]
        string[] array)
    {
        Console.WriteLine("text = {0}", text);
        Console.WriteLine("number = {0}", number);
        Console.WriteLine("percent = {0}", percent);
        Console.WriteLine("verbose = {0}", verbose);
        Console.WriteLine("option = {0}", option);
        Console.WriteLine("array = [{0}]", string.Join(",", array));
    }
}
```

<pre>
>myexe foo -t:hello -number:88 -percent:12.446 -verbose -option:Option2 -array:a,b,c,d

text = hello
number = 88
percent = 12.446
verbose = True
option = Option2
array = [a,b,c,d]
</pre>

**No arguments, help is printed:**
<pre>
>myexe

Print:
 -text:  [String]
 -count:  [Int32]

Foo:
 -text/t: A string parameter with an additional alias [String]
 -number: An int parameter with a default [Int32, Default = 5]
 -percent: A double parameter with validation and a default value [Double, Default = 42.3, More than 10, Less than 100]
 -verbose: A bool parameter, which can be used as a switch [Boolean]
 -option: An enum parameter [OptionEnum]
 -array: An array of strings [String[]]

Global parameters:
 -Debug/d: Launch a debugger [Boolean]
</pre>