using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessmentOrgs.Api.Client.Core;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.ViewModel.Models;

namespace SFA.DAS.AssessorService.Application.RegisterUpdate
{
    public class RegisterUpdateHandler : IRequestHandler<RegisterUpdateRequest>
    {
        private readonly IAssessmentOrgsApiClient _registerApiClient;
        private readonly IOrganisationsApiClient _internalApiClient;

        public RegisterUpdateHandler(IAssessmentOrgsApiClient registerApiClient, IOrganisationsApiClient internalApiClient)
        {
            _registerApiClient = registerApiClient;
            _internalApiClient = internalApiClient;
        }

        public async Task Handle(RegisterUpdateRequest message, CancellationToken cancellationToken)
        {
            var epaosOnRegister = await _registerApiClient.FindAllAsync();

            var organisations = await _internalApiClient.GetAll("HANDLER");

            //var organisations = await _organisationRepository.GetAllOrganisations();
            foreach (var epaoSummary in epaosOnRegister)
            {
                if (!organisations.Any(o => o.EndPointAssessorOrganisationId == epaoSummary.Id))
                {
                    var epao = _registerApiClient.Get(epaoSummary.Id);

                    await _internalApiClient.Create("HANDLER",
                        new OrganisationCreateViewModel()
                        {
                            EndPointAssessorOrganisationId = epao.Id,
                            EndPointAssessorName = epao.Name
                        });
                }
            }

            foreach (var org in organisations)
            {
                if (!epaosOnRegister.Any(e => e.Id == org.EndPointAssessorOrganisationId))
                {
                    await _internalApiClient.Delete("HANDLER", org.Id);
                }
            }
        }
    }
}