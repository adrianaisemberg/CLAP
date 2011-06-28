using System;
using System.Collections.Generic;
using CLAP;
using CLAP.Validation;
using System.Reflection;

namespace Tests
{
    public interface IPrinter
    {
        void Print(String text);
    }

    public class Printer : IPrinter
    {
        public List<String> PrintedTexts = new List<String>();

        public void Print(String text)
        {
            PrintedTexts.Add(text);
        }
    }

    public class BaseSample
    {
        public IPrinter Printer { get; set; }
    }

    [DefaultVerb("print")]
    public class Sample_02 : BaseSample
    {
        [Verb(Aliases = "p", Description = "Prints a message")]
        public void Print(

            [Parameter(Aliases = "c")]
            Int32 count,

            [Parameter(Aliases = "m,msg", Default = "hello")]
            String message,

            String prefix,

            [Parameter(Aliases = "u", Default = false)]
            Boolean upper)
        {
            for (var i = 0; i < count; i++)
            {
                var text = prefix + message;

                Printer.Print(upper ? text.ToUpper() : text);
            }
        }
    }

    public class ValidationSample_01 : BaseSample
    {
        [Verb]
        public void MoreThan5([MoreThan(5)] Int32 n) { }

        [Verb]
        public void MoreOrEqualTo10([MoreOrEqualTo(10)] Int32 n) { }

        [Verb]
        public void LessThan20([LessThan(20)] Int32 n) { }

        [Verb]
        public void LessOrEqualTo30([LessOrEqualTo(30)] Int32 n) { }

        [Verb]
        public void RegexMatchesEmail([RegexMatches(@"^([0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$")] String text) { }

        [Verb]
        public void FileExists([FileExists] string path) { }

        [Verb]
        public void UriFileExists([FileExists] Uri path) { }

        [Verb]
        public void DirectoryExists([DirectoryExists] string path) { }

        [Verb]
        public void UriDirectoryExists([DirectoryExists] Uri path) { }

        [Verb]
        public void PathExists([PathExists] string path) { }

        [Verb]
        public void UriPathExists([PathExists] Uri path) { }
    }

    public class Sample_03 : BaseSample
    {
        [Verb(Aliases = "p", Description = "Prints a message")]
        public void Print(

            [Parameter(Aliases = "c")]
            [MoreThan(5)]
            [LessOrEqualTo(10)]
            Int32 count,

            [Parameter(Aliases = "m.msg", Default = "hello")]
            String message,

            String prefix)
        {
            for (var i = 0; i < count; i++)
            {
                Printer.Print(prefix + message);
            }
        }
    }

    [DefaultVerb("p")]
    public class Sample_04 : BaseSample
    {
        [Verb(Aliases = "p", Description = "Prints a message")]
        public void Print(

            Int32 count,

            [Parameter(Aliases = "m,msg", Default = "hello")]
            String message,

            String prefix,

            Case c)
        {
            for (var i = 0; i < count; i++)
            {
                var text = prefix + message;

                switch (c)
                {
                    case Case.Upper:
                        text = text.ToUpperInvariant();
                        break;
                    case Case.Lower:
                        text = text.ToLowerInvariant();
                        break;
                    case Case.Unchanged:
                    default:
                        break;
                }

                Printer.Print(text);
            }
        }
    }

    [DefaultVerb("p")]
    public class Sample_05 : BaseSample
    {
        [Verb(Aliases = "p", Description = "Prints a message")]
        public void Print(

            [Parameter(Aliases = "c")]
            int count,
            String c,
            String x,

            [Parameter(Aliases = "X")]
            int number,
            String y)
        {
        }
    }

    [DefaultVerb("p")]
    public class Sample_06 : BaseSample
    {
        [Verb(Aliases = "p", Description = "Prints a message")]
        public void Print(

            Int32 count,

            [Parameter(Aliases = "m,msg", Default = "hello")]
            String message,

            String prefix,

            [Parameter(Default = Case.Lower)]
            Case c)
        {
            for (var i = 0; i < count; i++)
            {
                var text = prefix + message;

                switch (c)
                {
                    case Case.Upper:
                        text = text.ToUpperInvariant();
                        break;
                    case Case.Lower:
                        text = text.ToLowerInvariant();
                        break;
                    case Case.Unchanged:
                    default:
                        break;
                }

                Printer.Print(text);
            }
        }
    }

    public class Sample_07 : BaseSample
    {
        [Verb]
        public void Print(

            [Parameter(Default = 1)]
            Int32 count,

            [Parameter(Required = false)]
            String message,

            [Parameter(Required = true)]
            String prefix,

            Case c)
        {
            for (var i = 0; i < count; i++)
            {
                var text = prefix + message;

                switch (c)
                {
                    case Case.Upper:
                        text = text.ToUpperInvariant();
                        break;
                    case Case.Lower:
                        text = text.ToLowerInvariant();
                        break;
                    case Case.Unchanged:
                    default:
                        break;
                }

                Printer.Print(text);
            }
        }
    }

