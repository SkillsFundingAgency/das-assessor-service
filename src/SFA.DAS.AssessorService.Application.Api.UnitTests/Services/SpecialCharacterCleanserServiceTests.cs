using Microsoft.AspNetCore.Mvc;
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

        [TestCase("Colleges’ ", "Colleges'")]
        [TestCase("Colleges ", "Colleges")]
        [TestCase("Raj o`intel", "Raj o\'intel")]
        [TestCase("Intel - Technologies Ltd", "Intel – Technologies Ltd")]
        [TestCase("column 1\u00A0column 2", "column 1 column 2")]
        [TestCase("column 3\tcolumn 4", "column 3 column 4")]
        [TestCase("wildcard%", "wildcard" )]
        public void GetCleansedStringFromOriginalString(string inputString, string outputString)
        {
            var returnedString = _cleanser.CleanseStringForSpecialCharacters(inputString);
            Assert.AreEqual(outputString, returnedString);
        }
        
        [TestCase("Colleges' ","Colleges")]
        [TestCase("Colleges ", "Colleges")]
        [TestCase("Raj o`intel", "Rajointel")]
        [TestCase("IntelTechnologiesLtd", "IntelTechnologiesLtd")]
        [TestCase("column 1\u00A0column 2", "column1column2")]
        [TestCase(" c o lumn3       co lumn4", "column3column4")]
        [TestCase("wildcard%", "wildcard" )]
        [TestCase("wild / card", "wildcard" )]
        [TestCase("wild \n card", "wildcard" )]
        [TestCase("wild %2F card", "wildcard" )]
        public void GetStringWithNonAlphanumericCharactersRemoved(string inputString, string outputString)
        {
            var returnedString = _cleanser.UnescapeAndRemoveNonAlphanumericCharacters(inputString);
            Assert.AreEqual(outputString, returnedString);
        }
    }
}