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
    public class RegisterQueryGetAssesmentOrganisationsTests
    {
        protected GetAssessmentOrganisationsHandler _sut;
        protected Mock<ILogger<GetAssessmentOrganisationsHandler>> Logger;
        protected Mock<IRegisterQueryRepository> RegisterQueryRepository;

        private List<AssessmentOrganisationSummary> _expectedOrganisationSummaries;
        private AssessmentOrganisationSummary _assessmentOrganisationSummary1;
        private AssessmentOrganisationSummary _assessmentOrganisationSummary2;

        [SetUp]
        public void Setup()
        {
            RegisterQueryRepository = new Mock<IRegisterQueryRepository>();
            _assessmentOrganisationSummary1 = new AssessmentOrganisationSummary { Id = "EPA0001", Name = "Name 1", Ukprn = 1111111 };
            _assessmentOrganisationSummary2 = new AssessmentOrganisationSummary { Id = "EPA0002", Name = "Name 2", Ukprn = 2222222 };

            Logger = new Mock<ILogger<GetAssessmentOrganisationsHandler>>();

            _expectedOrganisationSummaries = new List<AssessmentOrganisationSummary>
                {
                    _assessmentOrganisationSummary1,
                    _assessmentOrganisationSummary2
                };

            RegisterQueryRepository.Setup(r => r.GetAssessmentOrganisations())
                .Returns(Task.FromResult(_expectedOrganisationSummaries.AsEnumerable()));

            _sut = new GetAssessmentOrganisationsHandler(RegisterQueryRepository.Object, Logger.Object);
        }

        [Test]
        public void Handle_GetAssessmentOrganisationsIsCalled_WhenHandlerIsInvoked()
        {
            _sut.Handle(new GetAssessmentOrganisationsRequest(), new CancellationToken()).Wait();
            RegisterQueryRepository.Verify(r => r.GetAssessmentOrganisations());
        }

        [Test]
        public void Handle_ReturnExpectedListAssessmentOrganisationSummary_WhenHandlerIsInvoked()
        {
            var organisations = _sut.Handle(new GetAssessmentOrganisationsRequest(), new CancellationToken()).Result;
            organisations.Count.Should().Be(2);
            organisations.Should().Contain(_assessmentOrganisationSummary1);
            organisations.Should().Contain(_assessmentOrganisationSummary2);
        }
    }
}
