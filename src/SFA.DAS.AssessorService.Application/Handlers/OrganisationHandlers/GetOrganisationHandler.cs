using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Handlers.OrganisationHandlers
{
    public class GetOrganisationHandler : IRequestHandler<GetOrganisationRequest, Organisation>
    {
        private readonly IOrganisationQueryRepository _organisationQueryRepository;

        public GetOrganisationHandler(IOrganisationQueryRepository organisationQueryRepository)
        {
            _organisationQueryRepository = organisationQueryRepository;
        }

        public async Task<Organisation> Handle(GetOrganisationRequest request,
            CancellationToken cancellationToken)
        {
            var organisation = await _organisationQueryRepository.Get(request.Id);            
            return organisation;
        }
    }
}