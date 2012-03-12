using System;
using System.Collections.Generic;
using System.Reflection;
using CLAP;
using CLAP.Interception;
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

    public class Sample_02_Default : BaseSample
    {
        [Verb(IsDefault = true)]
        public void Print([Parameter(Default = "bar")]string foo)
        {
            Printer.Print(foo);
        }
    }

    public class Sample_02_No_Default : BaseSample
    {
        [Verb(IsDefault = true)]
        public void Print(string foo)
        {
            Printer.Print(foo);
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
        [Validate("str like 'foo*'")]
        public void Print2([Parameter(Required = true, Description = "string!")]string str)
        {
        }

        [Global]
        [Validate("x not like 'foo*'")]
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

        [Global(Description = "cat!")]
        public void Cat(Dictionary<string, Sample_10> x)
        {
        }

        [Verb]
        public void WithARequiredSwitch(string str, [Parameter(Required = true)] bool sw)
        {
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

        [Error]
        public void Error(ExceptionContext ex)
        {
            Ex = ex.Exception;

            ex.ReThrow = true;
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

        [Error]
        public void Error(ExceptionContext ex)
        {
            Handled = true;

            ex.ReThrow = true;
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

    public class Sample_41 : BaseSample
    {
        public Dictionary<string, object> Values { get; private set; }

        [Verb(IsDefault = true)]
        public void Foo(string str, int num, bool b, Case c, int[] numbers)
        {
            Values = new Dictionary<string, object>
            {
                { "str", str },
                { "num", num},
                { "b", b },
                { "c", c },
                { "numbers", numbers },
            };
        }
    }

    public class Sample_42 : BaseSample
    {
        public MyType TheType { get; private set; }
        public MyType TheType_Global { get; private set; }
        public int[][] Array { get; private set; }

        [Verb(IsDefault = true)]
        public void Foo(MyType t)
        {
            TheType = t;
        }

        [Verb]
        public void Bar(int[][] arr)
        {
            Array = arr;
        }

        [Global]
        public void Glob(MyType t)
        {
            TheType_Global = t;
        }

        [Verb]
        public void Val(MyValidatedType t)
        {
        }

        [Verb]
        public void ValCollection(MyValidatedType[] t)
        {
        }

        [Verb]
        public void Complex(MyComplexType t)
        {
        }

        [Verb]
        public void Props(MyComplexType_2 t)
        {
        }

        [Verb]
        public void Zoo(TypeWithProps t)
        {
        }

        [Verb]
        public void ZooCollection(TypeWithProps[] t)
        {
        }
    }

    public class Sample_43
    {
        public PreVerbExecutionContext PreContext;
        public PostVerbExecutionContext PostContext;
        public bool VerbExecuted;

        [Verb]
        public void Foo(string str, int num)
        {
            VerbExecuted = true;
        }

        [PreVerbExecution]
        private void Pre(PreVerbExecutionContext context)
        {
            PreContext = context;

            context.UserContext.Add("a", 54);
        }

        [PostVerbExecution]
        private void Post(PostVerbExecutionContext context)
        {
            PostContext = context;
        }
    }

    public class Sample_44
    {
        public PreVerbExecutionContext PreContext;
        public PostVerbExecutionContext PostContext;
        public bool VerbExecuted;

        [Verb]
        public void Foo(string str, int num)
        {
            VerbExecuted = true;
        }

        [PreVerbExecution]
        private void Pre(PreVerbExecutionContext context)
        {
            PreContext = context;

            context.Cancel = true;
        }

        [PostVerbExecution]
        private void Post(PostVerbExecutionContext context)
        {
            PostContext = context;
        }
    }

    public class Sample_45
    {
        [PreVerbExecution]
        private void Pre1(PreVerbExecutionContext context)
        {
        }

        [PreVerbExecution]
        private void Pre2(PreVerbExecutionContext context)
        {
        }
    }

    public class Sample_46
    {
        [PostVerbExecution]
        private void Pre1(PostVerbExecutionContext context)
        {
        }

        [PostVerbExecution]
        private void Pre2(PostVerbExecutionContext context)
        {
        }
    }

    public class Sample_47
    {
        public PostVerbExecutionContext Context;

        [Verb(IsDefault = true)]
        public void Foo(int x, string y)
        {
            throw new Exception("ha!");
        }

        [PostVerbExecution]
        public void Post(PostVerbExecutionContext context)
        {
            Context = context;
        }
    }

    public class Sample_48
    {
        public PostVerbExecutionContext Context;

        [Verb(IsDefault = true)]
        public void Foo(int x, string y)
        {
            throw new Exception("ha!");
        }

        [PostVerbExecution]
        public void Post(PostVerbExecutionContext context)
        {
            Context = context;
        }

        [Error]
        public void Error(ExceptionContext ex)
        {
        }
    }

    public class Sample_49
    {
        public PostVerbExecutionContext Context;

        [Verb(IsDefault = true)]
        public void Foo(int x, string y)
        {
            throw new Exception("ha!");
        }

        [PostVerbExecution]
        public void Post(PostVerbExecutionContext context)
        {
            Context = context;
        }

        [Error]
        public void Error(ExceptionContext ex)
        {
            ex.ReThrow = true;
        }
    }

    [VerbInterception(typeof(Inter))]
    public class Sample_50
    {
        public bool VerbExecuted;

        [Verb]
        public void Foo(string str, int num)
        {
            VerbExecuted = true;
        }

        class Inter : IVerbInterceptor
        {
            public PreVerbExecutionContext PreContext;
            public PostVerbExecutionContext PostContext;

            public void BeforeVerbExecution(PreVerbExecutionContext context)
            {
                PreContext = context;

                context.UserContext.Add("a", 54);
            }

            public void AfterVerbExecution(PostVerbExecutionContext context)
            {
                PostContext = context;
            }
        }
    }

    public class Sample_51
    {
        [Verb]
        public static void Foo(string str)
        {
        }
    }

    public class Sample_52
    {
        [Verb]
        public static void Foo(string str)
        {
        }
    }

    public class Sample_53
    {
        [Verb(IsDefault = true)]
        public static void Foo()
        {
        }

        [Empty]
        public static void Bar()
        {
        }
    }

    public class Sample_54
    {
        [Verb(IsDefault = true)]
        public static void Foo()
        {
        }

        [Empty, Help]
        public static void Bar()
        {
        }
    }

    public class Sample_55
    {
        [Verb(IsDefault = true)]
        public static void Foo()
        {
        }
    }

    public class Sample_56
    {
        [PreVerbExecution]
        static void Pre(PreVerbExecutionContext x, string y)
        {
        }
    }

    public class Sample_57
    {
        [PostVerbExecution]
        static void Post(PostVerbExecutionContext c, string y)
        {
        }
    }

    public class Sample_58
    {
        [PreVerbExecution]
        static void Post(int c)
        {
        }
    }

    public class Sample_59
    {
        [PostVerbExecution]
        static void Post(int c)
        {
        }
    }

    public class Sample_60
    {
        [Verb]
        static void Foo([Parameter(Aliases = "c")]int c)
        {
        }
    }

    public class Sample_61
    {
        public int? P1 { get; set; }
        public int? P2 { get; set; }
        public int? P3 { get; set; }
        public int? P4 { get; set; }
        public int? P5 { get; set; }

        [Verb]
        void Bar(
            int? p1,
            [Parameter(Default = 5)] int? p2,
            [Parameter(Default = "5")] int? p3,
            [Parameter(Default = "")] int? p4,
            [Parameter(Default = null)] int? p5)
        {
            P1 = p1;
            P2 = p2;
            P3 = p3;
            P4 = p4;
            P5 = p5;
        }
    }

    public enum Case
    {
        Upper,
        Lower,
        Unchanged,
    }

    public class MyType
    {
        public int Number { get; set; }
        public string Name { get; set; }
    }

    [Validate("Number > 10 AND Name LIKE '*foo'")]
    public class MyValidatedType
    {
        public int Number { get; set; }
        public string Name { get; set; }
    }

    [Validate("Number < 50 AND Name IN ('foo','bar')")]
    public class MyComplexType
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public MyValidatedType Validated { get; set; }
    }

    public class MyComplexType_2
    {
        public int Number { get; set; }
        public string Name { get; set; }

        [Validate("Number < 50 AND Name IN ('foo','bar')")]
        public MyValidatedType Validated { get; set; }
    }

    public class TypeWithProps
    {
        [MoreThan(10)]
        public int Number { get; set; }
    }

    public class ParameterWithDefaults_1
    {
        public string P1 { get; set; }
        public string P2 { get; set; }

        [Verb]
        public void Foo(
            [Parameter(Default = "def1")] string p1,
            [Parameter(DefaultProvider = typeof(MyDefault))] string p2)
        {
            P1 = p1;
            P2 = p2;
        }

        public class MyDefault : DefaultProvider
        {
            public override object GetDefault(VerbExecutionContext context)
            {
                return "def2";
            }
        }
    }

    public class ParameterWithDefaults_2
    {
        [Verb]
        public void Foo(
            [Parameter(Default = "def1", DefaultProvider = typeof(MyDefault))] string p1)
        {
        }

        public class MyDefault : DefaultProvider
        {
            public override object GetDefault(VerbExecutionContext context)
            {
                return "def2";
            }
        }
    }

    public class ParameterWithDefaults_3
    {
        [Verb]
        public void Foo(
            [Parameter(DefaultProvider = typeof(System.Text.StringBuilder))] string p1)
        {
        }
    }
}