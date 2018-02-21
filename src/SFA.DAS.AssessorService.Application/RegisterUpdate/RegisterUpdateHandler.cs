﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessmentOrgs.Api.Client.Core;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.ViewModel.Models;

namespace SFA.DAS.AssessorService.Application.RegisterUpdate
{
    public class RegisterUpdateHandler : IRequestHandler<RegisterUpdateRequest>
    {
        private readonly IAssessmentOrgsApiClient _registerApiClient;
        private readonly IOrganisationQueryRepository _organisationRepository;
        private readonly ILogger<RegisterUpdateHandler> _logger;
        private readonly IMediator _mediator;

        public RegisterUpdateHandler(IAssessmentOrgsApiClient registerApiClient, IOrganisationQueryRepository organisationRepository, ILogger<RegisterUpdateHandler> logger, IMediator mediator)
        {
            _registerApiClient = registerApiClient;
            _organisationRepository = organisationRepository;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Handle(RegisterUpdateRequest message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling EPAO Import Request");
            var epaosOnRegister = (await _registerApiClient.FindAllAsync()).ToList();

            _logger.LogInformation($"Received {epaosOnRegister.Count} EPAOs from AssessmentOrgs API");

            var organisations = (await _organisationRepository.GetAllOrganisations()).ToList();

            _logger.LogInformation($"Received {organisations.Count} Organisations from Repository");

            foreach (var epaoSummary in epaosOnRegister)
            {
                var rnd = new Random();
                if (organisations.Any(o => o.EndPointAssessorOrganisationId == epaoSummary.Id)) continue;

                _logger.LogInformation($"EPAO {epaoSummary.Id} not found in Repository");

                var epao = _registerApiClient.Get(epaoSummary.Id);

                _logger.LogInformation($"EPAO {epaoSummary.Id} further information received");

                var createdOrg = await _mediator.Send(new OrganisationCreateViewModel 
                {
                    EndPointAssessorName = epao.Name,
                    EndPointAssessorOrganisationId = epao.Id,
                    EndPointAssessorUKPRN = rnd.Next(77777777, 99999999)
                });

                _logger.LogInformation($"EPAO {epaoSummary.Id} Created in Repository with ID {createdOrg.Id}");
            }

            foreach (var org in organisations)
            {
                if (epaosOnRegister.Any(e => e.Id == org.EndPointAssessorOrganisationId)) continue;

                await _mediator.Send(new OrganisationDeleteViewModel {Id = org.Id});

                _logger.LogInformation($"Organisation with ID {org.Id} and EPAOgId {org.EndPointAssessorOrganisationId} no longer found on Register. Deleting from Repository");
            }
        }
    }
}