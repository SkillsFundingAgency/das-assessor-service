using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.ao.GetEpaOrganisationsByStandard
{
    public class GetEpaOrganisationsByStandardQueryHandler : IRequestHandler<GetEpaOrganisationsByStandardQuery, GetEpaOrganisationsByStandardResponse>
    {
        private readonly IOrganisationQueryRepository _repository;
        
        public GetEpaOrganisationsByStandardQueryHandler(IOrganisationQueryRepository repository)
        {
            _repository = repository;
            
        }

        public async Task<GetEpaOrganisationsByStandardResponse> Handle(GetEpaOrganisationsByStandardQuery request, CancellationToken cancellationToken)
        {
            var organisationsByStandard = await _repository.GetOrganisationsByStandard(request.Standard);

            return new GetEpaOrganisationsByStandardResponse{
                EpaOrganisations = organisationsByStandard
            };
        }
    }
}