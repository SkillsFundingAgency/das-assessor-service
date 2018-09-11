using MediatR;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Api.Types.Models.Staff
{
    public class StaffBatchSearchRequest : IRequest<PaginatedList<StaffBatchSearchResult>>
    {
        public StaffBatchSearchRequest(int batchNumber, int page)
        {
            BatchNumber = batchNumber;
            Page = page;
        }

        public int BatchNumber { get; set; }
        public int Page { get; set; }
    }
}
