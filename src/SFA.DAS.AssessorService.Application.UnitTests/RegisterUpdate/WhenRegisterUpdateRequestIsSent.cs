namespace SFA.DAS.AssessorService.Application.MSpec.UnitTests
{
    using Machine.Specifications;
    using Moq;
    using SFA.DAS.AssessmentOrgs.Api.Client.Core;
    using SFA.DAS.AssessorService.Application.Interfaces;
    using SFA.DAS.AssessorService.Application.RegisterUpdate;
    using System.Threading;

    [Subject("Authentication")]
    public class WhenRegisterUpdateRequestIsSent
    {
        private static Mock<IAssessmentOrgsApiClient> _apiClient;
        private static Mock<IOrganisationRepository> _organisationRepository;
        private static RegisterUpdateHandler _registerUpdateHandler;

        Establish context = () =>
        {
            _apiClient = new Mock<IAssessmentOrgsApiClient>();
            _organisationRepository = new Mock<IOrganisationRepository>();
            _registerUpdateHandler = new RegisterUpdateHandler(_apiClient.Object, _organisationRepository.Object);
        };

        Because of = () =>
        {
            _registerUpdateHandler.Handle(new RegisterUpdateRequest(), new CancellationToken()).Wait();
        };

        Machine.Specifications.It should_have_persisted_customer = () =>
        {
            _apiClient.Verify(client => client.FindAllAsync());
        };
    }
}
