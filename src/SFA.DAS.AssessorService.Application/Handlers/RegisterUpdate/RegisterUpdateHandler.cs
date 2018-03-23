using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs.Types;
using OrganisationResponse = SFA.DAS.AssessorService.Api.Types.Models.OrganisationResponse;

namespace SFA.DAS.AssessorService.Application.Handlers.RegisterUpdate
{
    public class RegisterUpdateHandler : IRequestHandler<RegisterUpdateRequest, RegisterUpdateResponse>
    {
        private readonly IAssessmentOrgsApiClient _registerApiClient;
        private readonly IOrganisationQueryRepository _organisationRepository;
        private readonly ILogger<RegisterUpdateHandler> _logger;
        private readonly IMediator _mediator;
        private List<OrganisationSummary> _epaosOnRegister;
        private List<Domain.Entities.Organisation> _organisations;
        private RegisterUpdateResponse _response;

        public RegisterUpdateHandler(IAssessmentOrgsApiClient registerApiClient, IOrganisationQueryRepository organisationRepository, ILogger<RegisterUpdateHandler> logger, IMediator mediator)
        {
            _registerApiClient = registerApiClient;
            _organisationRepository = organisationRepository;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<RegisterUpdateResponse> Handle(RegisterUpdateRequest request, CancellationToken cancellationToken)
        {
            _response = new RegisterUpdateResponse();

            
            await GetEpaosAndOrganisations();

            foreach (var epaoSummary in _epaosOnRegister)
            {
                if (OrganisationExists(epaoSummary))
                {
                    await CheckAndUpdateOrganisationName(epaoSummary);
                }
                else
                {
                    await CreateNewOrganisation(epaoSummary);
                }
            }

            foreach (var org in _organisations)
            {
                if (EpaoStillPresentOnRegister(org)) continue;

                await DeleteOrganisation(org);
            }
            return _response;
        }

        private async Task GetEpaosAndOrganisations()
        {
            _logger.LogInformation("Handling EPAO Import Request");
            _epaosOnRegister = (await _registerApiClient.FindAllAsync()).ToList();

            _logger.LogInformation($"Received {_epaosOnRegister.Count} EPAOs from AssessmentOrgs API");

            _organisations = (await _organisationRepository.GetAllOrganisations()).ToList();

            _logger.LogInformation($"Received {_organisations.Count} Organisations from Repository");

            _response.EpaosOnRegister = _epaosOnRegister.Count;
            _response.OrganisationsInDatabase =
                _organisations.Count(org => org.Status != OrganisationStatus.Deleted);
            _response.DeletedOrganisationsInDatabase =
                _organisations.Count(org => org.Status == OrganisationStatus.Deleted);
        }

        private bool EpaoStillPresentOnRegister(Domain.Entities.Organisation org)
        {
            return _epaosOnRegister.Any(e => e.Id == org.EndPointAssessorOrganisationId);
        }

        private bool OrganisationExists(OrganisationSummary epaoSummary)
        {
            return _organisations.Any(o => o.EndPointAssessorOrganisationId == epaoSummary.Id);
        }

        private async Task CheckAndUpdateOrganisationName(OrganisationSummary epaoSummary)
        {
            if (_organisations.Any(o =>
                o.EndPointAssessorOrganisationId == epaoSummary.Id && o.EndPointAssessorName != epaoSummary.Name))
            {
                var organisation =
                    _organisations.Single(o => o.EndPointAssessorOrganisationId == epaoSummary.Id);
                await _mediator.Send(new UpdateOrganisationRequest()
                {
                    EndPointAssessorName = epaoSummary.Name,
                    EndPointAssessorOrganisationId = organisation.EndPointAssessorOrganisationId
                });

                _response.OrganisationsUpdated++;

                _logger.LogInformation(
                    $"Organisation with ID {organisation.EndPointAssessorOrganisationId } and EPAOgId {epaoSummary.Id} has had it's Name changed from {organisation.EndPointAssessorName} to {epaoSummary.Name}");
            }

            if (_organisations.Any(o => o.EndPointAssessorOrganisationId == epaoSummary.Id && o.Status == OrganisationStatus.Deleted))
            {
                var organisation =
                    _organisations.Single(o => o.EndPointAssessorOrganisationId == epaoSummary.Id);
                await _mediator.Send(new UpdateOrganisationRequest()
                {
                    EndPointAssessorOrganisationId = organisation.EndPointAssessorOrganisationId,
                    EndPointAssessorName = organisation.EndPointAssessorName
                });

                _response.OrganisationsUnDeleted++;

                _logger.LogInformation(
                    $"Organisation with ID {organisation.EndPointAssessorOrganisationId } and EPAOgId {epaoSummary.Id} has rejoined the Register and has been Undeleted");
            }

        }

        private async Task DeleteOrganisation(Domain.Entities.Organisation org)
        {
            if (org.Status == OrganisationStatus.Deleted)
            {
                _logger.LogInformation(
                    $"Organisation with ID {org.EndPointAssessorOrganisationId } and EPAOgId {org.EndPointAssessorOrganisationId } no longer found on Register and has previously been deleted.");

                return;
            }
            await _mediator.Send(new DeleteOrganisationRequest { EndPointAssessorOrganisationId = org.EndPointAssessorOrganisationId });

            _response.OrganisationsDeleted++;

            _logger.LogInformation(
                $"Organisation with ID {org.EndPointAssessorOrganisationId } and EPAOgId {org.EndPointAssessorOrganisationId } no longer found on Register. Deleting from Repository");
        }

        private async Task CreateNewOrganisation(OrganisationSummary epaoSummary)
        {
            _logger.LogInformation($"EPAO {epaoSummary.Id} not found in Repository");

            var epao = _registerApiClient.Get(epaoSummary.Id);

            _logger.LogInformation($"EPAO {epaoSummary.Id} further information received");

            var createdOrg = await _mediator.Send(new CreateOrganisationRequest
            {
                EndPointAssessorName = epao.Name,
                EndPointAssessorOrganisationId = epao.Id,
                EndPointAssessorUkprn = int.Parse(epao.Id.Replace("EPA", "1111"))
            });

            _response.OrganisationsCreated++;

            _logger.LogInformation($"EPAO {epaoSummary.Id} Created in Repository with ID {createdOrg.EndPointAssessorOrganisationId }");
        }

        
    }
}