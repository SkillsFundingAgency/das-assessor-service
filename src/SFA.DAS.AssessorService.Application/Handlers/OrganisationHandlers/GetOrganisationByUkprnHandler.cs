using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.OrganisationHandlers 
{
    public class GetOrganisationByUkprnHandler : IRequestHandler<GetOrganisationByUkprnRequest, OrganisationResponse> 
    {
        private readonly IOrganisationQueryRepository _organisationQueryRepository;

        public GetOrganisationByUkprnHandler (IOrganisationQueryRepository organisationQueryRepository) 
        {
            _organisationQueryRepository = organisationQueryRepository;
        }

        public async Task<OrganisationResponse> Handle (GetOrganisationByUkprnRequest request, CancellationToken cancellationToken) 
        {
            var result = await _organisationQueryRepository.GetByUkPrn(request.Ukprn);
            return Mapper.Map<OrganisationResponse>(result);
        }
    }
}