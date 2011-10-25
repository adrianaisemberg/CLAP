using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using CLAP;
using CLAP.Validation;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Run<ClapApp>(args);
        }
    }

    class ClapApp
    {
        [Verb]
        public static void Foo(string bar, int count)
        {
            for (int i = 0; i < count; i++) Console.WriteLine("This parser {0}", bar);
        }

        [Verb]
        [SameLength]
        public static void Bar(int[] numbers, string[] names)
        {
            for (int i = 0; i < numbers.Length; i++)
            {
                Console.WriteLine("{0}:{1}", numbers[i], names[i]);
            }
        }

        [Verb]
        [Validate("num1 + num2 >= num3")]
        public static void Zoo(int num1, int num2, int num3)
        {
        }

        [Help, Empty]
        public static void Help(string h)
        {
            Console.WriteLine(h);
        }

        [Global]
        [Validate("num > 100")]
        public static void Pong(int num)
        {
        }

        [Global(Aliases = "d")]
        public static void Debug()
        {
            Debugger.Launch();
        }

        [Verb(Aliases = "save")]
        public static void SavePerson(Person p)
        {
        }

        [Verb]
        public static void SavePersons(IEnumerable<Person> p)
        {
        }
    }

    class BaseApp
    {
        [Error]
        public static void Error(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.ResetColor();
        }

        [Empty]
        [Help(Aliases = "h,?")]
        public static void Help(string help)
        {
            Console.WriteLine(help);
        }

        [Global]
        public static void Debug()
        {
            Debugger.Launch();
        }
    }

    class TheApp : BaseApp
    {
        [Verb(IsDefault = true, Description = "Prints 'Hello' and a name")]
        public static void Hello(

            [Parameter(
                Aliases = "n",
                Required = true,
                Description = "The name")]
            string name,

            [LessOrEqualTo(30)]
            [Parameter(
                Aliases = "c",
                Default = 10,
                Description = "The number of lines to print")]
            int count)
        {
            for (int i = 0; i < count; i++)
            {
                Console.WriteLine("Hello {0}", name);
            }
        }

        [Verb]
        public static void Count(
            [LessOrEqualTo(50), MoreOrEqualTo(0)] int start,
            [LessOrEqualTo(100), MoreThan(50)] int end,
            [Parameter(Aliases = "p,pause", Default = 1.5)] double pauseSeconds,
            [Parameter(Aliases = "align")] Alignment alignment)
        {
            var sign = alignment == Alignment.Left ? "-" : "";

            var format = "{0," + sign + "5}";

            for (int i = start; i <= end; i++)
            {
                Console.WriteLine(format, i);

                Thread.Sleep((int)(pauseSeconds * 1000));
            }
        }

        public enum Alignment
        {
            Right,
            Left,
        }
    }

    class TheNewApp : BaseApp
    {
        [Verb(IsDefault = true)]
        public static void Foo([MoreThan(10)]int count)
        {
        }
    }

    class AnotherApp
    {
        [Verb(IsDefault = true)]
        public static void Test(string x, int y)
        {
        }

        [Empty]
        public static void Empty()
        {
        }
    }

    class MyAppp
    {
        [Empty, Help]
        public static void Help(string h)
        {
            Console.WriteLine(h);
        }

        [Verb]
        public static void Foo(string n)
        {
            Console.WriteLine(n);
        }

        [Verb]
        public void Bar(string m)
        {
            Console.WriteLine(m);
        }
    }

    class MyApp
    {
        [Global(Aliases = "d", Description = "Launch a debugger")]
        public static void Debug()
        {
            // this is a global parameter handler.
            // it works for any verb.

            Debugger.Launch();
        }

        [Error]
        public static void Error(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(ex.Message);

            Console.ResetColor();
        }

        [Empty, Help]
        public static void Help(string help)
        {
            // this is an empty handler that prints
            // the automatic help string to the console.

            Console.WriteLine(help);
        }

        [Verb]
        public static void Print(Uri address, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Console.WriteLine(address);
            }
        }

        [Verb(Aliases = "f,fo,fooo")]
        public static void Foo(
            [Parameter(Aliases = "t", Description = "A string parameter with an additional alias")]
            string text,

            [Parameter(Default = 5, Description = "An int parameter with a default")]
            int number,

            [Parameter(Default = "http://www.com")]
            Uri address,

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

    enum OptionEnum
    {
        Option1,
        Option2,
    }

    //class MyApp
    //{
    //    [Empty]
    //    public static void Foo()
    //    {
    //    }
    //}

    //class MyApp
    //{
    //    [Help]
    //    public static void Help(string help)
    //    {
    //        Console.WriteLine(help);
    //    }

    //    [Verb]
    //    public static void Foo(string bar)
    //    {
    //    }
    //}

    //class MyApp
    //{
    //    [Help("h")]
    //    public static void Help(string help)
    //    {
    //        Console.WriteLine(help);
    //    }
    //}

    //class MyApp
    //{
    //    [Help(Aliases = "h,?")]
    //    public static void Help(string help)
    //    {
    //        Console.WriteLine(help);
    //    }
    //}


    //p.RegisterParameterHandler(
    //"debug,d,break",
    //delegate { Debugger.Launch(); },
    //"Attach a debugger to the process");

    //        p.RegisterHelpHandler("help,?", help => Console.WriteLine(help));

    //        p.RegisterParameterHandler<string>(
    //            "thread",
    //            name => Thread.CurrentThread.Name = name);

    //        p.RegisterParameterHandler<ThreadPriority>(
    //            "priority",
    //            priority => Thread.CurrentThread.Priority = priority);

    //        p.RegisterParameterHandler<int>(
    //            "number",
    //            number => Console.WriteLine("A number: {0}", number));

    //        p.RunWith(args);
    //    }
    //}

    [AttributeUsage(AttributeTargets.Parameter)]
    class LengthValidationAttribute : ValidationAttribute
    {
        public int Length { get; private set; }

        public LengthValidationAttribute(int length)
        {
            Length = length;
        }

        public override IValueValidator GetValidator()
        {
            return new LengthValidator(Length);
        }

        public override string Description
        {
            get
            {
                return string.Format(
                    "Length is at least {0}", Length);
            }
        }

        class LengthValidator : IValueValidator
        {
            public int Length { get; private set; }

            public LengthValidator(int length)
            {
                Length = length;
            }

            public void Validate(ValueInfo info)
            {
                var str = (string)info.Value;

                if (str.Length < Length)
                {
                    throw new ValidationException(
                        string.Format(
                            "Length should be at least {0}",
                            Length));
                }
            }
        }
    }


    //class Test
    //{
    //    [Verb]
    //    public static void Foo([CLAP.Validation.RegexMatches("")]string name)
    //    {
    //        Console.WriteLine("name={0}", name);
    //    }

    //    // single string overrides the name
    //    // additional names are additional

    //    // exe -d
    //    // exe -bar:arg
    //    // exe -b:arg

    //    [Global("d")]
    //    public static void Debug()
    //    {
    //        Debugger.Launch();
    //    }

    //    [Global(Aliases = "b")]
    //    public static void Bar(string arg)
    //    {
    //        Console.WriteLine("bar={0}", arg);
    //    }
    //}

    //class MyApp
    //{
    //    [Verb]
    //    public static void Print(string prefix, string name, int count)
    //    {
    //        for (int i = 0; i < count; i++)
    //        {
    //            Console.WriteLine("{0} {1}", prefix, name);
    //        }
    //    }
    //}

    //[DefaultVerb("print")]
    //class MyApp
    //{
    //    [Verb]
    //    public static void Print(string prefix, string name, int count)
    //    {
    //    }

    //    [Verb]
    //    public static void Login(string userName, string password)
    //    {
    //    }
    //}

    //[DefaultVerb("print")]
    //class MyApp
    //{
    //    [Verb(Aliases = "p")]
    //    public static void Print(string prefix, string name, int count)
    //    {
    //    }

    //    [Verb(Aliases = "l,log")]
    //    public static void Login(string userName, string password)
    //    {
    //    }
    //}

    //class MyApp
    //{
    //    [Verb]
    //    public static void Login(
    //        [Parameter(Aliases = "name,n")]
    //        string userName,
    //        [Parameter(Aliases = "pass,p")]
    //        string password)
    //    {
    //    }
    //}


    //class MyApp
    //{
    //    [Verb]
    //    public static void Print(
    //        [Parameter(Default = "Hello")]
    //        string prefix,
    //        string name,
    //        [Parameter(Default = 5)]
    //        int count,
    //        [Parameter(Aliases = "u")]
    //        bool upper)
    //    {
    //        if (upper)
    //        {
    //            prefix = prefix.ToUpper();
    //            name = name.ToUpper();
    //        }

    //        for (int i = 0; i < count; i++)
    //        {
    //            Console.WriteLine("{0} {1}", prefix, name);
    //        }
    //    }
    //}

    //class MyApp
    //{
    //    [Verb]
    //    public static void Hello(string[] names)
    //    {
    //        foreach (var name in names)
    //        {
    //            Console.WriteLine("Hello {0}", name);
    //        }
    //    }
    //}

    //class MyApp
    //{
    //    [Verb]
    //    public static void Print(
    //        [Parameter(Default = "Hello")]
    //        string prefix,
    //        [Parameter(Required = true)]
    //        string name)
    //    {
    //    }
    //}

    //class MyApp
    //{
    //    [Verb]
    //    public static void Print(
    //        string prefix,
    //        string name,
    //        [Parameter(Required = true)]
    //        [MoreThan(3)]
    //        [LessOrEqualTo(20)]
    //        int count)
    //    {
    //        for (int i = 0; i < count; i++)
    //        {
    //            Console.WriteLine("{0} {1}", prefix, name);
    //        }
    //    }
    //}

    //class MyApp
    //{
    //    [Verb(Description = "Print something")]
    //    public static void Print(
    //        [LengthValidation(10)]
    //        [Parameter(Description = "A prefix", Aliases = "p")]
    //        string prefix,

    //        [Parameter(Description = "A name", Default = "Adrian", Aliases = "n")]
    //        string name,

    //        [LessThan(10)]
    //        [Parameter(Description = "A count", Required = true, Aliases = "c")]
    //        int count,

    //        bool one)
    //    {
    //        if (one)
    //        {
    //            Console.WriteLine(name);
    //        }
    //        else
    //        {
    //            for (int i = 0; i < count; i++)
    //            {
    //                Console.WriteLine(name);
    //            }
    //        }
    //    }

    //    [Empty]
    //    public static void Help(string help, int x)
    //    {
    //        Console.WriteLine(help);
    //    }

    //    public static void Nothing()
    //    {
    //        Console.WriteLine("Nothing?");
    //    }

    //    [Verb]
    //    public static void Login(string userName, string password)
    //    {
    //    }

    //    [Global("break")]
    //    public static void Debug()
    //    {
    //        Debugger.Launch();
    //    }

    //    [Global("thread")]
    //    public static void SetThreadName(string name)
    //    {
    //        Thread.CurrentThread.Name = name;
    //    }

    //    [Global("priority")]
    //    public static void SetThreadPriority(
    //        [Parameter(Description = "The thread's priority")]
    //        ThreadPriority priority)
    //    {
    //        Thread.CurrentThread.Priority = priority;
    //    }

    //    [Global("number")]
    //    public static void DoSomethingWithAnumber([LessThan(10)] int number)
    //    {
    //        Console.WriteLine("A number: {0}", number);
    //    }
    //}

    public class SameLengthAttribute : CollectionValidationAttribute
    {
        public override string Description
        {
            get { return "All arrays are of the same length"; }
        }

        public override ICollectionValidator GetValidator()
        {
            return new SameLengthValidator();
        }

        private class SameLengthValidator : ICollectionValidator
        {
            public void Validate(ValueInfo[] parameters)
            {
                // At this point - we already know that all the parameters have a value
                // that matches their types.
                // This validator works only for arrays so we'll add
                // a simple assertion for that.
                //
                Debug.Assert(parameters.All(p => p.Value is Array));

                // Cast them all to arrays
                //
                var arrays = parameters.Select(p => p.Value).Cast<Array>();

                // Take the first length
                //
                var length = arrays.First().Length;

                // Validate that all arrays have the same length
                //
                if (!arrays.All(a => a.Length == length))
                {
                    throw new ValidationException("Not all arrays have the same length.");
                }
            }
        }
    }



    public class Person
    {
        public int Age { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<PhoneNumber> PhoneNumbers { get; set; }
    }

    public class PhoneNumber
    {
        public string Type { get; set; }
        public string Number { get; set; }
    }

    // Validate properties of this class
    [Validate("Name NOT LIKE 'Banana*'")]
    public class Product
    {
        // Validate the value of this property
        [RegexMatches("#PP[0-9]*")]
        public string ID { get; set; }

        // Validate the value of this property
        [MoreOrEqualTo(0)]
        public int Index { get; set; }

        public string Name { get; set; }

        // Validate the properties of the value of this property
        [Validate("Name NOT IN 'Adrian'")]
        public ProductReview Review { get; set; }
    }

    public class ProductReview
    {
        public string Name { get; set; }

        // Validate the value of this property
        [RegexMatches(@"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$")]
        public string Email { get; set; }
    }
}

