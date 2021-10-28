using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Certificates
{
    public class GetBatchCertificateLogsResponse
    {
        public string CertificateReference { get; set; }
        public IEnumerable<BatchCertificateLog> CertificateLogs { get; set; }
        public List<string> ValidationErrors { get; set; } = new List<string>();
    }

    public class BatchCertificateLog
    {
        public string Status { get; set; }
        public DateTime? EventTime { get; set; }
    }
}
