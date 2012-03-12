using System;
using System.Collections.Generic;
using System.Reflection;
using CLAP;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class UtilsTests
    {
        private static Type GetUtils()
        {
            return typeof(Parser).Assembly.GetType("CLAP.Utils");
        }

        private static object InvokeUtils(string method, params object[] args)
        {
            var utils = GetUtils();

            return utils.InvokeMember(
                method,
                BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                null, null, args);
        }

        [Test]
        public void GetGenericTypeName_Test()
        {
            Assert.AreEqual("List<Int32>", InvokeUtils("GetGenericTypeName", typeof(List<Int32>)));
            Assert.AreEqual("Dictionary<String,IEnumerable<Uri>>", InvokeUtils("GetGenericTypeName", typeof(Dictionary<string, IEnumerable<Uri>>)));
        }
    }
}