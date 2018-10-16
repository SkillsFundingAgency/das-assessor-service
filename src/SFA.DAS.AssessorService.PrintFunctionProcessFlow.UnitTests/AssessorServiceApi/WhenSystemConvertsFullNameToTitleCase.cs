using System;
using System.Globalization;
using NUnit.Framework;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.UnitTests.AssessorServiceApi
{
    public class WhenSystemConvertsFullNameToTitleCase
    {
        [TestCase(null)]
        [TestCase("TEST STRING")]
        [TestCase("Test sTring")]
        [TestCase("test string")]
        [TestCase("Test String")]
        [TestCase("test string")]
        [TestCase("tEST sSTRING")]
        [TestCase("TeST sSTRING")]
        public void ThenItShouldUpdateCertificates(string firstName)
        {
            var learnerName = "test string";

            var result = firstName != null
                ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(learnerName)
                : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(learnerName);

            Assert.AreEqual(result, "Test String");       
        }
    }
}