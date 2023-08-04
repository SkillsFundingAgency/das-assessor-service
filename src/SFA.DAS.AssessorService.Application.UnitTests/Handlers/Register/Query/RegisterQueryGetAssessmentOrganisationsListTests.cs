using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Handlers.ao;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Collections.Generic;
using System.Threading;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Register.Query
{
    [TestFixture]
    public class RegisterQueryGetAssessmentsOrganisationsListTests
    {
        protected GetAssessmentOrganisationsListHandler _sut;
        protected Mock<ILogger<GetAssessmentOrganisationsListHandler>> Logger;
        protected Mock<IRegisterQueryRepository> RegisterQueryRepository;

        private IEnumerable<AparSummaryItem> _expectedAparSummary;
        private IEnumerable<AparSummaryItem> _expectedAparSummaryForUkprn;
        private AparSummaryItem _aparSummary1;
        private AparSummaryItem _aparSummary2;

        [SetUp]
        public void Setup()
        {
            RegisterQueryRepository = new Mock<IRegisterQueryRepository>();
            _aparSummary1 = new AparSummaryItem { Id = "EPA0001", Name = "Name 1", Ukprn = 1111111 };
            _aparSummary2 = new AparSummaryItem { Id = "EPA0002", Name = "Name 2", Ukprn = 2222222 };

            Logger = new Mock<ILogger<GetAssessmentOrganisationsListHandler>>();

            _expectedAparSummary = new List<AparSummaryItem>
                {
                    _aparSummary1,
                    _aparSummary2
                };

            _expectedAparSummaryForUkprn = new List<AparSummaryItem>
                {
                    _aparSummary1
                };

            RegisterQueryRepository.Setup(r => r.GetAparSummaryByUkprn(new int()))
                .ReturnsAsync(_expectedAparSummary);

            RegisterQueryRepository.Setup(r => r.GetAparSummaryByUkprn(1111111))
                .ReturnsAsync(_expectedAparSummaryForUkprn);

            _sut = new GetAssessmentOrganisationsListHandler(RegisterQueryRepository.Object, Logger.Object);
        }

        [Test]
        public void Handle_GetAssessmentOrganisationsListIsCalledForNonSpecificUkprn_WhenHandlerIsInvoked()
        {
            _sut.Handle(new GetAparSummaryByUkprnRequest(), new CancellationToken()).Wait();
            RegisterQueryRepository.Verify(r => r.GetAparSummaryByUkprn(new int()));
        }

        [Test]
        public void Handle_ReturnExpectedListAssessmentOrganisationListSummaryForNonSpecificUkprn_WhenHandlerIsInvoked()
        {
            var organisations = _sut.Handle(new GetAparSummaryByUkprnRequest(), new CancellationToken()).Result;
            organisations.Count.Should().Be(2);
            organisations.Should().Contain(_aparSummary1);
            organisations.Should().Contain(_aparSummary2);
        }

        [Test]
        public void Handle_GetAssessmentOrganisationsListIsCalledForSpecificUkprn_WhenHandlerIsInvoked()
        {
            _sut.Handle(new GetAparSummaryByUkprnRequest(1111111), new CancellationToken()).Wait();
            RegisterQueryRepository.Verify(r => r.GetAparSummaryByUkprn(1111111));
        }

        [Test]
        public void Handle_ReturnExpectedListAssessmentOrganisationListSummaryForSpecificUkprn_WhenHandlerIsInvoked()
        {
            var organisations = _sut.Handle(new GetAparSummaryByUkprnRequest(1111111), new CancellationToken()).Result;
            organisations.Count.Should().Be(1);
            organisations.Should().Contain(_aparSummary1);
        }
    }
}
