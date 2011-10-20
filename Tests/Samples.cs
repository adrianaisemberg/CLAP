﻿using System;
using System.Collections.Generic;
using System.Reflection;
using CLAP;
using CLAP.Validation;

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

    public class Sample_02 : BaseSample
    {
        [Verb(IsDefault = true, Aliases = "p", Description = "Prints a message")]
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

    public class Sample_04 : BaseSample
    {
        [Verb(IsDefault = true, Aliases = "p", Description = "Prints a message")]
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

    public class Sample_05 : BaseSample
    {
        [Verb(IsDefault = true, Aliases = "p", Description = "Prints a message")]
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

    public class Sample_06 : BaseSample
    {
        [Verb(IsDefault = true, Aliases = "p", Description = "Prints a message")]
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
        [Verb(IsDefault = true, Description = "prints")]
        public void Print()
        {
        }

        [Verb]
        public void Print2([Parameter(Required = true)]string str)
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

        [Global(Aliases = "m")]
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

    public class Sample_30 : BaseSample
    {
        [Verb(IsDefault = true)]
        public void Foo()
        {
            Printer.Print("works!");
        }
    }

    public class Sample_31 : BaseSample
    {
        [Verb(IsDefault = true)]
        public void Foo(string x, int y)
        {
            Printer.Print("works!");
        }
    }

    public class Sample_32 : BaseSample
    {
        [Error]
        public void Error1()
        {
        }

        [Error]
        public void Error2()
        {
        }
    }

    public class Sample_33 : BaseSample
    {
        public Exception Ex { get; set; }

        [Error(ReThrow = true)]
        public void Error(Exception ex)
        {
            Ex = ex;
        }

        [Verb]
        public void Foo()
        {
            throw new Exception("blah");
        }
    }

    public class Sample_34 : BaseSample
    {
        public bool Handled { get; set; }

        [Error(ReThrow = true)]
        public void Error()
        {
            Handled = true;
        }

        [Verb]
        public void Foo()
        {
            throw new Exception("blah");
        }
    }

    public class Sample_35 : BaseSample
    {
        [Error]
        public void Error(int x)
        {
        }
    }

    public class Sample_36 : BaseSample
    {
        [Error]
        public void Error(TargetInvocationException ex)
        {
        }
    }

    public class Sample_37 : BaseSample
    {
        [Error]
        public void Error(Exception ex1, Exception ex2)
        {
        }
    }

    public class Sample_38 : BaseSample
    {
        [Verb(IsDefault = true)]
        public void Foo1()
        {
        }

        [Verb(IsDefault = true)]
        public void Foo2()
        {
        }
    }

    public class Sample_39 : BaseSample
    {
        [Verb(IsDefault = true)]
        public void Foo()
        {
            throw new Exception();
        }
    }

    public class All_Validations_Sample
    {
        [Verb]
        [Validate("")]
        public static void Foo(
            [MoreThan(1)]
            [MoreOrEqualTo(1)]
            [LessThan(1)]
            [LessOrEqualTo(1)]
            [RegexMatches("1")]
            [FileExists]
            [DirectoryExists]
            [PathExists]
            int x)
        {
        }
    }

    public class Sample_40 : BaseSample
    {
        [Verb]
        [Validate("p1 > p2")]
        public static void Foo1(int p1, int p2)
        {
        }

        [Verb]
        [Validate("p1 > p2")]
        [Validate("p1 > 10")]
        public static void Foo2(int p1, int p2)
        {
        }

        [Verb]
        [Validate("str in ('abc','def')")]
        public static void Foo3(string str)
        {
        }

        [Verb]
        [Validate("str in ('abc','def')", CaseSensitive = true)]
        public static void Foo5(string str)
        {
        }

        [Verb]
        [Validate("str like '*foo'")]
        public static void Foo4(string str)
        {
        }

        [Verb]
        public static void Dummy()
        {
        }

        [Global]
        [Validate("num > 10")]
        public static void Boing(int num)
        {
        }
    }

    public class InterceptorSample : BaseSample
    {
        public IList<string> Intercepted { get; private set; }
        public IList<string> Invoked { get; private set; }

        public InterceptorSample()
        {
            Intercepted = new List<string>();
            Invoked = new List<string>();
        }

        [Verb]
        public void Foo1(string str)
        {
            Invoked.Add("Foo1:" + str);
        }

        [Verb]
        public void Foo2(string str)
        {
            Invoked.Add("Foo2:" + str);
        }

        [VerbInterceptor]
        public void Intercept(IVerbInvocation invocation)
        {
            Intercepted.Add(invocation.Verb);
        }
    }

    public class MultipleInterceptorSample : BaseSample
    {
        [Verb]
        public void Foo(string str)
        { }

        [VerbInterceptor]
        public void Intercept1(IVerbInvocation invocation)
        { }

        [VerbInterceptor] // second interceptor results in error
        public void Intercept2(IVerbInvocation invocation)
        { }
    }

    public class InvalidInterceptorSample : BaseSample
    {
        [Verb]
        public void Foo(string str)
        { }

        [VerbInterceptor]
        public void Intercept(int wrong)
        { }
    }

    public class InterceptorBaseSample : BaseSample
    {
        public IList<string> Intercepted { get; private set; }

        public InterceptorBaseSample()
        {
            Intercepted = new List<string>();
        }

        [VerbInterceptor]
        public void Intercept(IVerbInvocation invocation)
        {
            Intercepted.Add(invocation.Verb);
            invocation.Proceed();
        }
    }

    public class VerbWithInterceptorBaseSample : InterceptorBaseSample
    {
        public IList<string> Invoked { get; private set; }

        public VerbWithInterceptorBaseSample()
        {
            Invoked = new List<string>();
        }

        [Verb]
        public void Foo(string str)
        {
            Invoked.Add("Foo:" + str);
        }
    }

    public class VerbBaseSample : BaseSample
    {
        public IList<string> Invoked { get; private set; }

        public VerbBaseSample()
        {
            Invoked = new List<string>();
        }

        [Verb]
        public void Foo(string str)
        {
            Invoked.Add("Foo:" + str);
        }
    }

    public class InterceptorWithVerbBaseSample : VerbBaseSample
    {
        public IList<string> Intercepted { get; private set; }

        public InterceptorWithVerbBaseSample()
        {
            Intercepted = new List<string>();
        }

        [VerbInterceptor]
        public void Intercept(IVerbInvocation invocation)
        {
            Intercepted.Add(invocation.Verb);
            invocation.Proceed();
        }
    }

    public enum Case
    {
        Upper,
        Lower,
        Unchanged,
    }
}