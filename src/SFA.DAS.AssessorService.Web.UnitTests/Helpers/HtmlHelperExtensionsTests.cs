using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Extensions;
using System.Collections;

namespace SFA.DAS.AssessorService.Web.UnitTests.Helpers
{
    [TestFixture]
    public class HtmlHelperExtensionsTests
    { 
        private string expectedOutput;
        private string actualOutput;      

        [Test]
        public void WhenICallSetZenDeskLabelsWithOutLabel_ThenTheOutputIsCorrect()
        {
            //Arrange            
            expectedOutput =
                $"<script type=\"text/javascript\">zE('webWidget', 'helpCenter:setSuggestions', {{ labels: [] }});</script>";

            //Act
            actualOutput = HtmlHelperExtensions.SetZenDeskLabels(null).ToString();

            //Assert
            Assert.AreEqual(expectedOutput, actualOutput);
        }      

        [TestCaseSource(typeof(StringArrayTestDataSource))]
        public void WhenICallSetZenDeskLabelsWithLabels_ThenTheOutputIsCorrect(string[] labels)
        {
            //Arrange
            var actualOutput = "<script type=\"text/javascript\">zE('webWidget', 'helpCenter:setSuggestions', { labels: [";

            var first = true;
            foreach (var label in labels)
            {
                if (!string.IsNullOrEmpty(label))
                {
                    if (!first) actualOutput += ",";
                    first = false;

                    actualOutput += $"'{ EscapeApostrophes(label) }'";
                }
            }

            actualOutput += "] });</script>";

            //Act
            this.actualOutput = HtmlHelperExtensions.SetZenDeskLabels(null, labels).ToString();

            //Assert
            Assert.AreEqual(actualOutput, this.actualOutput);
        }

        private static string EscapeApostrophes(string input)
        {
            return input.Replace("'", @"\'");
        }

    }

    public class StringArrayTestDataSource : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {            
            yield return new string[] { "a string with multiple words", "the title of another page" };
            yield return new string[] { "ass-dashboard" };
            yield return new string[] { null };
        }
    }
}
