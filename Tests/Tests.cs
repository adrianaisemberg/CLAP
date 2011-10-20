﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            Parser.Run(cmd.Split(' '), obj);
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
        [ExpectedException(typeof(MissingArgumentValueException))]
        public void Execute_NoParameterValue()
        {
            var printer = new Printer();
            var sample = new Sample_02 { Printer = printer };

            Execute(sample, "print /c=5 /msg= /prefix=hello_");
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

            var p = new Parser<Sample_02>();

            // with and without description for coverage
            //
            p.RegisterParameterHandler("dec", delegate { x--; });
            p.RegisterParameterHandler("inc", delegate { x++; }, "description");

            p.Run("print /c=5 /msg=test /prefix=hello_ /inc".Split(' '), sample);

            Assert.AreEqual(1, x);

            p.Run("print /c=5 /msg=test /prefix=hello_ /dec".Split(' '), sample);

            Assert.AreEqual(0, x);
        }

        [Test]
        public void RegisterParameterHandler_CallsTheHandler_UseTheValue()
        {
            var printer = new Printer();
            var sample = new Sample_02 { Printer = printer };

            var debug = String.Empty;

            var p = new Parser<Sample_02>();
            p.RegisterParameterHandler<string>("debug", str => debug = str);

            p.Run("print /c=5 /msg=test /prefix=hello_ /debug=true".Split(' '), sample);

            Assert.AreEqual("true", debug);
        }

        [Test]
        public void Global_Defined_WithArg()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_10 { Printer = mock.Object };

            var p = new Parser<Sample_10>();

            p.Run("print -foo:blah".Split(' '), sample);

            mock.Verify(o => o.Print("blah"));
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void Global_Defined_MoreThanOneParameter_Exception()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_12 { Printer = mock.Object };

            var p = new Parser<Sample_12>();

            p.Run("print -foo".Split(' '), sample);
        }

        [Test]
        public void Global_Defined_NoArgs()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_10 { Printer = mock.Object };

            var p = new Parser<Sample_10>();

            p.Run("print -bar".Split(' '), sample);

            mock.Verify(o => o.Print("zoo"));
        }

        [Test]
        public void Global_Defined_WithValidation()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_10 { Printer = mock.Object };

            var p = new Parser<Sample_10>();

            p.Run("print -mish:5".Split(' '), sample);

            mock.Verify(o => o.Print("mesh"), Times.Exactly(5));
        }

        [Test]
        [ExpectedException(typeof(TypeConvertionException))]
        public void Global_Defined_BadConvertion()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_10 { Printer = mock.Object };

            var p = new Parser<Sample_10>();

            p.Run("print -abra:cadabra".Split(' '), sample);
        }

        [Test]
        public void Help_Defined()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_10 { Printer = mock.Object };

            var p = new Parser<Sample_10>();

            p.Run("-showhelp".Split(' '), sample);
            p.Run("showhelp".Split(' '), sample);

            mock.Verify(o => o.Print("help"), Times.Exactly(2));
        }

        [Test]
        public void Help_Registered()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_11 { Printer = mock.Object };

            var p = new Parser<Sample_11>();

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

            var p = new Parser<Sample_09>();

            p.Run(new string[] { }, sample);

            mock.Verify(o => o.Print("a"));
        }

        [Test]
        public void EmptyHandler_Static_CalledWhenNoArgs()
        {
            var mock = new Mock<IPrinter>();

            Sample_16.StaticPrinter = mock.Object;

            var sample = new Sample_16();

            var p = new Parser<Sample_16>();

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

            var p = new Parser<Sample_20>();

            p.Run(new string[] { }, sample);
        }

        [Test]
        public void Empty_Registered()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_11 { Printer = mock.Object };

            var p = new Parser<Sample_11>();

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

            var p = new Parser<Sample_13>();

            p.Run(new string[] { }, sample);
        }

        [Test]
        [ExpectedException(typeof(ParserExecutionTargetException))]
        public void Empty_Defined_Static_TargetNotNull_Exception()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_14 { Printer = mock.Object };

            var p = new Parser<Sample_14>();

            p.Run(new string[] { }, sample);
        }

        [Test]
        [ExpectedException(typeof(ParserExecutionTargetException))]
        public void Empty_Defined_NotStatic_TargetNull_Exception()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_15 { Printer = mock.Object };

            var p = new Parser<Sample_15>();

            p.Run(new string[] { });
        }

        [Test]
        public void EmptyHelp_Defined_Called()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_17 { Printer = mock.Object };

            var p = new Parser<Sample_17>();

            p.Run(new string[] { }, sample);

            mock.Verify(o => o.Print("a"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentMismatchException))]
        public void EmptyHelp_Defined_IntParameter_Exception()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_18 { Printer = mock.Object };

            var p = new Parser<Sample_18>();

            p.Run(new string[] { }, sample);
        }

        [Test]
        [ExpectedException(typeof(ArgumentMismatchException))]
        public void EmptyHelp_Defined_TwoParameters_Exception()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_19 { Printer = mock.Object };

            var p = new Parser<Sample_19>();

            p.Run(new string[] { }, sample);
        }

        [Test]
        public void Help_WithAliases()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_21 { Printer = mock.Object };

            var p = new Parser<Sample_21>();

            p.Run(new[] { "help" }, sample);
            p.Run(new[] { "h" }, sample);
            p.Run(new[] { "?" }, sample);
            p.Run(new[] { "-help" }, sample);
            p.Run(new[] { "-h" }, sample);
            p.Run(new[] { "-?" }, sample);

            mock.Verify(o => o.Print("help"), Times.Exactly(6));
        }

        [Test]
        public void _Help_WithEverything_Coverage()
        {
            var p = new Parser<Sample_10>();

            p.RegisterParameterHandler("param", delegate { }, "description");

            p.GetHelpString();
        }

        [Test]
        [ExpectedException(typeof(ParserExecutionTargetException))]
        public void Help_Static_CalledWithTarget_Exception()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_22 { Printer = mock.Object };

            var p = new Parser<Sample_22>();

            p.Run(new[] { "-?" }, sample);
        }

        [Test]
        [ExpectedException(typeof(ParserExecutionTargetException))]
        public void Help_NonStatic_CalledWithNull_Exception()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_23 { Printer = mock.Object };

            var p = new Parser<Sample_23>();

            p.Run(new[] { "-?" });
        }

        [Test]
        public void Help_Static()
        {
            var mock = new Mock<IPrinter>();

            Sample_22.StaticPrinter = mock.Object;

            var sample = new Sample_22 { Printer = mock.Object };

            var p = new Parser<Sample_22>();

            p.Run(new[] { "-?" });

            Sample_22.StaticPrinter = null;
        }

        [Test]
        public void Help_NonStatic()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_23 { Printer = mock.Object };

            var p = new Parser<Sample_23>();

            p.Run(new[] { "-?" }, sample);

            mock.Verify(o => o.Print("help"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentMismatchException))]
        public void Help_MoreThanOneParameter_Exception()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_24 { Printer = mock.Object };

            var p = new Parser<Sample_24>();

            p.Run(new[] { "-?" }, sample);
        }

        [Test]
        [ExpectedException(typeof(ArgumentMismatchException))]
        public void Help_IntParameter_Exception()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_25 { Printer = mock.Object };

            var p = new Parser<Sample_25>();

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
        [ExpectedException(typeof(MoreThanOneEmptyHandlerException))]
        public void RegisterHelpHandler_MoreThanOnce_Exception()
        {
            var p = new Parser<Sample_25>();

            p.RegisterEmptyHandler(delegate { });
            p.RegisterEmptyHandler(delegate { });
        }

        [Test]
        public void RegisterEmptyHelpHandler_Called()
        {
            var p = new Parser<Sample_25>();

            string help = null;

            p.RegisterEmptyHelpHandler(h => help = h);

            Assert.IsNull(help);

            p.Run(new string[] { });

            Assert.IsNotNull(help);
        }

        [Test]
        [ExpectedException(typeof(MoreThanOneEmptyHandlerException))]
        public void RegisterEmptyHelpHandler_MoreThanOnce_Exception()
        {
            var p = new Parser<Sample_25>();

            p.RegisterEmptyHelpHandler(delegate { });
            p.RegisterEmptyHelpHandler(delegate { });
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RegisterHelpHandler_MoreThanOnce_SameKey_Exception()
        {
            var p = new Parser<Sample_25>();

            p.RegisterHelpHandler("a", delegate { });
            p.RegisterHelpHandler("b", delegate { });
            p.RegisterHelpHandler("a", delegate { });
        }

        [Test]
        [ExpectedException(typeof(MissingVerbException))]
        public void Run_Verb_NoMatchingMethod_Exception()
        {
            var p = new Parser<Sample_25>();

            p.Run(new[] { "boo!" });
        }

        [Test]
        [ExpectedException(typeof(MissingDefaultVerbException))]
        public void Run_NoVerb_NoDefaultVerb_Exception()
        {
            var p = new Parser<Sample_25>();

            p.Run(new string[] { "-x" });
        }

        [Test]
        public void GenericParser_Run()
        {
            var mock = new Mock<IPrinter>();

            Sample_27.StaticPrinter = mock.Object;

            Parser.Run<Sample_27>("foo -x:bar".Split(' '));

            Sample_27.StaticPrinter = null;

            mock.Verify(o => o.Print("bar"));
        }

        [Test]
        public void Parse_GuidParameter()
        {
            var mock = new Mock<IPrinter>();

            var sample = new Sample_28 { Printer = mock.Object };

            Parser.Run("foo -x:string -g:{0813A561-AC86-4C82-8EB1-0B6814637C7C}".Split(' '), sample);

            mock.Verify(o => o.Print("string0813A561-AC86-4C82-8EB1-0B6814637C7C".ToLower()));
        }

        [Test]
        public void Parse_GuidParameter_NoInput()
        {
            var mock = new Mock<IPrinter>();

            var sample = new Sample_28 { Printer = mock.Object };

            Parser.Run("foo -x:string".Split(' '), sample);

            mock.Verify(o => o.Print("string" + Guid.Empty));
        }

        [Test]
        public void Parse_GuidParameter_WithDefault()
        {
            var mock = new Mock<IPrinter>();

            var sample = new Sample_28 { Printer = mock.Object };

            Parser.Run("bar -x:string".Split(' '), sample);

            mock.Verify(o => o.Print("string2FBBAAAA-02AF-4F40-BADE-957F566B221E".ToLower()));
        }

        [Test]
        public void GetHelp_GuidParameter()
        {
            var mock = new Mock<IPrinter>();

            var sample = new Sample_28 { Printer = mock.Object };

            // should not fail. I don't care what the help string is
            //
            Parser.Run(new string[0], sample);
        }

        [Test]
        public void Default_IntAsString()
        {
            var mock = new Mock<IPrinter>();

            var sample = new Sample_28 { Printer = mock.Object };

            Parser.Run(new[] { "zoo" }, sample);

            mock.Verify(o => o.Print("5"));
        }

        [Test]
        public void Parse_UriParameter()
        {
            var mock = new Mock<IPrinter>();

            var sample = new Sample_29 { Printer = mock.Object };

            Parser.Run("foo -x:string -u:http://www.com".Split(' '), sample);

            mock.Verify(o => o.Print("stringhttp://www.com/".ToLower()));
        }

        [Test]
        public void Parse_UriParameter_NoInput()
        {
            var mock = new Mock<IPrinter>();

            var sample = new Sample_29 { Printer = mock.Object };

            Parser.Run("foo -x:string".Split(' '), sample);

            mock.Verify(o => o.Print("string"));
        }

        [Test]
        public void Parse_UriParameter_WithDefault()
        {
            var mock = new Mock<IPrinter>();

            var sample = new Sample_29 { Printer = mock.Object };

            Parser.Run("bar -x:string".Split(' '), sample);

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

        [Test]
        public void Unhandled_Exception_One()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_02 { Printer = mock.Object };

            var p = new Parser<Sample_02>();

            try
            {
                p.Run(new[]
                {
                    "-count:1",
                    "-message:a",
                    "-prefix:p",
                    "-upper",
                    "-what:x"
                }, sample);

                Assert.Fail();
            }
            catch (UnhandledParametersException ex)
            {
                Assert.AreEqual(1, ex.UnhandledParameters.Count());
                Assert.AreEqual("what", ex.UnhandledParameters.First().Key);
                Assert.AreEqual("x", ex.UnhandledParameters.First().Value);
            }
        }

        [Test]
        public void Unhandled_Exception_Some()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_02 { Printer = mock.Object };

            var p = new Parser<Sample_02>();

            try
            {
                p.Run(new[]
                {
                    "-count:1",
                    "-who:me",
                    "-foo:bar",
                    "-message:a",
                    "-prefix:p",
                    "-upper",
                    "-what:x"
                }, sample);

                Assert.Fail();
            }
            catch (UnhandledParametersException ex)
            {
                Assert.AreEqual(3, ex.UnhandledParameters.Count());
                Assert.IsTrue(ex.UnhandledParameters.Keys.Contains("who"));
                Assert.IsTrue(ex.UnhandledParameters.Keys.Contains("foo"));
                Assert.IsTrue(ex.UnhandledParameters.Keys.Contains("what"));
            }
        }

        [Test]
        public void Execute_DefaultVerb_NoInput()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_30 { Printer = mock.Object };

            Parser.Run(new string[] { }, sample);

            mock.Verify(o => o.Print("works!"));
        }

        [Test]
        public void Execute_DefaultVerb_NoInput_MethodWithParameters()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_31 { Printer = mock.Object };

            Parser.Run(new string[] { }, sample);

            mock.Verify(o => o.Print("works!"));
        }

        [Test]
        public void Execute_HandleError_Registered_NoRethrow()
        {
            var p = new Parser<Sample_39>();
            var handled = false;

            p.RegisterErrorHandler(ex =>
            {
                handled = true;
            }, false);

            p.Run(new string[] { });

            Assert.IsTrue(handled);
        }

        [Test]
        public void Execute_HandleError_Registered_DefaultNoRethrow()
        {
            var p = new Parser<Sample_39>();
            var handled = false;

            p.RegisterErrorHandler(ex =>
            {
                handled = true;
            });

            p.Run(new string[] { });

            Assert.IsTrue(handled);
        }

        [Test]
        public void Execute_HandleError_Registered_Rethrow()
        {
            var p = new Parser<Sample_39>();
            var handled = false;

            p.RegisterErrorHandler(ex =>
            {
                handled = true;
            }, true);

            try
            {
                p.Run(new string[] { });

                Assert.Fail();
            }
            catch (Exception)
            {
                Assert.IsTrue(handled);
            }
        }

        [Test]
        public void Execute_HandleError_Registered_UnhandledParameters()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_02 { Printer = mock.Object };

            var p = new Parser<Sample_02>();

            var handled = false;

            p.RegisterErrorHandler(ex =>
            {
                handled = true;
            }, false);

            p.Run(new[]
            {
                "-count:1",
                "-message:a",
                "-prefix:p",
                "-upper",
                "-what:x"
            }, sample);

            Assert.IsTrue(handled);
        }

        [Test]
        public void Execute_HandleError_Registered_ValidationError()
        {
            var sample = new ValidationSample_01();

            var p = new Parser<ValidationSample_01>();
            var handled = false;

            p.RegisterErrorHandler(ex =>
            {
                handled = true;
            }, false);

            p.Run(new[] { "morethan5", "/n=1" }, sample);

            Assert.IsTrue(handled);
        }

        [Test]
        [ExpectedException(typeof(MoreThanOneErrorHandlerException))]
        public void Error_MoreThanOne_Exception()
        {
            var mock = new Mock<IPrinter>();
            var sample = new Sample_32 { Printer = mock.Object };

            var p = new Parser<Sample_32>();

            p.Run(new string[] { }, sample);
        }

        [Test]
        [ExpectedException(typeof(MoreThanOneErrorHandlerException))]
        public void RegisterError_MoreThanOne_Exception()
        {
            var p = new Parser<Sample_02>();

            p.RegisterErrorHandler(delegate { });
            p.RegisterErrorHandler(delegate { });
        }

        [Test]
        [ExpectedException(typeof(ArgumentMismatchException))]
        public void Error_DefinedWithInt_Exception()
        {
            Parser.Run<Sample_35>(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentMismatchException))]
        public void Error_DefinedWithBadException_Exception()
        {
            Parser.Run<Sample_36>(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentMismatchException))]
        public void Error_DefinedWithMoreThanOneParameter_Exception()
        {
            Parser.Run<Sample_37>(null);
        }

        [Test]
        public void Error_Handled_1()
        {
            var sample = new Sample_33();

            Assert.IsNull(sample.Ex);

            try
            {
                Parser.Run(new[] { "foo1" }, sample);

                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(sample.Ex);
                Assert.AreEqual(ex, sample.Ex);
            }
        }

        [Test]
        public void Error_Handled_2()
        {
            var sample = new Sample_33();

            Assert.IsNull(sample.Ex);

            try
            {
                Parser.Run(new[] { "foo" }, sample);

                Assert.Fail();
            }
            catch (TargetInvocationException tex)
            {
                Assert.IsNotNull(sample.Ex);
                Assert.AreEqual(tex.InnerException, sample.Ex);
            }
            catch (Exception ex)
            {
                Assert.IsNotNull(sample.Ex);
                Assert.AreEqual(ex, sample.Ex);
            }
        }

        [Test]
        public void Error_Handled_Empty()
        {
            var sample = new Sample_34();

            Assert.IsFalse(sample.Handled);

            try
            {
                Parser.Run(new[] { "foo" }, sample);

                Assert.Fail();
            }
            catch
            {
                Assert.IsTrue(sample.Handled);
            }
        }

        [Test]
        public void _Validation_Descriptions_Coverage()
        {
            var p = new Parser<All_Validations_Sample>();

            p.GetHelpString();
        }

        [Test]
        [ExpectedException(typeof(MoreThanOneDefaultVerbException))]
        public void DefaultVerb_MoreThanOnce_Exception()
        {
            new Parser<Sample_38>();
        }

        [Test]
        public void ValidateExpression_TwoInts_Pass()
        {
            Parser.Run<Sample_40>(new[]
            {   
                "foo1",
                "-p1:5",
                "-p2:3",
            });
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void ValidateExpression_TwoInts_Fail()
        {
            Parser.Run<Sample_40>(new[]
            {
                "foo1",
                "-p1:1",
                "-p2:3",
            });
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void ValidateExpression_TwoInts_TwoValidators_Fail()
        {
            Parser.Run<Sample_40>(new[]
            {
                "foo2",
                "-p1:8",
                "-p2:3",
            });
        }

        [Test]
        public void ValidateExpression_TwoInts_TwoValidators_Pass()
        {
            Parser.Run<Sample_40>(new[]
            {
                "foo2",
                "-p1:11",
                "-p2:3",
            });
        }

        [Test]
        public void ValidateExpression_String_Pass()
        {
            Parser.Run<Sample_40>(new[]
            {
                "foo3",
                "-str:def",
            });
        }

        [Test]
        public void ValidateExpression_String_DifferentCase_Pass()
        {
            Parser.Run<Sample_40>(new[]
            {
                "foo3",
                "-str:DEF",
            });
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void ValidateExpression_String_DifferentCase_Fail()
        {
            Parser.Run<Sample_40>(new[]
            {
                "foo5",
                "-str:DEF",
            });
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void ValidateExpression_String_Like_Fail()
        {
            Parser.Run<Sample_40>(new[]
            {
                "foo4",
                "-str:blah",
            });
        }

        [Test]
        public void ValidateExpression_String_Like_Pass()
        {
            Parser.Run<Sample_40>(new[]
            {
                "foo4",
                "-str:blahfoo",
            });
        }

        [Test]
        public void ValidateExpression_Global_Pass()
        {
            Parser.Run<Sample_40>(new[]
            {
                "dummy",
                "-boing:20",
            });
        }

        [Test]
        [ExpectedException(typeof(ValidationException))]
        public void ValidateExpression_Global_Fail()
        {
            Parser.Run<Sample_40>(new[]
            {
                "dummy",
                "-boing:5",
            });
        }

        [Test]
        public void InterceptorTest()
        {
            const string method1 = "foo1";
            const string method2 = "foo2";
            const string arg1 = "argument1";
            const string arg2 = "argument2";

            var sample = new InterceptorSample();

            Parser.Run(new[] { method1, "-str:" + arg1 }, sample);
            Parser.Run(new[] { method2, "-str:" + arg2 }, sample);

            Assert.AreEqual(2, sample.Intercepted.Count());
            Assert.AreEqual(method1, sample.Intercepted[0]);
            Assert.AreEqual(method2, sample.Intercepted[1]);

            Assert.AreEqual(2, sample.Invoked.Count());
            Assert.AreEqual("Foo1:" + arg1, sample.Invoked[0]);
            Assert.AreEqual("Foo2:" + arg2, sample.Invoked[1]);
        }

        [Test]
        public void MultipleInterceptorTest()
        {
            var sample = new MultipleInterceptorSample();

            Assert.Throws<MoreThanOneVerbInterceptorException>(() => Parser.Run(new[] { "foo" }, sample));
        }

        [Test]
        public void InvalidInterceptorTest()
        {
            var sample = new InvalidInterceptorSample();

            Assert.Throws<InvalidVerbInterceptorException>(() => Parser.Run(new[] { "foo", "-str:something" }, sample));
        }

        [Test]
        public void InheritedInterceptorTest()
        {
            const string method1 = "foo";
            const string arg1 = "argument1";

            var sample = new VerbWithInterceptorBaseSample();

            Parser.Run(new[] { method1, "-str:" + arg1 }, sample);

            Assert.AreEqual(1, sample.Intercepted.Count());
            Assert.AreEqual(method1, sample.Intercepted[0]);

            Assert.AreEqual(1, sample.Invoked.Count());
            Assert.AreEqual("Foo:" + arg1, sample.Invoked[0]);
        }

        [Test]
        public void InterceptorOnInheritedVerbTest()
        {
            const string method1 = "foo";
            const string arg1 = "argument1";

            var sample = new InterceptorWithVerbBaseSample();

            Parser.Run(new[] { method1, "-str:" + arg1 }, sample);

            Assert.AreEqual(1, sample.Intercepted.Count());
            Assert.AreEqual(method1, sample.Intercepted[0]);

            Assert.AreEqual(1, sample.Invoked.Count());
            Assert.AreEqual("Foo:" + arg1, sample.Invoked[0]);
        }

        [Test]
        public void RegisteredVerbInterceptorTest()
        {
            var printer = new Printer();
            var sample = new Sample_02 { Printer = printer };

            var list = new List<string>();
            var p = new Parser<Sample_02>();
            p.RegisterInterceptor(x => list.Add(x.Verb));

            p.Run("print /c=5 /msg=test /prefix=hello_".Split(' '), sample);
            Assert.AreEqual(1, list.Count());
            Assert.AreEqual("print", list[0]);
        }

    }
}