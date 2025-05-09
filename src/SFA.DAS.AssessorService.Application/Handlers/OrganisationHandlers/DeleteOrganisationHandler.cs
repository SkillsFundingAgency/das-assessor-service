﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Data.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.OrganisationHandlers
{
    public class DeleteOrganisationHandler : IRequestHandler<DeleteOrganisationRequest, Unit>
    {
        private readonly IOrganisationRepository _organisationRepository;

        public DeleteOrganisationHandler(IOrganisationRepository organisationRepository)
        {
            _organisationRepository = organisationRepository;
        }

        public async Task<Unit> Handle(DeleteOrganisationRequest deleteOrganisationRequest, CancellationToken cancellationToken)
        {
            await _organisationRepository.Delete(deleteOrganisationRequest.EndPointAssessorOrganisationId);
            return Unit.Value;
        }
    }
}