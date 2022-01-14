using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Application.Handlers.OrganisationHandlers
{
    public class GetMergeLogHandler : IRequestHandler<GetMergeLogRequest, PaginatedList<MergeOrganisation>>
    {
        private readonly IOrganisationQueryRepository _organisationQueryRepository;

        public GetMergeLogHandler(IOrganisationQueryRepository organisationQueryRepository)
        {
            _organisationQueryRepository = organisationQueryRepository;
        }

        public async Task<PaginatedList<MergeOrganisation>> Handle(GetMergeLogRequest request,
            CancellationToken cancellationToken)
        {
            var mergelog = await _organisationQueryRepository.GetAllMergeOrganisations(request.PageSize ?? 1, request.PageIndex ?? 0, request.PrimaryEPAOId, request.SecondaryEPAOId);
            return mergelog;
        }
    }
}