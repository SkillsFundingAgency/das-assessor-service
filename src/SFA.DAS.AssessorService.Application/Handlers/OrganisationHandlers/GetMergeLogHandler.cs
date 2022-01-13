using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;

namespace SFA.DAS.AssessorService.Application.Handlers.OrganisationHandlers
{
    public class GetMergeLogHandler : IRequestHandler<GetMergeLogRequest, GetMergeLogResponse>
    {
        private readonly IOrganisationQueryRepository _organisationQueryRepository;

        public GetMergeLogHandler(IOrganisationQueryRepository organisationQueryRepository)
        {
            _organisationQueryRepository = organisationQueryRepository;
        }

        public async Task<GetMergeLogResponse> Handle(GetMergeLogRequest request,
            CancellationToken cancellationToken)
        {
            var mergelog = await _organisationQueryRepository.GetAllMergeOrganisations();
            return new GetMergeLogResponse() { MergeOrganisations = mergelog };
        }
    }
}