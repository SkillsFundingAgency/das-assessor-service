using Moq;
using SFA.DAS.AssessmentOrgs.Api.Client.Core;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.RegisterUpdate;

namespace SFA.DAS.AssessorService.Application.UnitTests.RegisterUpdate
{
    public class RegisterUpdateTestsBase
    {
        protected Mock<IAssessmentOrgsApiClient> ApiClient;
        protected Mock<IOrganisationRepository> OrganisationRepository;
        protected RegisterUpdateHandler RegisterUpdateHandler;

        protected void Setup()
        {
            ApiClient = new Mock<IAssessmentOrgsApiClient>();

            OrganisationRepository = new Mock<IOrganisationRepository>();

            RegisterUpdateHandler = new RegisterUpdateHandler(ApiClient.Object, OrganisationRepository.Object);
        }
    }
}