using System.Threading;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessmentOrgs.Api.Client;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Application.RegisterUpdate;

namespace SFA.DAS.AssessorService.Application.UnitTests.RegisterUpdate
{
    [TestFixture]
    public class WhenRegisterUpdateRequestIsSent
    {
        [Test]
        public void ACallToTheEpaoRegisterApiIsMade()
        {
            var apiClient = new Mock<IAssessmentOrgsApiClient>();
            var organisationRepository = new Mock<IOrganisationRepository>();
            var handler = new RegisterUpdateHandler(apiClient.Object, organisationRepository.Object);

            handler.Handle(new RegisterUpdateRequest(), new CancellationToken()).Wait();

            apiClient.Verify(client => client.FindAllAsync());
        }
    }
}