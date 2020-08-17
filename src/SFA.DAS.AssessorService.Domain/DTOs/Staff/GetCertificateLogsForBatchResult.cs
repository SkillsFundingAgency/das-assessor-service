using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Domain.DTOs.Staff
{
    public class GetCertificateLogsForBatchResult
    {
        public DateTime SentToPrinterAt { get; set; }
        public DateTime? PrintedAt { get; set; }

        public IEnumerable<CertificateBatchLogSummary> PageOfResults { get; set; }

        public int TotalCount { get; set; }
    }
}