    public class Sample_08 : BaseSample
    {
        [Verb]
        public void Print(String[] messages, String prefix)
        {
            if (messages == null)
            {
                return;
            }

            foreach (var msg in messages)
            {
                Printer.Print(prefix + msg);
            }
        }

        [Verb]
        public void PrintNumbers(Int32[] numbers, String prefix)
        {
            if (numbers == null)
            {
                return;
            }

            foreach (var msg in numbers)
            {
                Printer.Print(prefix + msg);
            }
        }

        [Verb]
        public void PrintEnums(Case[] enums, String prefix)
        {
            if (enums == null)
            {
                return;
            }

            foreach (var msg in enums)
            {
                Printer.Print(prefix + msg);
            }
        }
    }

    public class Sample_09 : BaseSample
    {
        [Empty]
        public void Foo()
        {
            Printer.Print("a");
        }
    }

    public class Sample_10 : BaseSample
    {
        [Verb]
        public void Print()
        {
        }

        [Global]
        public void Foo(string x)
        {
            Printer.Print(x);
        }

        [Global]
        public void Bar()
        {
            Printer.Print("zoo");
        }

        [Global]
        public void Mish([LessThan(10)] int count)
        {
            for (int i = 0; i < count; i++)
            {
                Printer.Print("mesh");
            }
        }

        [Global]
        public void Abra(BindingFlags f)
        {
        }

        [Help]
        public void ShowHelp(string help)
        {
            Printer.Print("help");
        }
    }

    public class Sample_11 : BaseSample
    {
        public void Print()
        {
            Printer.Print("x");
        }
    }

    public class Sample_12 : BaseSample
    {
        [Verb]
        public void Print()
        {
        }

        [Global]
        public void Foo(int x, int y)
        {
        }
    }

    public class Sample_13 : BaseSample
    {
        [Empty]
        public void Foo()
        {
        }

        [Empty]
        public void Bar()
        {
        }
    }

    public class Sample_14 : BaseSample
    {
        [Empty]
        public static void Foo()
        {
        }
    }

    public class Sample_15 : BaseSample
    {
        [Empty]
        public void Foo()
        {
        }
    }

    public class Sample_16 : BaseSample
    {
        public static IPrinter StaticPrinter { get; set; }

        [Empty]
        public static void Foo()
        {
            StaticPrinter.Print("a");
        }
    }

    public class Sample_17 : BaseSample
    {
        [Empty, Help]
        public void Foo(string h)
        {
            Printer.Print("a");
        }
    }

    public class Sample_18 : BaseSample
    {
        [Empty, Help]
        public void Foo(int h)
        {
            Printer.Print("a");
        }
    }

    public class Sample_19 : BaseSample
    {
        [Empty, Help]
        public void Foo(string h, int n)
        {
            Printer.Print("a");
        }
    }

    public class Sample_20 : BaseSample
    {
        [Empty]
        public void Foo(string h)
        {
        }
    }

    public class Sample_21 : BaseSample
    {
        [Help(Aliases = "h,?")]
        public void Help(string h)
        {
            Printer.Print("help");
        }
    }

    public class Sample_22 : BaseSample
    {
        public static IPrinter StaticPrinter { get; set; }

        [Help(Aliases = "h,?")]
        public static void Help(string h)
        {
            StaticPrinter.Print("help");
        }
    }

    public class Sample_23 : BaseSample
    {
        [Help(Aliases = "h,?")]
        public void Help(string h)
        {
            Printer.Print("help");
        }
    }

    public class Sample_24 : BaseSample
    {
        [Help(Aliases = "h,?")]
        public void Help(string h, string boo)
        {
            Printer.Print("help");
        }
    }

    public class Sample_25 : BaseSample
    {
        [Help(Aliases = "h,?")]
        public void Help(int i)
        {
        }

        [Verb]
        public static void Foo()
        {
        }
    }

    [DefaultVerb("loof")]
    public class Sample_26 : BaseSample
    {
        [Verb]
        public static void Foo()
        {
        }
    }

    public class Sample_27 : BaseSample
    {
        public static IPrinter StaticPrinter { get; set; }

        [Verb]
        public static void Foo(string x)
        {
            StaticPrinter.Print(x);
        }
    }

    public class Sample_28 : BaseSample
    {
        [Verb]
        public void Foo(string x, Guid g)
        {
            Printer.Print(x + g);
        }

        [Verb]
        public void Zoo([Parameter(Default = "5")] int n)
        {
            Printer.Print(n.ToString());
        }

        [Verb]
        public void Bar(
            string x,
            [Parameter(Default = "{2FBBAAAA-02AF-4F40-BADE-957F566B221E}")]
            Guid g)
        {
            Printer.Print(x + g);
        }

        [Empty, Help]
        public void Help(string help)
        {
            Printer.Print(help);
        }
    }

    public class Sample_29 : BaseSample
    {
        [Verb]
        public void Foo(string x, Uri u)
        {
            Printer.Print(x + u);
        }

        [Verb]
        public void Bar(
            string x,
            [Parameter(Default = "http://www.com")]
            Uri u)
        {
            Printer.Print(x + u);
        }
    }

    public enum Case
    {
        Upper,
        Lower,
        Unchanged,
    }
}