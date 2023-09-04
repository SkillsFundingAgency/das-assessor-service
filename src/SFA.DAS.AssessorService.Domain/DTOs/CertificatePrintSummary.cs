using System;

namespace SFA.DAS.AssessorService.Domain.DTOs
{
    public class CertificatePrintSummary
    {
        public long Uln { get; set; }
        public int StandardCode { get; set; }
        public int ProviderUkPrn { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }
        public string EndPointAssessorOrganisationName { get; set; }
        public string CertificateReference { get; set; }
        public string BatchNumber { get; set; }
        public string LearnerGivenNames { get; set; }
        public string LearnerFamilyName { get; set; }
        public string StandardName { get; set; }
        public int StandardLevel { get; set; }
        public bool CoronationEmblem { get; set; }
        public string ContactName { get; set; }
        public string ContactOrganisation { get; set; }
        public string ContactAddLine1 { get; set; }
        public string ContactAddLine2 { get; set; }
        public string ContactAddLine3 { get; set; }
        public string ContactAddLine4 { get; set; }
        public string ContactPostCode { get; set; }
        public DateTime? AchievementDate { get; set; }
        public string CourseOption { get; set; }
        public string OverallGrade { get; set; }
        public string Department { get; set; }
        public string FullName { get; set; }
        public string Status { get; set; }
    }
}
