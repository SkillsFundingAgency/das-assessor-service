using System;

namespace SFA.DAS.AssessorService.Domain.DTOs
{
    public class CertificatePrintSummaryBase
    {
        public string Type { get; set; }
        public string CertificateReference { get; set; }
        public string BatchNumber { get; set; }
        public int? ProviderUkPrn { get; set; }
        public string LearnerGivenNames { get; set; }
        public string LearnerFamilyName { get; set; }
        public string ContactName { get; set; }
        public string ContactAddLine1 { get; set; }
        public string ContactAddLine2 { get; set; }
        public string ContactAddLine3 { get; set; }
        public string ContactAddLine4 { get; set; }
        public string ContactPostCode { get; set; }
        public DateTime? AchievementDate { get; set; }
        public string FullName { get; set; }
        public string Status { get; set; }
    }
}
