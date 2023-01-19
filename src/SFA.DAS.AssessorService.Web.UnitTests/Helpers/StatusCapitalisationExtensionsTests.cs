using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Extensions;

namespace SFA.DAS.AssessorService.Web.UnitTests.Helpers
{
    [TestFixture]
    public class StatusCapitalisationExtensionsTests
    {
        [TestCase("In Progress", "In progress")]
        [TestCase("feedback Added", "Feedback added")]
        [TestCase("InProgress", "In progress")]
        [TestCase("feedbackAdded", "Feedback added")]
        [TestCase(" ", " ")]
        [TestCase("", "")]
        [TestCase(null, null)]
        public void When_FormatStatus_Then_CorrectStatusIsReturned(string status, string expected)
        {
            var result = status.FormatStatus();

            Assert.AreEqual(expected, result);
        }
    }
}
