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

