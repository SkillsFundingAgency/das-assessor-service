using System;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class StaffSearchResult
    {
        public long Uln { get; set; }
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }
        public int StandardCode { get; set; }
        public string Standard { get; set; }
        public string CertificateReference { get; set; }
        public string CertificateStatus { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
    }
}