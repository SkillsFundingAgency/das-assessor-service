﻿using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.Certificates
{
    public class CertificateHistoryResponse
    {
        public string CertificateReference { get; set; }
        public long Uln { get; set; }
        public string FullName { get; set; }
        public string TrainingProvider { get; set; }
        public DateTime CreatedAt { get; set; }

        public string StandardName { get; set; }
        public string OverallGrade { get; set; }
        public string CourseOption { get; set; }
        public DateTime? AchievementDate { get; set; }

        public string ContactOrganisation { get; set; }
        public string ContactAddLine1 { get; set; }
        public string ContactAddLine2 { get; set; }
        public string ContactAddLine3 { get; set; }
        public string ContactAddLine4 { get; set; }
        public string ContactPostCode { get; set; }
    }
}
