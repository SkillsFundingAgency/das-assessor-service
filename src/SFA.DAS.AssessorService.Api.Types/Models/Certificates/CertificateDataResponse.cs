using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class CertificateDataResponse
    {
        public string LearnerGivenNames { get; set; }
        public string LearnerFamilyName { get; set; }
        public string StandardReference { get; set; }
        public string StandardName { get; set; }
        public int StandardLevel { get; set; }
        public bool CoronationEmblem { get; set; }
        public DateTime? StandardPublicationDate { get; set; }
        public string ContactName { get; set; }
        public string ContactOrganisation { get; set; }
        public string ContactAddLine1 { get; set; }
        public string ContactAddLine2 { get; set; }
        public string ContactAddLine3 { get; set; }
        public string ContactAddLine4 { get; set; }
        public string ContactPostCode { get; set; }
        public string Registration { get; set; }
        public string ProviderName { get; set; }
        public DateTime LearningStartDate { get; set; }
        public DateTime? AchievementDate { get; set; }
        public string CourseOption { get; set; }
        public string OverallGrade { get; set; }
        public string Department { get; set; }
        public string FullName { get; set; }
        public string FrameworkCertificateNumber { get; set; }
        public string FrameworkName { get; set; }
        public string FrameworkLevelName { get; set; }
        public string PathwayName { get; set; }
        public string TrainingCode { get; set; }
    }
}