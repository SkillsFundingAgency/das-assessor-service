using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class CertificatePrintStatus
    {
        public string CertificateReference { get; set; }
        public int BatchNumber { get; set; }
        public string Status { get; set; }
        public DateTime StatusChangedAt { get; set; }
        public string ReasonForChange { get; set; }
    }
}