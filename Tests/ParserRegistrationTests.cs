using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CLAP;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class ParserRegistrationTests
    {
        [Test, ExpectedException(typeof(DuplicateTargetAliasException))]
        public void Construct_with_types_that_have_conflicting_alias_names()
        {
            var types = GetBaselineTestTypes().ToList();
            types.Add(typeof(ParserRegTest02_Dupe));

            var SUT = new ParserRegistration(types.ToArray(), GetHelpTextForTest);
            var result = SUT.GetTargetType("test02");
            Assert.IsNull(result);
        }

        [Test]
        public void Find_registered_type_by_type_name()
        {
            var SUT = new ParserRegistration(GetBaselineTestTypes(), GetHelpTextForTest);
            var result = SUT.GetTargetType("ParserRegTest02");
            AssertTypeResult(typeof (ParserRegTest02), result);
        }

        [Test]
        public void Find_Registered_Type_By_Type_Name_Where_Type_Name_Is_Not_Registered()
        {
            var SUT = new ParserRegistration(GetBaselineTestTypes().Where(t => t.Name != "ParserRegTest01").ToArray(), GetHelpTextForTest);
            var result = SUT.GetTargetType("ParserRegTest01");
            AssertTypeResult(null, result);
        }

        [Test]
        public void Find_registered_type_by_name_where_type_name_provided_does_not_match_the_case_of_the_registered_type_name()
        {
            var SUT = new ParserRegistration(GetBaselineTestTypes(), GetHelpTextForTest);
            var result = SUT.GetTargetType("parserregtest02");
            AssertTypeResult(typeof (ParserRegTest02), result);
        }

        [Test]
        public void Find_registered_type_by_alias()
        {
            var SUT = new ParserRegistration(GetBaselineTestTypes(), GetHelpTextForTest);
            var result = SUT.GetTargetType("test03");
            AssertTypeResult(typeof (ParserRegTest03), result);
        }

        private void AssertTypeResult(Type expectedType, Type actualType)
        {
            Assert.AreEqual(expectedType, actualType);
        }

        private string GetHelpTextForTest()
        {
            return "help text for test";
        }

        private Type[] GetBaselineTestTypes()
        {
            return new Type[]
                {
                    typeof (ParserRegTest01),
                    typeof (ParserRegTest02),
                    typeof (ParserRegTest03)
                };
        }
    }

    public class ParserRegTest01
    {
        
    }

    [TargetAlias("Test02")]
    public class ParserRegTest02
    {
        
    }

    [TargetAlias("Test03")]
    public class ParserRegTest03
    {
        
    }

    [TargetAlias("test02")]
    public class ParserRegTest02_Dupe
    {
        
    }
}
