using SFA.DAS.AssessorService.Domain.Paging;
using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Staff
{
    public class StaffBatchSearchResponse
    {
        public DateTime SentToPrinterDate { get; set; }
        public DateTime? PrintedDate { get; set; }
        public PaginatedList<StaffBatchSearchResult> Results { get; set; }
    }

}
