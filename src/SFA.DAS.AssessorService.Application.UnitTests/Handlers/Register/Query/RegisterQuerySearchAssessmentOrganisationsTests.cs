using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Application.Handlers.ao;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Register.Query
{
    [TestFixture]
    public class RegisterQuerySearchAssessmentOrganisationsTests
    {
        private Mock<IRegisterQueryRepository> _registerQueryRepository;
        private SearchAssessmentOrganisationHandler _searchAssessmentOrganisationsHandler;
        private Mock<ILogger<SearchAssessmentOrganisationHandler>> _logger;
        private Mock<IEpaOrganisationSearchValidator> _searchValidator;
        private Mock<ISpecialCharacterCleanserService> _cleanserService;
        private List<AssessmentOrganisationSummary> _expectedOrganisationListOfDetails;
        private AssessmentOrganisationSummary _assessmentOrganisationDetails1;
        private AssessmentOrganisationSummary _assessmentOrganisationDetails2;

        [SetUp]
        public void Setup()
        {
            _registerQueryRepository = new Mock<IRegisterQueryRepository>();
            _searchValidator = new Mock<IEpaOrganisationSearchValidator>();
            _cleanserService = new Mock<ISpecialCharacterCleanserService>();
            _logger = new Mock<ILogger<SearchAssessmentOrganisationHandler>>();

            _assessmentOrganisationDetails1 = new AssessmentOrganisationSummary { Id = "EPA9999", Name = "Name 100", Ukprn = 777777 };
            _assessmentOrganisationDetails2 = new AssessmentOrganisationSummary { Id = "EPA8888", Name = "Name 10" };

            _expectedOrganisationListOfDetails = new List<AssessmentOrganisationSummary>
            {
                _assessmentOrganisationDetails1,
                _assessmentOrganisationDetails2
            };
           
            _searchAssessmentOrganisationsHandler = new SearchAssessmentOrganisationHandler(_registerQueryRepository.Object, _searchValidator.Object, _logger.Object,_cleanserService.Object);
        }

        [TestCase("A")]
        [TestCase("A ")]
        [TestCase("")]
        [TestCase("A        ")]
        [TestCase("   A  ")]
        public void SearchAssessmentOrganisationsThrowsBadRequestExceptionIfSearchStringTooShort(string search)
        {
            var request = new SearchAssessmentOrganisationsRequest { Searchstring = search };
            _cleanserService.Setup(c => c.CleanseStringForSpecialCharacters(search.Trim())).Returns(search.Trim());
            Assert.ThrowsAsync<BadRequestException>(() => _searchAssessmentOrganisationsHandler.Handle(request, new CancellationToken())); 
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
            var organisations = _searchAssessmentOrganisationsHandler.Handle(request, new CancellationToken()).Result;

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
            var organisations = _searchAssessmentOrganisationsHandler.Handle(request, new CancellationToken()).Result;

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
            _registerQueryRepository.Setup(r => r.GetAssessmentOrganisationsbyName(searchstring))
                .Returns(Task.FromResult(_expectedOrganisationListOfDetails.AsEnumerable()));
            var organisations = _searchAssessmentOrganisationsHandler.Handle(request, new CancellationToken()).Result;

            _searchValidator.Verify(v => v.IsValidEpaOrganisationId(searchstring));
            _registerQueryRepository.Verify(r => r.GetAssessmentOrganisationsByOrganisationId(searchstring), Times.Never);
            _searchValidator.Verify(v => v.IsValidUkprn(It.IsAny<string>()));
            _registerQueryRepository.Verify(r => r.GetAssessmentOrganisationsByUkprn(searchstring), Times.Never);
            _registerQueryRepository.Verify(r => r.GetAssessmentOrganisationsbyName(searchstring));


            organisations.Count.Should().Be(2);
            organisations.Should().Contain(_assessmentOrganisationDetails1);
            organisations.Should().Contain(_assessmentOrganisationDetails2);
        }
    }
}
