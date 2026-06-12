using System;

namespace SFA.DAS.AssessorService.Domain.DTOs.Staff
{
    public class CertificateBatchLogSummary
    {
        public int BatchNumber { get; set; }
        public DateTime StatusAt { get; set; }
        public string Status { get; set; }
        public string CertificateData { get; set; }
        public string CertificateReference { get; set; }
        public long Uln { get; set; }
        public int StandardCode { get; set; }
        public Guid? FrameworkLearnerId { get; set; }
    }
}
