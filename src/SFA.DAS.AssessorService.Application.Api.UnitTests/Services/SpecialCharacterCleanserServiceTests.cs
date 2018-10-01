using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
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
        [TestCase("Raj o`intel", "Raj o\'intel")]
        [TestCase("Intel - Technologies Ltd", "Intel – Technologies Ltd")]
        [TestCase("column 1\u00A0column 2", "column 1 column 2")]
        [TestCase("column 3\tcolumn 4", "column 3 column 4")]
        [TestCase("wildcard%", "wildcard " )]
        public void GetCleansedStringFromOriginalString(string inputString, string outputString)
        {
            var returnedString = _cleanser.CleanseStringForSpecialCharacters(inputString);
            Assert.AreEqual(outputString, returnedString);
        }
    }
}