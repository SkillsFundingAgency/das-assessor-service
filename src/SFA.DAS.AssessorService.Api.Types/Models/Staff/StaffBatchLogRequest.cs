using MediatR;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Api.Types.Models.Staff
{
    public class StaffBatchLogRequest : IRequest<PaginatedList<StaffBatchLogResult>>
    {
        public StaffBatchLogRequest(int page)
        {
            Page = page;
        }

        public int Page { get; set; }
    }
}
