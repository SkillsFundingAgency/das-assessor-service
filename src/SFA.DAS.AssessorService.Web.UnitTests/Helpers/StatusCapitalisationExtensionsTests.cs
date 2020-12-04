using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Extensions;

namespace SFA.DAS.AssessorService.Web.UnitTests.Helpers
{
    [TestFixture]
    public class StatusCapitalisationExtensionsTests
    {
        [TestCase("In Progress", "In progress")]
        [TestCase("FeEdbAcK Added", "Feedback added")]
        [TestCase("feedback Added", "Feedback added")]
        [TestCase(" "," ")]
        [TestCase("","")]
        [TestCase(null,null)]
        public void When_CapitaliseFirstLetterOnly_Then_FirstLetterWillBeUpperCaseAndTheRemainingLettersWillBeLowerCase(string status, string expected)
        {
            var result = status.CapitaliseFirstLetterOnly();

            Assert.AreEqual(expected, result);
        }
    }
}
