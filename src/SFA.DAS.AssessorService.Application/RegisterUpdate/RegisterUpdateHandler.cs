using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Enums;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs;
using SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs.Types;

namespace SFA.DAS.AssessorService.Application.RegisterUpdate
{
    using AssessorService.Api.Types.Models;

    public class RegisterUpdateHandler : IRequestHandler<RegisterUpdateRequest>
    {
        private readonly IAssessmentOrgsApiClient _registerApiClient;
        private readonly IOrganisationQueryRepository _organisationRepository;
        private readonly ILogger<RegisterUpdateHandler> _logger;
        private readonly IMediator _mediator;
        private List<OrganisationSummary> _epaosOnRegister;
        private List<Organisation> _organisations;

        public RegisterUpdateHandler(IAssessmentOrgsApiClient registerApiClient, IOrganisationQueryRepository organisationRepository, ILogger<RegisterUpdateHandler> logger, IMediator mediator)
        {
            _registerApiClient = registerApiClient;
            _organisationRepository = organisationRepository;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Handle(RegisterUpdateRequest message, CancellationToken cancellationToken)
        {
            var rnd = new Random();

            await GetEpaosAndOrganisations();

            foreach (var epaoSummary in _epaosOnRegister)
            {
                if (OrganisationExists(epaoSummary))
                {
                    await CheckAndUpdateOrganisationName(epaoSummary);
                }
                else
                {
                    await CreateNewOrganisation(epaoSummary, rnd);
                }
            }

            foreach (var org in _organisations)
            {
                if (EpaoStillPresentOnRegister(org)) continue;

                await DeleteOrganisation(org);
            }
        }

        private async Task GetEpaosAndOrganisations()
        {
            _logger.LogInformation("Handling EPAO Import Request");
            _epaosOnRegister = (await _registerApiClient.FindAllAsync()).ToList();

            _logger.LogInformation($"Received {_epaosOnRegister.Count} EPAOs from AssessmentOrgs API");

            _organisations = (await _organisationRepository.GetAllOrganisations()).ToList();

            _logger.LogInformation($"Received {_organisations.Count} Organisations from Repository");
        }

        private bool EpaoStillPresentOnRegister(Organisation org)
        {
            return _epaosOnRegister.Any(e => e.EndPointAssessorOrganisationId == org.EndPointAssessorOrganisationId);
        }

        private bool OrganisationExists(OrganisationSummary epaoSummary)
        {
            return _organisations.Any(o => o.EndPointAssessorOrganisationId == epaoSummary.EndPointAssessorOrganisationId);
        }

        private async Task CheckAndUpdateOrganisationName(OrganisationSummary epaoSummary)
        {
            if (_organisations.Any(o =>
                o.EndPointAssessorOrganisationId == epaoSummary.EndPointAssessorOrganisationId && o.EndPointAssessorName != epaoSummary.Name ))
            {
                var organisation =
                    _organisations.Single(o => o.EndPointAssessorOrganisationId == epaoSummary.EndPointAssessorOrganisationId);
                await _mediator.Send(new UpdateOrganisationRequest()
                {
                    EndPointAssessorName = epaoSummary.Name,
                    EndPointAssessorOrganisationId = organisation.EndPointAssessorOrganisationId
                });

                _logger.LogInformation(
                    $"Organisation with ID {organisation.Id} and EPAOgId {epaoSummary.EndPointAssessorOrganisationId} has had it's Name changed from {organisation.EndPointAssessorName} to {epaoSummary.Name}");
            }
            
            if (_organisations.Any(o => o.EndPointAssessorOrganisationId == epaoSummary.EndPointAssessorOrganisationId && o.OrganisationStatus == OrganisationStatus.Deleted))
            {
                var organisation =
                    _organisations.Single(o => o.EndPointAssessorOrganisationId == epaoSummary.EndPointAssessorOrganisationId);
                await _mediator.Send(new UpdateOrganisationRequest()
                {
                    EndPointAssessorOrganisationId = organisation.EndPointAssessorOrganisationId,
                    EndPointAssessorName = organisation.EndPointAssessorName
                });

                _logger.LogInformation(
                    $"Organisation with ID {organisation.Id} and EPAOgId {epaoSummary.EndPointAssessorOrganisationId} has rejoined the Register and has been Undeleted");
            }

        }

        private async Task DeleteOrganisation(Organisation org)
        {
            await _mediator.Send(new DeleteOrganisationRequest { EndPointAssessorOrganisationId = org.EndPointAssessorOrganisationId });

            _logger.LogInformation(
                $"Organisation with ID {org.Id} and EPAOgId {org.Id} no longer found on Register. Deleting from Repository");
        }

        private async Task CreateNewOrganisation(OrganisationSummary epaoSummary, Random rnd)
        {
            _logger.LogInformation($"EPAO {epaoSummary.EndPointAssessorOrganisationId} not found in Repository");

            var epao = _registerApiClient.Get(epaoSummary.EndPointAssessorOrganisationId);

            _logger.LogInformation($"EPAO {epaoSummary.EndPointAssessorOrganisationId} further information received");

            var createdOrg = await _mediator.Send(new CreateOrganisationRequest
            {
                EndPointAssessorName = epao.Name,
                EndPointAssessorOrganisationId = epao.Id,
                EndPointAssessorUKPRN = rnd.Next(77777777, 99999999)
            });

            _logger.LogInformation($"EPAO {epaoSummary.EndPointAssessorOrganisationId} Created in Repository with ID {createdOrg.Id}");
        }
    }
}