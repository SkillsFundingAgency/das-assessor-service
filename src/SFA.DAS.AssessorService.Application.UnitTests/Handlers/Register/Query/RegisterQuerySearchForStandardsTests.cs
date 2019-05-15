using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.AssessorService.Api.Types;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Handlers;
using SFA.DAS.AssessorService.Application.Handlers.ao;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ExternalApis.Services;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Register.Query
{
    [TestFixture]
    public class RegisterQuerySearchForStandardsTests
    {
        private Mock<IStandardService> _standardService;
        private SearchStandardsHandler _searchStandardsHandler;
        private Mock<ILogger<SearchStandardsHandler>> _logger;
        private Mock<ISpecialCharacterCleanserService> _cleanserService;
        private Mock<IEpaOrganisationValidator> _validator;
        private List<StandardCollation> _expectedStandards;
        private StandardCollation _standardSummary1;
        private StandardCollation _standardSummary2;
        private ValidationResponse errorResponse;
        const string errorMessage = "error happened";

        [SetUp]
        public void Setup()
        {
            _standardService = new Mock<IStandardService>();
            _cleanserService = new Mock<ISpecialCharacterCleanserService>();
            _logger = new Mock<ILogger<SearchStandardsHandler>>();
            _validator = new Mock<IEpaOrganisationValidator>();
            _standardSummary1 = new StandardCollation { StandardId = 1, Title = "name 100" };
            _standardSummary2 = new StandardCollation { StandardId = 2, Title = "name 10" };
            errorResponse = BuildErrorResponse(errorMessage, ValidationStatusCode.BadRequest);
            _validator.Setup(v => v.ValidatorSearchStandardsRequest(It.IsAny<SearchStandardsValidationRequest>())).Returns(new ValidationResponse());

            _expectedStandards = new List<StandardCollation>
            {
                _standardSummary1,
                _standardSummary2
            };
           
            _cleanserService.Setup(c => c.UnescapeAndRemoveNonAlphanumericCharacters(_standardSummary1.Title)).Returns(_standardSummary1.Title);
            _cleanserService.Setup(c => c.UnescapeAndRemoveNonAlphanumericCharacters(_standardSummary2.Title)).Returns(_standardSummary2.Title);

            _standardService.Setup(s => s.GetAllStandards()).Returns(Task.FromResult(_expectedStandards.AsEnumerable()));
            _searchStandardsHandler = new SearchStandardsHandler(_standardService.Object,  _logger.Object,_cleanserService.Object, _validator.Object);
        }

        [TestCase("A")]
        [TestCase("A ")]
        [TestCase("")]
        [TestCase("A        ")]
        [TestCase("   A  ")]
        public void SearchStandardsThrowsBadRequestExceptionIfSearchStringTooShort(string search)
        {
            var request = new SearchStandardsRequest { SearchTerm = search };
            _cleanserService.Setup(c => c.UnescapeAndRemoveNonAlphanumericCharacters(search)).Returns(search.Trim());
            _validator.Setup(v => v.ValidatorSearchStandardsRequest(It.IsAny<SearchStandardsValidationRequest>())).Returns(errorResponse);
            Assert.ThrowsAsync<BadRequestException>(() => _searchStandardsHandler.Handle(request, new CancellationToken())); 
        }

        [Test]
        public void SearchStandardsWithValidStandardId()
        {
            var searchstring = _standardSummary1.StandardId.ToString();
            var request = new SearchStandardsRequest { SearchTerm = searchstring };
            _cleanserService.Setup(c => c.UnescapeAndRemoveNonAlphanumericCharacters(searchstring)).Returns(searchstring);
            _standardService.Setup(r => r.GetAllStandards())
                .Returns(Task.FromResult(_expectedStandards.AsEnumerable()));
            var standards = _searchStandardsHandler.Handle(request, new CancellationToken()).Result;

            standards.Count.Should().Be(1);
            standards.Should().Contain(_standardSummary1);
        }
        
        [Test]
        public void SearchStandardsWithInvalidStandardId()
        {
            var searchstring = "99";
            var request = new SearchStandardsRequest { SearchTerm = searchstring };
            _cleanserService.Setup(c => c.UnescapeAndRemoveNonAlphanumericCharacters(searchstring)).Returns(searchstring);
        
            _standardService.Setup(r => r.GetAllStandards())
                .Returns(Task.FromResult(_expectedStandards.AsEnumerable()));
            var standardSummaries = _searchStandardsHandler.Handle(request, new CancellationToken()).Result;

            standardSummaries.Count.Should().Be(0);
        }
         
        [Test]
        public void SearchStandardsWithValidWordSearchReturns2Results()
        {
            var searchstring = "Name";
            var request = new SearchStandardsRequest { SearchTerm = searchstring };
            _cleanserService.Setup(c => c.UnescapeAndRemoveNonAlphanumericCharacters(searchstring)).Returns(searchstring); 
            _standardService.Setup(r => r.GetAllStandards())
                .Returns(Task.FromResult(_expectedStandards.AsEnumerable()));
            var standardSummaries = _searchStandardsHandler.Handle(request, new CancellationToken()).Result;

            standardSummaries.Count.Should().Be(2);
            standardSummaries.Should().Contain(_standardSummary1);
            standardSummaries.Should().Contain(_standardSummary2);
        }
        
        [Test]
        public void SearchStandardsWithValidWordSearchReturns1Result()
        {
            var searchstring = "Name 100";
            var request = new SearchStandardsRequest { SearchTerm = searchstring };
            _cleanserService.Setup(c => c.UnescapeAndRemoveNonAlphanumericCharacters(searchstring)).Returns(searchstring);       
            _standardService.Setup(r => r.GetAllStandards())
                .Returns(Task.FromResult(_expectedStandards.AsEnumerable()));
            var standardSummaries = _searchStandardsHandler.Handle(request, new CancellationToken()).Result;

            standardSummaries.Count.Should().Be(1);
            standardSummaries.Should().Contain(_standardSummary1);
        }
        
        [Test]
        public void SearchStandardsWithValidWordSearchReturnsZeroResults()
        {
            var searchstring = "no match";
            var request = new SearchStandardsRequest { SearchTerm = searchstring };
            _cleanserService.Setup(c => c.UnescapeAndRemoveNonAlphanumericCharacters(searchstring)).Returns(searchstring);       
            _standardService.Setup(r => r.GetAllStandards())
                .Returns(Task.FromResult(_expectedStandards.AsEnumerable()));
            var standardSummaries = _searchStandardsHandler.Handle(request, new CancellationToken()).Result;

            standardSummaries.Count.Should().Be(0);
        }

        private ValidationResponse BuildErrorResponse(string errorMessage, ValidationStatusCode statusCode)
        {
            var validationResponse = new ValidationResponse();
            validationResponse.Errors.Add(new ValidationErrorDetail(errorMessage, statusCode));
            return validationResponse;
        }
    }
}
