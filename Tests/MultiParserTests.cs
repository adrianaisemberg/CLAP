using System;
using System.Reflection;
using CLAP;
using NUnit.Framework;
using System.Linq;

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
        public void MultiParser_Run_HappyFlow()
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