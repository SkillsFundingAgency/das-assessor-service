using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessmentOrgs.Api.Client.Core;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.RegisterUpdate
{
    public class RegisterUpdateHandler : IRequestHandler<RegisterUpdateRequest>
    {
        private readonly IAssessmentOrgsApiClient _apiClient;
        private readonly IOrganisationRepository _organisationRepository;

        public RegisterUpdateHandler(IAssessmentOrgsApiClient apiClient, IOrganisationRepository organisationRepository)
        {
            _apiClient = apiClient;
            _organisationRepository = organisationRepository;
        }

        public async Task Handle(RegisterUpdateRequest message, CancellationToken cancellationToken)
        {
            var epaosOnRegister = await _apiClient.FindAllAsync();
            var organisations = _organisationRepository.GetAllOrganisations();
            foreach (var epaoSummary in epaosOnRegister)
            {
                
            }
        }
    }
}