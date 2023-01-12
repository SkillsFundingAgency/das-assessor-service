using MediatR;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

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

            return new GetEpaOrganisationsByStandardResponse
            {
                EpaOrganisations = organisationsByStandard
            };
        }
    }
}