
using System.Threading;
using System.Threading.Tasks;
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
    public class RegisterQueryGetOrganisationTests
    {
        protected Mock<IRegisterQueryRepository> RegisterQueryRepository;
        protected GetAssessmentOrganisationHandler GetAssessmentOrganisationDetailsHandler;
        protected Mock<ILogger<GetAssessmentOrganisationHandler>> Logger;
        private EpaOrganisation _initialOrganisationDetails; 
        private GetAssessmentOrganisationRequest _requestDetails;
        private const string OrganisationId = "DEF345";

        [SetUp]
        public void Setup()
        {
            RegisterQueryRepository = new Mock<IRegisterQueryRepository>();
          
            Logger = new Mock<ILogger<GetAssessmentOrganisationHandler>>();

            _requestDetails = new GetAssessmentOrganisationRequest {OrganisationId = OrganisationId};

            _initialOrganisationDetails = new EpaOrganisation
            {
                OrganisationId = OrganisationId,
                Name = "Organisation X",
                Ukprn = 123456
            };
            
            RegisterQueryRepository.Setup(r => r.GetEpaOrganisationByOrganisationId(OrganisationId))
                .Returns(Task.FromResult(_initialOrganisationDetails));

            GetAssessmentOrganisationDetailsHandler = new GetAssessmentOrganisationHandler(RegisterQueryRepository.Object, Logger.Object);
        }

        [Test]
        public void GetOrganisationDetailsRepoIsCalledWhenHandlerInvoked()
        {
            GetAssessmentOrganisationDetailsHandler.Handle(_requestDetails, new CancellationToken()).Wait();
            RegisterQueryRepository.Verify(r => r.GetEpaOrganisationByOrganisationId(OrganisationId));
        }
    }
}
