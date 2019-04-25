using MediatR;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class GetEpaoRegisteredStandardsRequest : IRequest<PaginatedList<GetEpaoRegisteredStandardsResponse>>
    {
        public GetEpaoRegisteredStandardsRequest(string epaoId, int? pageIndex)
        {
            EpaoId = epaoId;
            PageIndex = pageIndex;
        }

        public int? PageIndex { get; private set; }
        public string EpaoId { get; private set; }
    }
}
