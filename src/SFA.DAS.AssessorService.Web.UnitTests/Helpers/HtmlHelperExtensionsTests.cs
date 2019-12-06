using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Extensions;

namespace SFA.DAS.AssessorService.Web.UnitTests.Helpers
{
    [TestFixture]
    public class HtmlHelperExtensionsTests
    {        
        private string expectedLabelOne;
        private string expectedLabelTwo;
        private string expectedOutput;
        private string actualOutput;      

        [Test]
        public void WhenICallSetZenDeskLabelsWithMultipleLabelsWithMultipleWordsAndApostrophes_ThenTheOutputIsCorrect()
        {
            //Arrange
            expectedLabelOne = "a string with multiple words";
            expectedLabelTwo = "the title of another page";
            expectedOutput =
                $"<script type=\"text/javascript\">zE('webWidget', 'helpCenter:setSuggestions', {{ labels: ['{expectedLabelOne}','{expectedLabelTwo}'] }});</script>";

            //Act
            actualOutput = HtmlHelperExtensions.SetZenDeskLabels(null, new string[] { expectedLabelOne, expectedLabelTwo }).ToString();

            //Assert
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        public void WhenICallSetZenDeskLabelsWithLabel_ThenTheOutputIsCorrect()
        {
            //Arrange
            expectedLabelOne = "ass-dashboard";
            expectedOutput =
                $"<script type=\"text/javascript\">zE('webWidget', 'helpCenter:setSuggestions', {{ labels: ['{expectedLabelOne}'] }});</script>";
            
            //Act
            actualOutput = HtmlHelperExtensions.SetZenDeskLabels(null, new string[] { expectedLabelOne}).ToString();

            //Assert
            Assert.AreEqual(expectedOutput, actualOutput);

        }

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

        [Test]
        public void WhenICallSetZenDeskLabelsWithNullLabel_ThenTheOutputIsCorrect()
        {
            //Arrange            
            expectedOutput =
                $"<script type=\"text/javascript\">zE('webWidget', 'helpCenter:setSuggestions', {{ labels: [] }});</script>";
            string[] label = new string[] { null };

            //Act            
            actualOutput = HtmlHelperExtensions.SetZenDeskLabels(null, label).ToString();

            //Assert
            Assert.AreEqual(expectedOutput, actualOutput);
        }
    }
}
