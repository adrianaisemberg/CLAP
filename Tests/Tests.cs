using System;
using System.Linq;
using CLAP;
using Moq;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class Tests
    {
        private void Execute<T>(T obj, String cmd)
        {
            Parser<T>.Run(cmd.Split(' '), obj);
        }

        [Test]
        public void Execute_Verb()
        {
            var printer = new Printer();
            var sample = new Sample_02 { Printer = printer };

            Execute(sample, "print /c=5 /msg=test /prefix=hello_");

            Assert.AreEqual(5, printer.PrintedTexts.Count);
            Assert.IsTrue(printer.PrintedTexts.All(t => t.Equals("hello_test")));
        }

        [Test]
        public void Execute_DefaultVerb()
        {
            var printer = new Printer();
            var sample = new Sample_02 { Printer = printer };

            Execute(sample, "/c=5 /msg=test /prefix=hello_");

            Assert.AreEqual(5, printer.PrintedTexts.Count);
            Assert.IsTrue(printer.PrintedTexts.All(t => t.Equals("hello_test")));
        }

        [Test]
        public void Execute_DefaultVerb_Switch()
        {
            var printer = new Printer();
            var sample = new Sample_02 { Printer = printer };

            Execute(sample, "/c=5 /msg=test /prefix=hello_ /u");

            Assert.AreEqual(5, printer.PrintedTexts.Count);
            Assert.IsTrue(printer.PrintedTexts.All(t => t.Equals("HELLO_TEST")));
        }

        [Test]
        public void Execute_WithEnum()
        {
            var printer = new Printer();
            var sample = new Sample_04 { Printer = printer };

            Execute(sample, "/count=5 /msg=test /prefix=hello_ /c:Upper");

            Assert.AreEqual(5, printer.PrintedTexts.Count);
            Assert.IsTrue(printer.PrintedTexts.All(t => t.Equals("HELLO_TEST")));
        }

        [Test]
        public void Execute_WithDefaultEnum()
        {
            var printer = new Printer();
            var sample = new Sample_06 { Printer = printer };

            Execute(sample, "/count=5 /msg=test /prefix=hello_");

            Assert.AreEqual(5, printer.PrintedTexts.Count);
            Assert.IsTrue(printer.PrintedTexts.All(t => t.Equals("hello_test")));
        }

        [Test]
        public void Execute_DuplicateParameterNames()
        {
            try
            {
                var printer = new Printer();
                var sample = new Sample_05 { Printer = printer };

                Execute(sample, "p");

                Assert.Fail();
            }
            catch (InvalidOperationException ex)
            {
                Assert.IsTrue(ex.Message.Contains("Duplicate parameter names found in Print: c, x"));
            }
        }

        [Test]
        public void Validation_MoreThan()
        {
            var sample = new ValidationSample_01();
            Execute(sample, "morethan5 /n=10");

            try
            {
                Execute(sample, "morethan5 /n=1");

                Assert.Fail();
            }
            catch (ValidationException)
            {
            }
        }

        [Test]
        public void Validation_MoreOrEqualTo()
        {
            var sample = new ValidationSample_01();
            Execute(sample, "moreorequalto10 /n=10");
            Execute(sample, "moreorequalto10 /n=11");

            try
            {
                Execute(sample, "moreorequalto10 /n=9");

                Assert.Fail();
            }
            catch (ValidationException)
            {
            }
        }

        [Test]
        public void Validation_LessThan()
        {
            var sample = new ValidationSample_01();
            Execute(sample, "lessthan20 /n=10");

            try
            {
                Execute(sample, "lessthan20 /n=20");

                Assert.Fail();
            }
            catch (ValidationException)
            {
            }
        }

        [Test]
        public void Validation_LessOrEqualTo()
        {
            var sample = new ValidationSample_01();
            Execute(sample, "lessorequalto30 /n=10");
            Execute(sample, "lessorequalto30 /n=30");

            try
            {
                Execute(sample, "lessorequalto30 /n=40");

                Assert.Fail();
            }
            catch (ValidationException)
            {
            }
        }

        [Test]
        public void Validation_RegexMatchesEmail()
        {
            var sample = new ValidationSample_01();
            Execute(sample, "regexmatchesemail /text=name@email.com");
            Execute(sample, "regexmatchesemail /text=more@some.email.co.il");

            try
            {
                Execute(sample, "regexmatchesemail /text=no_email");

                Assert.Fail();
            }
            catch (ValidationException)
            {
            }
        }

        [Test]
        public void Required()
        {
            var printer = new Printer();
            var sample = new Sample_07 { Printer = printer };

            Execute(sample, "print /prefix:hello");

            Assert.AreEqual(1, printer.PrintedTexts.Count);
            Assert.AreEqual("HELLO", printer.PrintedTexts[0]);
        }

        [Test]
        public void Required_Exception()
        {
            try
            {
                var printer = new Printer();
                var sample = new Sample_07 { Printer = printer };

                Execute(sample, "print /message:world");

                Assert.Fail();
            }
            catch (CommandLineException)
            {
            }
        }

        [Test]
        public void Array_Strings()
        {
            var printer = new Printer();
            var sample = new Sample_08 { Printer = printer };

            Execute(sample, "print /messages:a,b,c /prefix:test_");

            Assert.AreEqual(3, printer.PrintedTexts.Count);
            Assert.AreEqual("test_a", printer.PrintedTexts[0]);
            Assert.AreEqual("test_b", printer.PrintedTexts[1]);
            Assert.AreEqual("test_c", printer.PrintedTexts[2]);
        }

        [Test]
        public void Array_Strings_NoInput()
        {
            var printer = new Printer();
            var sample = new Sample_08 { Printer = printer };

            Execute(sample, "print /prefix:test_");

            Assert.AreEqual(0, printer.PrintedTexts.Count);
        }

        [Test]
        public void Array_Numbers()
        {
            var printer = new Printer();
            var sample = new Sample_08 { Printer = printer };

            Execute(sample, "printnumbers /numbers:1,2,3 /prefix:test_");

            Assert.AreEqual(3, printer.PrintedTexts.Count);
            Assert.AreEqual("test_1", printer.PrintedTexts[0]);
            Assert.AreEqual("test_2", printer.PrintedTexts[1]);
            Assert.AreEqual("test_3", printer.PrintedTexts[2]);
        }

        [Test]
        public void Array_Numbers_NoInput()
        {
            var printer = new Printer();
            var sample = new Sample_08 { Printer = printer };

            Execute(sample, "printnumbers /prefix:test_");

            Assert.AreEqual(0, printer.PrintedTexts.Count);
        }

        [Test]
        public void Array_Enum()
        {
            var printer = new Printer();
            var sample = new Sample_08 { Printer = printer };

            Execute(sample, "printenums /enums:Upper,Lower /prefix:test_");

            Assert.AreEqual(2, printer.PrintedTexts.Count);
            Assert.AreEqual("test_Upper", printer.PrintedTexts[0]);
            Assert.AreEqual("test_Lower", printer.PrintedTexts[1]);
        }

        [Test]
        public void Array_Enum_NoInput()
        {
            var printer = new Printer();
            var sample = new Sample_08 { Printer = printer };

            Execute(sample, "printenums /prefix:test_");

            Assert.AreEqual(0, printer.PrintedTexts.Count);
        }

        [Test]
        public void RegisterParameterHandler_CallsTheHandler_IgnoreTheValue()
        {
            var printer = new Printer();
            var sample = new Sample_02 { Printer = printer };

            int x = 0;

            var p = Parser.Create<Sample_02>();
            p.RegisterParameterHandler("inc", delegate { x++; });

            p.Run("print /c=5 /msg=test /prefix=hello_ /inc".Split(' '), sample);

            Assert.AreEqual(1, x);
        }

        [Test]
        public void RegisterParameterHandler_CallsTheHandler_UseTheValue()
        {
            var printer = new Printer();
            var sample = new Sample_02 { Printer = printer };

            var debug = String.Empty;

            var p = Parser.Create<Sample_02>();
            p.RegisterParameterHandler<string>("debug", str => debug = str);

            p.Run("print /c=5 /msg=test /prefix=hello_ /debug=true".Split(' '), sample);

            Assert.AreEqual("true", debug);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Parser_NullType_Exception()
        {
            var p = new Parser(null);
        }

        [Test]
        public void Global_Defined_WithArg()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_10 { Printer = mock.Object };

            var p = Parser.Create<Sample_10>();

            p.Run("print -foo:blah".Split(' '), sample);

            mock.Verify(o => o.Print("blah"));
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void Global_Defined_MoreThanOneParameter_Exception()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_12 { Printer = mock.Object };

            var p = Parser.Create<Sample_12>();

            p.Run("print -foo".Split(' '), sample);
        }

        [Test]
        public void Global_Defined_NoArgs()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_10 { Printer = mock.Object };

            var p = Parser.Create<Sample_10>();

            p.Run("print -bar".Split(' '), sample);

            mock.Verify(o => o.Print("zoo"));
        }

        [Test]
        public void Global_Defined_WithValidation()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_10 { Printer = mock.Object };

            var p = Parser.Create<Sample_10>();

            p.Run("print -mish:5".Split(' '), sample);

            mock.Verify(o => o.Print("mesh"), Times.Exactly(5));
        }

        [Test]
        [ExpectedException(typeof(TypeConvertionException))]
        public void Global_Defined_BadConvertion()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_10 { Printer = mock.Object };

            var p = Parser.Create<Sample_10>();

            p.Run("print -abra:cadabra".Split(' '), sample);
        }

        [Test]
        public void Help_Defined()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_10 { Printer = mock.Object };

            var p = Parser.Create<Sample_10>();

            p.Run("-showhelp".Split(' '), sample);
            p.Run("showhelp".Split(' '), sample);

            mock.Verify(o => o.Print("help"), Times.Exactly(2));
        }

        [Test]
        public void Help_Registered()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_11 { Printer = mock.Object };

            var p = Parser.Create<Sample_11>();

            p.RegisterHelpHandler("help", s => sample.Print());

            p.Run("-help".Split(' '), sample);
            p.Run("help".Split(' '), sample);

            mock.Verify(o => o.Print("x"), Times.Exactly(2));
        }

        [Test]
        public void EmptyHandler_NonStatic_CalledWhenNoArgs()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_09 { Printer = mock.Object };

            var p = Parser.Create<Sample_09>();

            p.Run(new string[] { }, sample);

            mock.Verify(o => o.Print("a"));
        }

        [Test]
        public void EmptyHandler_Static_CalledWhenNoArgs()
        {
            var mock = new Mock<IPrinter>();

            Sample_16.StaticPrinter = mock.Object;

            var sample = new Sample_16();

            var p = Parser.Create<Sample_16>();

            p.Run(new string[] { });

            mock.Verify(o => o.Print("a"));

            Sample_16.StaticPrinter = null;
        }

        [Test]
        [ExpectedException(typeof(ArgumentMismatchException))]
        public void EmptyDefined_WithParameters_Exception()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_20 { Printer = mock.Object };

            var p = Parser.Create<Sample_20>();

            p.Run(new string[] { }, sample);
        }

        [Test]
        public void Empty_Registered()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_11 { Printer = mock.Object };

            var p = Parser.Create<Sample_11>();

            p.RegisterEmptyHandler(() => sample.Print());

            p.Run(new string[] { }, sample);

            mock.Verify(o => o.Print("x"));
        }

        [Test]
        [ExpectedException(typeof(MoreThanOneEmptyHandlerException))]
        public void Empty_MoreThanOne_Exception()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_13 { Printer = mock.Object };

            var p = Parser.Create<Sample_13>();

            p.Run(new string[] { }, sample);
        }

        [Test]
        [ExpectedException(typeof(ParserExecutionTargetException))]
        public void Empty_Defined_Static_TargetNotNull_Exception()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_14 { Printer = mock.Object };

            var p = Parser.Create<Sample_14>();

            p.Run(new string[] { }, sample);
        }

        [Test]
        [ExpectedException(typeof(ParserExecutionTargetException))]
        public void Empty_Defined_NotStatic_TargetNull_Exception()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_15 { Printer = mock.Object };

            var p = Parser.Create<Sample_15>();

            p.Run(new string[] { });
        }

        [Test]
        public void EmptyHelp_Defined_Called()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_17 { Printer = mock.Object };

            var p = Parser.Create<Sample_17>();

            p.Run(new string[] { }, sample);

            mock.Verify(o => o.Print("a"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentMismatchException))]
        public void EmptyHelp_Defined_IntParameter_Exception()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_18 { Printer = mock.Object };

            var p = Parser.Create<Sample_18>();

            p.Run(new string[] { }, sample);
        }

        [Test]
        [ExpectedException(typeof(ArgumentMismatchException))]
        public void EmptyHelp_Defined_TwoParameters_Exception()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_19 { Printer = mock.Object };

            var p = Parser.Create<Sample_19>();

            p.Run(new string[] { }, sample);
        }

        [Test]
        public void Help_WithAliases()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_21 { Printer = mock.Object };

            var p = Parser.Create<Sample_21>();

            p.Run(new[] { "help" }, sample);
            p.Run(new[] { "h" }, sample);
            p.Run(new[] { "?" }, sample);
            p.Run(new[] { "-help" }, sample);
            p.Run(new[] { "-h" }, sample);
            p.Run(new[] { "-?" }, sample);

            mock.Verify(o => o.Print("help"), Times.Exactly(6));
        }

        [Test]
        [ExpectedException(typeof(ParserExecutionTargetException))]
        public void Help_Static_CalledWithTarget_Exception()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_22 { Printer = mock.Object };

            var p = Parser.Create<Sample_22>();

            p.Run(new[] { "-?" }, sample);
        }

        [Test]
        [ExpectedException(typeof(ParserExecutionTargetException))]
        public void Help_NonStatic_CalledWithNull_Exception()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_23 { Printer = mock.Object };

            var p = Parser.Create<Sample_23>();

            p.Run(new[] { "-?" });
        }

        [Test]
        public void Help_Static()
        {
            var mock = new Mock<IPrinter>();

            Sample_22.StaticPrinter = mock.Object;

            var sample = new Sample_22 { Printer = mock.Object };

            var p = Parser.Create<Sample_22>();

            p.Run(new[] { "-?" });

            Sample_22.StaticPrinter = null;
        }

        [Test]
        public void Help_NonStatic()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_23 { Printer = mock.Object };

            var p = Parser.Create<Sample_23>();

            p.Run(new[] { "-?" }, sample);

            mock.Verify(o => o.Print("help"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentMismatchException))]
        public void Help_MoreThanOneParameter_Exception()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_24 { Printer = mock.Object };

            var p = Parser.Create<Sample_24>();

            p.Run(new[] { "-?" }, sample);
        }

        [Test]
        [ExpectedException(typeof(ArgumentMismatchException))]
        public void Help_IntParameter_Exception()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_25 { Printer = mock.Object };

            var p = Parser.Create<Sample_25>();

            p.Run(new[] { "-?" }, sample);
        }

        [Test]
        [ExpectedException(typeof(MissingArgumentPrefixException))]
        public void MapArguments_InvalidPrefix_Exception()
        {
            var printer = new Printer();
            var sample = new Sample_02 { Printer = printer };

            Execute(sample, "print *c=5 /msg=test /prefix=hello_");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RegisterHelpHandler_MoreThanOnce_Exception()
        {
            var p = Parser.Create<Sample_25>();

            p.RegisterEmptyHandler(delegate { });
            p.RegisterEmptyHandler(delegate { });
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RegisterEmptyHelpHandler_MoreThanOnce_Exception()
        {
            var p = Parser.Create<Sample_25>();

            p.RegisterEmptyHelpHandler(delegate { });
            p.RegisterEmptyHelpHandler(delegate { });
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RegisterHelpHandler_MoreThanOnce_SameKey_Exception()
        {
            var p = Parser.Create<Sample_25>();

            p.RegisterHelpHandler("a", delegate { });
            p.RegisterHelpHandler("b", delegate { });
            p.RegisterHelpHandler("a", delegate { });
        }

        [Test]
        [ExpectedException(typeof(MissingVerbException))]
        public void Run_Verb_NoMatchingMethod_Exception()
        {
            var p = Parser.Create<Sample_25>();

            p.Run(new[] { "boo!" });
        }

        [Test]
        [ExpectedException(typeof(MissingDefaultVerbException))]
        public void Run_NoVerb_NoDefaultVerb_Exception()
        {
            var p = Parser.Create<Sample_25>();

            p.Run(new string[] { "-x" });
        }

        [Test]
        [ExpectedException(typeof(MissingVerbException))]
        public void Run_DefaultVerb_NoMatchingDefaultMethod_Exception()
        {
            var p = Parser.Create<Sample_26>();

            p.Run(new string[] { "-x" });
        }

        [Test]
        public void GenericParser_Run()
        {
            var mock = new Mock<IPrinter>();

            Sample_27.StaticPrinter = mock.Object;

            Parser<Sample_27>.Run("foo -x:bar".Split(' '));

            Sample_27.StaticPrinter = null;

            mock.Verify(o => o.Print("bar"));
        }

        [Test]
        public void Parse_GuidParameter()
        {
            var mock = new Mock<IPrinter>();

            var sample = new Sample_28 { Printer = mock.Object };

            Parser<Sample_28>.Run("foo -x:string -g:{0813A561-AC86-4C82-8EB1-0B6814637C7C}".Split(' '), sample);

            mock.Verify(o => o.Print("string0813A561-AC86-4C82-8EB1-0B6814637C7C".ToLower()));
        }

        [Test]
        public void Parse_GuidParameter_NoInput()
        {
            var mock = new Mock<IPrinter>();

            var sample = new Sample_28 { Printer = mock.Object };

            Parser<Sample_28>.Run("foo -x:string".Split(' '), sample);

            mock.Verify(o => o.Print("string" + Guid.Empty));
        }

        [Test]
        public void Parse_GuidParameter_WithDefault()
        {
            var mock = new Mock<IPrinter>();

            var sample = new Sample_28 { Printer = mock.Object };

            Parser<Sample_28>.Run("bar -x:string".Split(' '), sample);

            mock.Verify(o => o.Print("string2FBBAAAA-02AF-4F40-BADE-957F566B221E".ToLower()));
        }

        [Test]
        public void GetHelp_GuidParameter()
        {
            var mock = new Mock<IPrinter>();

            var sample = new Sample_28 { Printer = mock.Object };

            // should not fail. I don't care what the help string is
            //
            Parser<Sample_28>.Run(new string[0], sample);
        }

        [Test]
        public void Default_IntAsString()
        {
            var mock = new Mock<IPrinter>();

            var sample = new Sample_28 { Printer = mock.Object };

            Parser<Sample_28>.Run(new[] { "zoo" }, sample);

            mock.Verify(o => o.Print("5"));
        }

        [Test]
        public void Parse_UriParameter()
        {
            var mock = new Mock<IPrinter>();

            var sample = new Sample_29 { Printer = mock.Object };

            Parser<Sample_29>.Run("foo -x:string -u:http://www.com".Split(' '), sample);

            mock.Verify(o => o.Print("stringhttp://www.com/".ToLower()));
        }

        [Test]
        public void Parse_UriParameter_NoInput()
        {
            var mock = new Mock<IPrinter>();

            var sample = new Sample_29 { Printer = mock.Object };

            Parser<Sample_29>.Run("foo -x:string".Split(' '), sample);

            mock.Verify(o => o.Print("string"));
        }

        [Test]
        public void Parse_UriParameter_WithDefault()
        {
            var mock = new Mock<IPrinter>();

            var sample = new Sample_29 { Printer = mock.Object };

            Parser<Sample_29>.Run("bar -x:string".Split(' '), sample);

            mock.Verify(o => o.Print("stringhttp://www.com/".ToLower()));
        }

        [Test]
        public void Validation_FileExists()
        {
            var sample = new ValidationSample_01();
            Execute(sample, @"fileexists /path=c:\windows\system32\cmd.exe");
            Execute(sample, @"fileexists /path=%WINDIR%\system32\cmd.exe");

            Execute(sample, @"urifileexists /path=c:\windows\system32\cmd.exe");
            Execute(sample, @"urifileexists /path=%WINDIR%\system32\cmd.exe");

            try
            {
                Execute(sample, @"fileexists /path=y:\{B2C97314-4C55-4EB9-9049-63BB65AC980A}.{6E8698D0-4CFA-4ACB-8AA3-26476F490228}");

                Assert.Fail();
            }
            catch (ValidationException)
            {
            }

            try
            {
                Execute(sample, @"urifileexists /path=y:\{B2C97314-4C55-4EB9-9049-63BB65AC980A}.{6E8698D0-4CFA-4ACB-8AA3-26476F490228}");

                Assert.Fail();
            }
            catch (ValidationException)
            {
            }
        }

        [Test]
        public void Validation_DirectoryExists()
        {
            var sample = new ValidationSample_01();
            Execute(sample, @"directoryexists /path=c:\windows\system32");
            Execute(sample, @"directoryexists /path=%WINDIR%\system32");

            Execute(sample, @"uridirectoryexists /path=c:\windows\system32");
            Execute(sample, @"uridirectoryexists /path=%WINDIR%\system32");

            try
            {
                Execute(sample, @"directoryexists /path=y:\{B2C97314-4C55-4EB9-9049-63BB65AC980A}.{6E8698D0-4CFA-4ACB-8AA3-26476F490228}");

                Assert.Fail();
            }
            catch (ValidationException)
            {
            }

            try
            {
                Execute(sample, @"uridirectoryexists /path=y:\{B2C97314-4C55-4EB9-9049-63BB65AC980A}.{6E8698D0-4CFA-4ACB-8AA3-26476F490228}");

                Assert.Fail();
            }
            catch (ValidationException)
            {
            }
        }

        [Test]
        public void Validation_PathExists()
        {
            var sample = new ValidationSample_01();
            Execute(sample, @"pathexists /path=c:\windows\system32");
            Execute(sample, @"pathexists /path=%WINDIR%\system32");
            Execute(sample, @"pathexists /path=c:\windows\system32\cmd.exe");
            Execute(sample, @"pathexists /path=%WINDIR%\system32\cmd.exe");

            Execute(sample, @"uripathexists /path=c:\windows\system32\cmd.exe");
            Execute(sample, @"uripathexists /path=%WINDIR%\system32\cmd.exe");
            Execute(sample, @"uripathexists /path=c:\windows\system32");
            Execute(sample, @"uripathexists /path=%WINDIR%\system32");

            try
            {
                Execute(sample, @"pathexists /path=y:\{B2C97314-4C55-4EB9-9049-63BB65AC980A}.{6E8698D0-4CFA-4ACB-8AA3-26476F490228}");

                Assert.Fail();
            }
            catch (ValidationException)
            {
            }

            try
            {
                Execute(sample, @"uripathexists /path=y:\{B2C97314-4C55-4EB9-9049-63BB65AC980A}.{6E8698D0-4CFA-4ACB-8AA3-26476F490228}");

                Assert.Fail();
            }
            catch (ValidationException)
            {
            }
        }
    }
}