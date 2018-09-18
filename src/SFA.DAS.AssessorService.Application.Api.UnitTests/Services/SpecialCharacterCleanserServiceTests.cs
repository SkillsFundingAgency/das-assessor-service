using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Services;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Services
{
    [TestFixture]
    public class SpecialCharacterCleanserServiceTests
    {
        private SpecialCharacterCleanserService _cleanser;


        [SetUp]
        public void Setup()
        {           
            _cleanser = new SpecialCharacterCleanserService();   
        }

        [TestCase("Colleges’ ", "Colleges' ")]
        [TestCase("Colleges ", "Colleges ")]
        [TestCase("Raj o`intel - Technologies Ltd", "Raj o\'intel – Technologies Ltd")]
        public void GetCleansedStringFromOriginalString(string inputString, string outputString)
        {
            var returnedString = _cleanser.CleanseStringForSpecialCharacters(inputString);
            Assert.AreEqual(outputString, returnedString);
        }
    }
}