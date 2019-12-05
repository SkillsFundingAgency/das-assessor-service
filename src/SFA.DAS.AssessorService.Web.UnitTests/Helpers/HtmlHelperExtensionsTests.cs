using NUnit.Framework;
using SFA.DAS.AssessorService.Web.Extensions;

namespace SFA.DAS.AssessorService.Web.UnitTests.Helpers
{
    [TestFixture]
    public class HtmlHelperExtensionsTests
    {
        private string _expectedOutput;
        private string _expectedLabelOne;
        private string _expectedLabelTwo;
        private string _actualOutput;      

        [Test]
        public void WhenICallSetZenDeskLabelsWithMultipleLabelsWithMultipleWordsAndApostrophes_ThenTheOutputIsCorrect()
        {
            //Arrange
            _expectedLabelOne = "a string with multiple words";
            _expectedLabelTwo = "the title of another page";
            _expectedOutput =
                $"<script type=\"text/javascript\">zE('webWidget', 'helpCenter:setSuggestions', {{ labels: ['{_expectedLabelOne}','{_expectedLabelTwo}'] }});</script>";

            //Act
            _actualOutput = HtmlHelperExtensions.SetZenDeskLabels(null, new string[] { _expectedLabelOne, _expectedLabelTwo }).ToString();

            //Assert
            Assert.AreEqual(_expectedOutput, _actualOutput);
        }

        [Test]
        public void WhenICallSetZenDeskLabelsWithLabel_ThenTheOutputIsCorrect()
        {
            //Arrange
            _expectedLabelOne = "ass-dashboard";
            _expectedOutput =
                $"<script type=\"text/javascript\">zE('webWidget', 'helpCenter:setSuggestions', {{ labels: ['{_expectedLabelOne}'] }});</script>";
            
            //Act
            _actualOutput = HtmlHelperExtensions.SetZenDeskLabels(null, new string[] { _expectedLabelOne}).ToString();

            //Assert
            Assert.AreEqual(_expectedOutput, _actualOutput);

        }
    }
}
