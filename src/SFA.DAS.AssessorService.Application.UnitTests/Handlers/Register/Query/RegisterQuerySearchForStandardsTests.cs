using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.AssessorService.Application.Handlers;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Web.Staff.Services;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Register.Query
{
    [TestFixture]
    public class RegisterQuerySearchForStandardsTests
    {
        private Mock<IStandardService> _standardService;
        private SearchStandardsHandler _searchStandardsHandler;
        private Mock<ILogger<SearchStandardsHandler>> _logger;;
        private Mock<ISpecialCharacterCleanserService> _cleanserService;
        private List<StandardSummary> _expectedStandards;
        private StandardSummary _standardSummary1;
        private StandardSummary _standardSummary2;

        [SetUp]
        public void Setup()
        {
            _standardService = new Mock<IStandardService>();
            _cleanserService = new Mock<ISpecialCharacterCleanserService>();
            _logger = new Mock<ILogger<SearchStandardsHandler>>();

            _standardSummary1 = new StandardSummary { Id = "1", Title = "Name 100" };
            _standardSummary2 = new StandardSummary { Id = "2", Title = "Name 10" };

            _expectedStandards = new List<StandardSummary>
            {
                _standardSummary1,
                _standardSummary2
            };
           
            _searchStandardsHandler = new SearchStandardsHandler(_standardService.Object,  _logger.Object,_cleanserService.Object);
        }
/*
        [TestCase("A")]
        [TestCase("A ")]
        [TestCase("")]
        [TestCase("A        ")]
        [TestCase("   A  ")]
        public void SearchAssessmentOrganisationsThrowsBadRequestExceptionIfSearchStringTooShort(string search)
        {
            var request = new SearchAssessmentOrganisationsRequest { Searchstring = search };
            _cleanserService.Setup(c => c.CleanseStringForSpecialCharacters(search.Trim())).Returns(search.Trim());
            Assert.ThrowsAsync<BadRequestException>(() => _searchStandardsHandler.Handle(request, new CancellationToken())); 
        }

        [Test]
        public void SearchAssessmentOrganisationsWithValidOrganisationId()
        {
            const string searchstring = "epacode";
            var request = new SearchAssessmentOrganisationsRequest { Searchstring = searchstring };
            _cleanserService.Setup(c => c.CleanseStringForSpecialCharacters(searchstring)).Returns(searchstring);
            _searchValidator.Setup(v => v.IsValidEpaOrganisationId(searchstring)).Returns(true);
            _searchValidator.Setup(v => v.IsValidUkprn(searchstring)).Returns(true);
            _registerQueryRepository.Setup(r => r.GetAssessmentOrganisationsByOrganisationId(searchstring))
                .Returns(Task.FromResult(_expectedOrganisationListOfDetails.AsEnumerable()));
            var organisations = _searchStandardsHandler.Handle(request, new CancellationToken()).Result;

            _searchValidator.Verify(v => v.IsValidEpaOrganisationId(searchstring));
            _searchValidator.Verify(v => v.IsValidUkprn(It.IsAny<string>()),Times.Never);
            _registerQueryRepository.Verify(r => r.GetAssessmentOrganisationsByOrganisationId(searchstring));
            organisations.Count.Should().Be(2);
            organisations.Should().Contain(_assessmentOrganisationDetails1);
            organisations.Should().Contain(_assessmentOrganisationDetails2);
        }

        [Test]
        public void SearchAssessmentOrganisationsWithValidUkprn()
        {
            const string searchstring = "12345678";
            var request = new SearchAssessmentOrganisationsRequest { Searchstring = searchstring };
            _cleanserService.Setup(c => c.CleanseStringForSpecialCharacters(searchstring)).Returns(searchstring);
            _searchValidator.Setup(v => v.IsValidEpaOrganisationId(searchstring)).Returns(false);
            _searchValidator.Setup(v => v.IsValidUkprn(searchstring)).Returns(true);
            _registerQueryRepository.Setup(r => r.GetAssessmentOrganisationsByOrganisationId(searchstring))
                .Returns(Task.FromResult(new List<AssessmentOrganisationSummary>().AsEnumerable()));
            _registerQueryRepository.Setup(r => r.GetAssessmentOrganisationsByUkprn(searchstring))
                .Returns(Task.FromResult(_expectedOrganisationListOfDetails.AsEnumerable()));
            var organisations = _searchStandardsHandler.Handle(request, new CancellationToken()).Result;

            _searchValidator.Verify(v => v.IsValidEpaOrganisationId(searchstring));
            _registerQueryRepository.Verify(r => r.GetAssessmentOrganisationsByOrganisationId(searchstring), Times.Never);
            _searchValidator.Verify(v => v.IsValidUkprn(It.IsAny<string>()));
            _registerQueryRepository.Verify(r => r.GetAssessmentOrganisationsByUkprn(searchstring));

            organisations.Count.Should().Be(2);
            organisations.Should().Contain(_assessmentOrganisationDetails1);
            organisations.Should().Contain(_assessmentOrganisationDetails2);
        }

        [Test]
        public void SearchAssessmentOrganisationsWithGeneralSearchString()
        {
            const string searchstring = "12345678";
            var request = new SearchAssessmentOrganisationsRequest { Searchstring = searchstring };
            _cleanserService.Setup(c => c.CleanseStringForSpecialCharacters(searchstring)).Returns(searchstring);
            _searchValidator.Setup(v => v.IsValidEpaOrganisationId(searchstring)).Returns(false);
            _searchValidator.Setup(v => v.IsValidUkprn(searchstring)).Returns(false);
            _registerQueryRepository.Setup(r => r.GetAssessmentOrganisationsByOrganisationId(searchstring))
                .Returns(Task.FromResult(new List<AssessmentOrganisationSummary>().AsEnumerable()));
            _registerQueryRepository.Setup(r => r.GetAssessmentOrganisationsByUkprn(searchstring))
                .Returns(Task.FromResult(new List<AssessmentOrganisationSummary>().AsEnumerable()));
            _registerQueryRepository.Setup(r => r.GetAssessmentOrganisationsByName(searchstring))
                .Returns(Task.FromResult(_expectedOrganisationListOfDetails.AsEnumerable()));
            var organisations = _searchStandardsHandler.Handle(request, new CancellationToken()).Result;

            _searchValidator.Verify(v => v.IsValidEpaOrganisationId(searchstring));
            _registerQueryRepository.Verify(r => r.GetAssessmentOrganisationsByOrganisationId(searchstring), Times.Never);
            _searchValidator.Verify(v => v.IsValidUkprn(It.IsAny<string>()));
            _registerQueryRepository.Verify(r => r.GetAssessmentOrganisationsByUkprn(searchstring), Times.Never);
            _registerQueryRepository.Verify(r => r.GetAssessmentOrganisationsByName(searchstring));


            organisations.Count.Should().Be(2);
            organisations.Should().Contain(_assessmentOrganisationDetails1);
            organisations.Should().Contain(_assessmentOrganisationDetails2);
        }
    }*/
}
