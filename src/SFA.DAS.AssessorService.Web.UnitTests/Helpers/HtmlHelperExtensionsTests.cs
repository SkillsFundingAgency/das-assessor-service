using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Extensions;
using System.Collections;

namespace SFA.DAS.AssessorService.Web.UnitTests.Helpers
{
    [TestFixture]
    public class HtmlHelperExtensionsTests
    { 
        [TestCaseSource(nameof(LabelCases))]
        public void WhenICallSetZenDeskLabelsWithLabels_ThenTheKeywordsAreCorrect(string[] labels, string keywords)
        {
            // Arrange
            var expected = $"<script type=\"text/javascript\">zE('webWidget', 'helpCenter:setSuggestions', {{ labels: [{keywords}] }});</script>";

            // Act
            var actual = HtmlHelperExtensions.SetZenDeskLabels(null, labels).ToString();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        private static readonly object[] LabelCases =
        {
            new object[] { new string[] { "a string with multiple words", "the title of another page" }, "'a string with multiple words','the title of another page'"},
            new object[] { new string[] { "ass-dashboard"}, "'ass-dashboard'"},
            new object[] { new string[] { "ass-apostrophe's" }, @"'ass-apostrophe\'s'"},
            new object[] { new string[] { null }, "''" }
        };
    }
}
