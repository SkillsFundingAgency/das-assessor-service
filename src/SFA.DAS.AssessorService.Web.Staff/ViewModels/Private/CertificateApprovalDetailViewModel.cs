using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels.Private
{
    public class CertificateDetailApprovalViewModel
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
        public string IsApproved { get; set; }
    }
}