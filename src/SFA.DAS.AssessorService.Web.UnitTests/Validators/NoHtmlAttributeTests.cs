namespace SFA.DAS.AssessorService.Web.UnitTests.Validators
{
    using NUnit.Framework;
    using SFA.DAS.AssessorService.Web.Validators;

    public class NoHtmlAttributeTests
    {
        
        [TestCase("<a onmouseovealert(document.cookie)> XSS Attack </a>")]
        [TestCase("&lt;")]
        [TestCase("&gt;")]
        [TestCase("123 blah <road")]
        [TestCase("123 blah road>")]
        public void IsValid_Returns_False_ForInvalidInput(string input)
        {
            Assert.That(new NoHtmlAttribute().IsValid(input), Is.False);
        }

        [TestCase("123 blah road")]
        [TestCase("London")]
        [TestCase("N9 7BN")]
        public void IsValid_Returns_True_ForValidInput(string input)
        {
            Assert.That(new NoHtmlAttribute().IsValid(input), Is.True);
        }
    }
}
