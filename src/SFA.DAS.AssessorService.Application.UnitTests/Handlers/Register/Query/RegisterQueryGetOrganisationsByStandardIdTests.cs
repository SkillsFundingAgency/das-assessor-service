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
using SFA.DAS.AssessorService.Application.Handlers.ao;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Register.Query
{
    [TestFixture]
    public class RegisterQueryGetOrganisationsByStandardIdTests
    {
        protected Mock<IRegisterQueryRepository> RegisterQueryRepository;
        protected GetAssessmentOrganisationsByStandardHandler GetAssessmentOrganisationsByStandardHandler;
        protected Mock<ILogger<GetAssessmentOrganisationsByStandardHandler>> Logger;
        private List<AssessmentOrganisationDetails> _expectedOrganisationListOfDetails;
        private AssessmentOrganisationDetails _assessmentOrganisationDetails1;
        private AssessmentOrganisationDetails _assessmentOrganisationDetails2;
        private readonly int _standardId = 1;
        private GetAssessmentOrganisationsbyStandardRequest _request;
        [SetUp]
        public void Setup()
        {
            RegisterQueryRepository = new Mock<IRegisterQueryRepository>();
            _assessmentOrganisationDetails1 = new AssessmentOrganisationDetails { Id = "EPA9999", Name = "Name 100", Ukprn = 777777 };
            _assessmentOrganisationDetails2 = new AssessmentOrganisationDetails { Id = "EPA8888", Name = "Name 10" };
            _request = new GetAssessmentOrganisationsbyStandardRequest {StandardId = _standardId};

            Logger = new Mock<ILogger<GetAssessmentOrganisationsByStandardHandler>>();

            _expectedOrganisationListOfDetails = new List<AssessmentOrganisationDetails>
            {
                _assessmentOrganisationDetails1,
                _assessmentOrganisationDetails2
            };

            RegisterQueryRepository.Setup(r => r.GetAssessmentOrganisationsByStandardId(_standardId))
                .Returns(Task.FromResult(_expectedOrganisationListOfDetails.AsEnumerable()));

            GetAssessmentOrganisationsByStandardHandler = new GetAssessmentOrganisationsByStandardHandler(RegisterQueryRepository.Object, Logger.Object);
        }

        [Test]
        public void GetAssessmentOrganisationSummarysRepoIsCalledWhenHandlerInvoked()
        {
            GetAssessmentOrganisationsByStandardHandler.Handle(_request, new CancellationToken()).Wait();
            RegisterQueryRepository.Verify(r => r.GetAssessmentOrganisationsByStandardId(_standardId));
        }

        [Test]
        public void GetAssessmentOrganisationSummarysReturnedExpectedResults()
        {
            var organisations = GetAssessmentOrganisationsByStandardHandler.Handle(_request, new CancellationToken()).Result;
            organisations.Count.Should().Be(2);
            organisations.Should().Contain(_assessmentOrganisationDetails1);
            organisations.Should().Contain(_assessmentOrganisationDetails2);
        }
    }
}
