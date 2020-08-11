using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Domain.DTOs.Staff
{
    public class GetBatchLogsResult
    {
        public IEnumerable<BatchLogSummary> PageOfResults { get; set; }

        public int TotalCount { get; set; }
    }
}
