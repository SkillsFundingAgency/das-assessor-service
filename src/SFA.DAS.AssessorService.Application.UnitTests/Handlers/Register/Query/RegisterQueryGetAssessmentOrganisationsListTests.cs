using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Handlers.ao;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Register.Query
{
    [TestFixture]
    public class RegisterQueryGetAssessmentsOrganisationsListTests
    {
        protected GetAssessmentOrganisationsListHandler _sut;
        protected Mock<ILogger<GetAssessmentOrganisationsListHandler>> Logger;
        protected Mock<IRegisterQueryRepository> RegisterQueryRepository;

        private IEnumerable<AssessmentOrganisationListSummary> _expectedOrganisationSummaries;
        private IEnumerable<AssessmentOrganisationListSummary> _expectedOrganisationSummariesForUkprn;
        private AssessmentOrganisationListSummary _assessmentOrganisationSummary1;
        private AssessmentOrganisationListSummary _assessmentOrganisationSummary2;

        [SetUp]
        public void Setup()
        {
            RegisterQueryRepository = new Mock<IRegisterQueryRepository>();
            _assessmentOrganisationSummary1 = new AssessmentOrganisationListSummary { Id = "EPA0001", Name = "Name 1", Ukprn = 1111111 };
            _assessmentOrganisationSummary2 = new AssessmentOrganisationListSummary { Id = "EPA0002", Name = "Name 2", Ukprn = 2222222 };

            Logger = new Mock<ILogger<GetAssessmentOrganisationsListHandler>>();

            _expectedOrganisationSummaries = new List<AssessmentOrganisationListSummary>
                {
                    _assessmentOrganisationSummary1,
                    _assessmentOrganisationSummary2
                };

            _expectedOrganisationSummariesForUkprn = new List<AssessmentOrganisationListSummary>
                {
                    _assessmentOrganisationSummary1,
                };

            RegisterQueryRepository.Setup(r => r.GetAssessmentOrganisationsList(null))
                .ReturnsAsync(_expectedOrganisationSummaries);

            RegisterQueryRepository.Setup(r => r.GetAssessmentOrganisationsList(1111111))
                .ReturnsAsync(_expectedOrganisationSummariesForUkprn);

            _sut = new GetAssessmentOrganisationsListHandler(RegisterQueryRepository.Object, Logger.Object);
        }

        [Test]
        public void Handle_GetAssessmentOrganisationsListIsCalledForNonSpecificUkprn_WhenHandlerIsInvoked()
        {
            _sut.Handle(new GetAssessmentOrganisationsListRequest(), new CancellationToken()).Wait();
            RegisterQueryRepository.Verify(r => r.GetAssessmentOrganisationsList(null));
        }

        [Test]
        public void Handle_ReturnExpectedListAssessmentOrganisationListSummaryForNonSpecificUkprn_WhenHandlerIsInvoked()
        {
            var organisations = _sut.Handle(new GetAssessmentOrganisationsListRequest(), new CancellationToken()).Result;
            organisations.Count.Should().Be(2);
            organisations.Should().Contain(_assessmentOrganisationSummary1);
            organisations.Should().Contain(_assessmentOrganisationSummary2);
        }

        [Test]
        public void Handle_GetAssessmentOrganisationsListIsCalledForSpecificUkprn_WhenHandlerIsInvoked()
        {
            _sut.Handle(new GetAssessmentOrganisationsListRequest(1111111), new CancellationToken()).Wait();
            RegisterQueryRepository.Verify(r => r.GetAssessmentOrganisationsList(1111111));
        }

        [Test]
        public void Handle_ReturnExpectedListAssessmentOrganisationListSummaryForSpecificUkprn_WhenHandlerIsInvoked()
        {
            var organisations = _sut.Handle(new GetAssessmentOrganisationsListRequest(1111111), new CancellationToken()).Result;
            organisations.Count.Should().Be(1);
            organisations.Should().Contain(_assessmentOrganisationSummary1);
        }
    }
}
