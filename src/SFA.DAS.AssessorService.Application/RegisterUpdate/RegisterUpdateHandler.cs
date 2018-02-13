﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessmentOrgs.Api.Client.Core;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

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
            var organisations = await _organisationRepository.GetAllOrganisations();
            foreach (var epaoSummary in epaosOnRegister)
            {
                if (!organisations.Any(o => o.EpaoOrgId == epaoSummary.Id))
                {
                    var epao = _apiClient.Get(epaoSummary.Id);
                    await _organisationRepository.CreateNewOrganisation(new Organisation()
                    {
                        EpaoOrgId = epao.Id,
                        Name = epao.Name
                    });
                }
            }
        }
    }
}