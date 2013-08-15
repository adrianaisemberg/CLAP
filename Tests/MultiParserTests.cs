using System;
using System.Linq;
using System.Reflection;
using CLAP;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class MultiParserTests
    {
        private IMethodInvoker DefaultMethodInvoker;

        [SetUp]
        public void SetUp()
        {
            DefaultMethodInvoker = MethodInvoker.Invoker;
        }

        [TearDown]
        public void TearDown()
        {
            MethodInvoker.Invoker = DefaultMethodInvoker;
        }

        [Test]
        public void MultiParser_Run_HappyFlow_1()
        {
            var mock = new MethodInvokerMock();

            var called = false;

            mock.Action = (method, obj, parameters) =>
            {
                called = true;

                Assert.IsTrue(method.Name == "Print");
                Assert.IsTrue(method.DeclaringType == typeof(Sample_02));
                Assert.IsTrue(parameters.Contains(10));
                Assert.IsTrue(parameters.Contains("aaa"));
            };

            MethodInvoker.Invoker = mock;

            var p = new Parser<Sample_02, Sample_03>();

            p.Run(new[]
            {
                "sample_02.print",
                "-c=10",
                "-prefix=aaa",
            }, new Sample_02(), new Sample_03());

            Assert.IsTrue(called);
        }

        [Test]
        public void MultiParser_Run_HappyFlow_2()
        {
            var mock = new MethodInvokerMock();

            var called = false;

            mock.Action = (method, obj, parameters) =>
            {
                called = true;

                Assert.IsTrue(method.Name == "Print");
                Assert.IsTrue(method.DeclaringType == typeof(Sample_02));
                Assert.IsTrue(parameters.Contains(10));
                Assert.IsTrue(parameters.Contains("aaa"));
            };

            MethodInvoker.Invoker = mock;

            var p = new Parser(typeof(Sample_02), typeof(Sample_03));

            p.RunTargets(new[]
            {
                "sample_02.print",
                "-c=10",
                "-prefix=aaa",
            }, new Sample_02(), new Sample_03());

            Assert.IsTrue(called);
        }

        [Test]
        public void MultiParser_Run_With_TargetResolver()
        {
            var resolver = new TargetResolver();
            resolver.RegisterTargetType(() => new Sample_02());
            resolver.RegisterTargetType(() => new Sample_03());

            var mock = new MethodInvokerMock();

            var called = false;
            mock.Action = (method, obj, parameters) =>
                {
                    called = true;
                    Assert.IsTrue(method.Name == "Print");
                    Assert.IsTrue(obj.GetType().Name == "Sample_03");
                };

            MethodInvoker.Invoker = mock;

            Parser.Run(new[]
                {
                    "sample_03.print",
                    "-c=8",
                    "-prefix=xyz"
                }, resolver);

            Assert.IsTrue(called);
        }

        [Test]
        public void MultiParser_Run_With_Target_Alias()
        {
            var mock = new MethodInvokerMock();

            var called = false;

            mock.Action = (method, obj, parameters) =>
            {
                called = true;

                Assert.IsTrue(method.Name == "Print");
                Assert.IsTrue(method.DeclaringType == typeof(Sample_03));
                Assert.IsTrue(parameters.Contains(10));
                Assert.IsTrue(parameters.Contains("aaa"));
            };

            MethodInvoker.Invoker = mock;

            var p = new Parser(typeof(Sample_02), typeof(Sample_03));

            p.RunTargets(new[]
            {
                "s03.print", //the Sample_03 class has an alias attribute of 's03'
                "-c=10",
                "-prefix=aaa",
            }, new Sample_02(), new Sample_03());

            Assert.IsTrue(called);
        }

        [Test]
        public void MultiParser_Run_OneParser()
        {
            var mock = new MethodInvokerMock();

            var called = false;

            mock.Action = (method, obj, parameters) =>
            {
                called = true;

                Assert.IsTrue(method.Name == "Print");
                Assert.IsTrue(method.DeclaringType == typeof(Sample_03));
                Assert.IsTrue(parameters.Contains(10));
                Assert.IsTrue(parameters.Contains("aaa"));
            };

            MethodInvoker.Invoker = mock;

            var p = new Parser<Sample_03>();

            p.Run(new[]
            {
                "sample_03.print",
                "-c=10",
                "-prefix=aaa",
            }, new Sample_03());

            Assert.IsTrue(called);
        }

        [Test]
        [ExpectedException(typeof(MissingVerbException))]
        public void MultiParser_Run_NoVerb_Exception()
        {
            var p = new Parser<Sample_03, Sample_02>();

            p.Run(new[]
            {
                "-c=10",
                "-prefix=aaa",
            }, new Sample_03(), new Sample_02());
        }

        [Test]
        [ExpectedException(typeof(MultiParserMissingClassNameException))]
        public void MultiParser_Run_NoDelimiter_Exception_1()
        {
            var p = new Parser<Sample_03, Sample_02>();

            p.Run(new[]
            {
                "sample02print",
                "-c=10",
                "-prefix=aaa",
            }, new Sample_03(), new Sample_02());
        }

        [Test]
        [ExpectedException(typeof(MultiParserMissingClassNameException))]
        public void MultiParser_Run_NoDelimiter_Exception_2()
        {
            var p = new Parser<Sample_03, Sample_02>();

            p.Run(new[]
            {
                "print",
                "-c=10",
                "-prefix=aaa",
            }, new Sample_03(), new Sample_02());
        }

        [Test]
        [ExpectedException(typeof(InvalidVerbException))]
        public void MultiParser_Run_TooMuchDelimiters_Exception_1()
        {
            var p = new Parser<Sample_03>();

            p.Run(new[]
            {
                "sample_03.print.foo",
                "-c=10",
                "-prefix=aaa",
            }, new Sample_03());
        }

        [Test]
        [ExpectedException(typeof(InvalidVerbException))]
        public void MultiParser_Run_TooMuchDelimiters_Exception_2()
        {
            var p = new Parser<Sample_03, Sample_02>();

            p.Run(new[]
            {
                "sample_03.print.foo",
                "-c=10",
                "-prefix=aaa",
            }, new Sample_03(), new Sample_02());
        }

        [Test]
        [ExpectedException(typeof(UnknownParserTypeException))]
        public void MultiParser_Run_MissingType_Exception_1()
        {
            var p = new Parser<Sample_03>();

            p.Run(new[]
            {
                "sample_03foo.print",
                "-c=10",
                "-prefix=aaa",
            }, new Sample_03());
        }

        [Test]
        [ExpectedException(typeof(UnknownParserTypeException))]
        public void MultiParser_Run_MissingType_Exception_2()
        {
            var p = new Parser<Sample_03, Sample_02>();

            p.Run(new[]
            {
                "sample_03foo.print",
                "-c=10",
                "-prefix=aaa",
            }, new Sample_03(), new Sample_02());
        }

        [Test]
        [ExpectedException(typeof(MissingRequiredArgumentException))]
        public void MultiParser_Run_MissingRequiredArgument()
        {
            var mock = new MethodInvokerMock();

            var called = false;

            mock.Action = (method, obj, parameters) =>
            {
                called = true;

                Assert.IsTrue(method.Name == "Print");
                Assert.IsTrue(method.DeclaringType == typeof(Sample_07));
                Assert.IsTrue(parameters.Contains(10));
            };

            MethodInvoker.Invoker = mock;

            var p = new Parser<Sample_02, Sample_03, Sample_04, Sample_06, Sample_07>();

            p.Run(new[]
            {
                "sample_07.print",
                "-count=10",
            },
            new Sample_02(),
            new Sample_03(),
            new Sample_04(),
            new Sample_06(),
            new Sample_07());

            Assert.IsTrue(called);
        }

        [Test]
        public void MultiParser_Run_HappyFlow_1_Type()
        {
            var mock = new MethodInvokerMock();

            var called = false;

            mock.Action = (method, obj, parameters) =>
            {
                called = true;

                Assert.IsTrue(method.Name == "Print");
                Assert.IsTrue(method.DeclaringType == typeof(Sample_02));
                Assert.IsTrue(parameters.Contains(10));
                Assert.IsTrue(parameters.Contains("aaa"));
            };

            MethodInvoker.Invoker = mock;

            var p = new Parser<Sample_02>();

            p.Run(new[]
            {
                "sample_02.print",
                "-c=10",
                "-prefix=aaa",
            }, new Sample_02());

            Assert.IsTrue(called);
        }

        [Test]
        public void MultiParser_Run_HappyFlow_2_Types()
        {
            var mock = new MethodInvokerMock();

            var called = false;

            mock.Action = (method, obj, parameters) =>
            {
                called = true;

                Assert.IsTrue(method.Name == "Print");
                Assert.IsTrue(method.DeclaringType == typeof(Sample_02));
                Assert.IsTrue(parameters.Contains(10));
                Assert.IsTrue(parameters.Contains("aaa"));
            };

            MethodInvoker.Invoker = mock;

            var p = new Parser<Sample_02, Sample_03>();

            p.Run(new[]
            {
                "sample_02.print",
                "-c=10",
                "-prefix=aaa",
            }, new Sample_02(), new Sample_03());

            Assert.IsTrue(called);
        }

        [Test]
        public void MultiParser_Run_HappyFlow_3_Types()
        {
            var mock = new MethodInvokerMock();

            var called = false;

            mock.Action = (method, obj, parameters) =>
            {
                called = true;

                Assert.IsTrue(method.Name == "Print");
                Assert.IsTrue(method.DeclaringType == typeof(Sample_04));
                Assert.IsTrue(parameters.Contains(Case.Upper));
                Assert.IsTrue(parameters.Contains("aaa"));
            };

            MethodInvoker.Invoker = mock;

            var p = new Parser<Sample_02, Sample_03, Sample_04>();

            p.Run(new[]
            {
                "sample_04.print",
                "-c=Upper",
                "-prefix=aaa",
            },
            new Sample_02(),
            new Sample_03(),
            new Sample_04());

            Assert.IsTrue(called);
        }

        [Test]
        public void MultiParser_Run_HappyFlow_4_Types()
        {
            var mock = new MethodInvokerMock();

            var called = false;

            mock.Action = (method, obj, parameters) =>
            {
                called = true;

                Assert.IsTrue(method.Name == "Print");
                Assert.IsTrue(method.DeclaringType == typeof(Sample_06));
                Assert.IsTrue(parameters.Contains(10));
                Assert.IsTrue(parameters.Contains(Case.Lower));
            };

            MethodInvoker.Invoker = mock;

            var p = new Parser<Sample_02, Sample_03, Sample_04, Sample_06>();

            p.Run(new[]
            {
                "sample_06.print",
                "-count=10",
            },
            new Sample_02(),
            new Sample_03(),
            new Sample_04(),
            new Sample_06());

            Assert.IsTrue(called);
        }

        [Test]
        public void MultiParser_Run_HappyFlow_5_Types()
        {
            var mock = new MethodInvokerMock();

            var called = false;

            mock.Action = (method, obj, parameters) =>
            {
                called = true;

                Assert.IsTrue(method.Name == "Print");
                Assert.IsTrue(method.DeclaringType == typeof(Sample_07));
                Assert.IsTrue(parameters.Contains(10));
                Assert.IsTrue(parameters.Contains("a"));
            };

            MethodInvoker.Invoker = mock;

            var p = new Parser<Sample_02, Sample_03, Sample_04, Sample_06, Sample_07>();

            p.Run(new[]
            {
                "sample_07.print",
                "-count=10",
                "-prefix=a",
            },
            new Sample_02(),
            new Sample_03(),
            new Sample_04(),
            new Sample_06(),
            new Sample_07());

            Assert.IsTrue(called);
        }

        [Test]
        public void MultiParser_Run_HappyFlow_6_Types()
        {
            var mock = new MethodInvokerMock();

            var called = false;

            mock.Action = (method, obj, parameters) =>
            {
                called = true;

                Assert.IsTrue(method.Name == "PrintEnums");
                Assert.IsTrue(method.DeclaringType == typeof(Sample_08));
                Assert.IsTrue(parameters.Contains("abc"));
                Assert.IsTrue(parameters.Any(param =>
                {
                    return param.GetType() == typeof(Case[]) &&
                        ((Case[])param).Length == 2 &&
                        ((Case[])param)[0] == Case.Upper &&
                        ((Case[])param)[1] == Case.Lower;
                }));
            };

            MethodInvoker.Invoker = mock;

            var p = new Parser<Sample_02, Sample_03, Sample_04, Sample_06, Sample_07, Sample_08>();

            p.Run(new[]
            {
                "sample_08.PrintEnums",
                "-enums=Upper,Lower",
                "-prefix=abc",
            },
            new Sample_02(),
            new Sample_03(),
            new Sample_04(),
            new Sample_06(),
            new Sample_07(),
            new Sample_08());

            Assert.IsTrue(called);
        }

        [Test]
        public void MultiParser_Run_HappyFlow_7_Types()
        {
            var mock = new MethodInvokerMock();

            var called = false;

            mock.Action = (method, obj, parameters) =>
            {
                called = true;

                Assert.IsTrue(method.Name == "Print");
                Assert.IsTrue(method.DeclaringType == typeof(Sample_07));
                Assert.IsTrue(parameters.Contains(10));
                Assert.IsTrue(parameters.Contains("a"));
            };

            MethodInvoker.Invoker = mock;

            var p = new Parser<Sample_02, Sample_03, Sample_04, Sample_06, Sample_07, Sample_08, Sample_10>();

            p.Run(new[]
            {
                "sample_07.print",
                "-count=10",
                "-prefix=a",
            },
            new Sample_02(),
            new Sample_03(),
            new Sample_04(),
            new Sample_06(),
            new Sample_07(),
            new Sample_08(),
            new Sample_10());

            Assert.IsTrue(called);
        }

        [Test]
        public void MultiParser_Run_HappyFlow_8_Types()
        {
            var mock = new MethodInvokerMock();

            var called = false;

            mock.Action = (method, obj, parameters) =>
            {
                if (called)
                {
                    Assert.IsTrue(method.Name == "Print2");
                    Assert.IsTrue(method.DeclaringType == typeof(Sample_10));
                    Assert.IsTrue(parameters.Contains("fooblah"));
                }
                else
                {
                    Assert.IsTrue(method.Name == "Bar");
                    Assert.IsTrue(method.DeclaringType == typeof(Sample_10));
                }

                called = true;
            };

            MethodInvoker.Invoker = mock;

            var p = new Parser<Sample_02, Sample_03, Sample_04, Sample_06, Sample_07, Sample_08, Sample_10, Sample_12>();

            p.Run(new[]
            {
                "sample_10.print2",
                "-bar",
                "-str=fooblah",
            },
            new Sample_02(),
            new Sample_03(),
            new Sample_04(),
            new Sample_06(),
            new Sample_07(),
            new Sample_08(),
            new Sample_10(),
            new Sample_12());

            Assert.IsTrue(called);
        }

        [Test]
        public void MultiParser_Run_HappyFlow_9_Types()
        {
            var mock = new MethodInvokerMock();

            var called = false;

            mock.Action = (method, obj, parameters) =>
            {
                Assert.IsTrue(method.Name == "Zoo");
                Assert.IsTrue(method.DeclaringType == typeof(Sample_28));
                Assert.IsTrue(parameters.Contains(98));

                called = true;
            };

            MethodInvoker.Invoker = mock;

            var p = new Parser<Sample_02, Sample_03, Sample_04, Sample_06, Sample_07, Sample_08, Sample_10, Sample_12, Sample_28>();

            p.Run(new[]
            {
                "sample_28.zoo",
                "-n=98",
            },
            new Sample_02(),
            new Sample_03(),
            new Sample_04(),
            new Sample_06(),
            new Sample_07(),
            new Sample_08(),
            new Sample_10(),
            new Sample_12(),
            new Sample_28());

            Assert.IsTrue(called);
        }

        [Test]
        public void _Coverage_StaticRun()
        {
            Parser.Run(new[] { "StaticSample_09.Foo" }, new StaticSample_01(), new StaticSample_02(), new StaticSample_03(), new StaticSample_04(), new StaticSample_05(), new StaticSample_06(), new StaticSample_07(), new StaticSample_08(), new StaticSample_09(), new StaticSample_10());

            Parser.Run<StaticSample_09>(new[] { "StaticSample_09.Foo" });
            Parser.Run(new[] { "StaticSample_09.Foo" }, new StaticSample_09());

            Parser.Run<StaticSample_08, StaticSample_09>(new[] { "StaticSample_09.Foo" });
            Parser.Run(new[] { "StaticSample_09.Foo" }, new StaticSample_08(), new StaticSample_09());

            Parser.Run<StaticSample_07, StaticSample_08, StaticSample_09>(new[] { "StaticSample_09.Foo" });
            Parser.Run(new[] { "StaticSample_09.Foo" }, new StaticSample_07(), new StaticSample_08(), new StaticSample_09());

            Parser.Run<StaticSample_06, StaticSample_07, StaticSample_08, StaticSample_09>(new[] { "StaticSample_09.Foo" });
            Parser.Run(new[] { "StaticSample_09.Foo" }, new StaticSample_06(), new StaticSample_07(), new StaticSample_08(), new StaticSample_09());

            Parser.Run<StaticSample_05, StaticSample_06, StaticSample_07, StaticSample_08, StaticSample_09>(new[] { "StaticSample_09.Foo" });
            Parser.Run(new[] { "StaticSample_09.Foo" }, new StaticSample_05(), new StaticSample_06(), new StaticSample_07(), new StaticSample_08(), new StaticSample_09());

            Parser.Run<StaticSample_04, StaticSample_05, StaticSample_06, StaticSample_07, StaticSample_08, StaticSample_09>(new[] { "StaticSample_09.Foo" });
            Parser.Run(new[] { "StaticSample_09.Foo" }, new StaticSample_04(), new StaticSample_05(), new StaticSample_06(), new StaticSample_07(), new StaticSample_08(), new StaticSample_09());

            Parser.Run<StaticSample_03, StaticSample_04, StaticSample_05, StaticSample_06, StaticSample_07, StaticSample_08, StaticSample_09>(new[] { "StaticSample_09.Foo" });
            Parser.Run(new[] { "StaticSample_09.Foo" }, new StaticSample_03(), new StaticSample_04(), new StaticSample_05(), new StaticSample_06(), new StaticSample_07(), new StaticSample_08(), new StaticSample_09());

            Parser.Run<StaticSample_02, StaticSample_03, StaticSample_04, StaticSample_05, StaticSample_06, StaticSample_07, StaticSample_08, StaticSample_09>(new[] { "StaticSample_09.Foo" });
            Parser.Run(new[] { "StaticSample_09.Foo" }, new StaticSample_02(), new StaticSample_03(), new StaticSample_04(), new StaticSample_05(), new StaticSample_06(), new StaticSample_07(), new StaticSample_08(), new StaticSample_09());

            Parser.Run<StaticSample_01, StaticSample_02, StaticSample_03, StaticSample_04, StaticSample_05, StaticSample_06, StaticSample_07, StaticSample_08, StaticSample_09>(new[] { "StaticSample_09.Foo" });
            Parser.Run(new[] { "StaticSample_09.Foo" }, new StaticSample_01(), new StaticSample_02(), new StaticSample_03(), new StaticSample_04(), new StaticSample_05(), new StaticSample_06(), new StaticSample_07(), new StaticSample_08(), new StaticSample_09());
        }
    }

    public class MethodInvokerMock : IMethodInvoker
    {
        public Action<MethodInfo, object, object[]> Action { get; set; }

        public void Invoke(MethodInfo method, object obj, object[] parameters)
        {
            Action(method, obj, parameters);
        }
    }
}