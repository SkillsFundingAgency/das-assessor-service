using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class CertificateSummaryResponse
    {
        public string CertificateReference { get; set; }
        public string RecordedBy { get; set; }
        public long Uln { get; set; }
        public string FullName { get; set; }
        public string TrainingProvider { get; set; }
        public DateTime CreatedAt { get; set; }

        public string StandardName { get; set; }
        public int Level { get; set; }
        public string OverallGrade { get; set; }
        public string CourseOption { get; set; }

        public DateTime? LearningStartDate { get; set; }
        public DateTime? AchievementDate { get; set; }

        public string ContactOrganisation { get; set; }
        public string ContactName { get; set; }
        public string ContactAddLine1 { get; set; }
        public string ContactAddLine2 { get; set; }
        public string ContactAddLine3 { get; set; }
        public string ContactAddLine4 { get; set; }
        public string ContactPostCode { get; set; }
        public string Status { get; set; }
        public string PrivatelyFundedStatus { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Ukprn { get; set; }
        public int StandardCode { get; set; }
        public string EpaoId { get; set; }
        public string EpaoName { get; set; }
    }
}
