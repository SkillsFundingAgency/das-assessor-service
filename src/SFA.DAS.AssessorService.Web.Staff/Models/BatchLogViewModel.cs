using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Web.Staff.Models
{
    public class BatchLogViewModel
    {
        public int BatchNumber { get; set; } // TODO: Is this required? Could we even have just the one viewmodel?
        public int Page { get; set; }
        public PaginatedList<StaffBatchLogResult> PaginatedList { get; set; }
    }
}
