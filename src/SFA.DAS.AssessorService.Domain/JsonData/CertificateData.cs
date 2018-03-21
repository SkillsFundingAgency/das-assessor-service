using System;

namespace SFA.DAS.AssessorService.Domain.JsonData
{
    public class CertificateData
    {
        public string LearnerGivenNames { get; set; }
        public string LearnerFamilyName { get; set; }
        public DateTime LearnerDateofBirth { get; set; }
        public string LearnerSex { get; set; }
        public string StandardName { get; set; }
        public int StandardLevel { get; set; }
        public DateTime StandardPublicationDate { get; set; }
        public string ContactName { get; set; }
        public string ContactOrganisation { get; set; }
        public string ContactAddLine1 { get; set; }
        public string ContactAddLine2 { get; set; }
        public string ContactAddLine3 { get; set; }
        public string ContactAddLine4 { get; set; }
        public string ContactPostCode { get; set; }
        public string Registration { get; set; }
        public DateTime LearningStartDate { get; set; }
        public string AchievementOutcome { get; set; }
        public DateTime? AchievementDate { get; set; }
        public string CourseOption { get; set; }
        public string OverallGrade { get; set; }
        public string Department { get; set; }
    }
}