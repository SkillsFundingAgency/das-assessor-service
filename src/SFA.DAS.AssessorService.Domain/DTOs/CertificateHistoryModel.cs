using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Domain.DTOs
{
    public class CertificateHistoryModel
    {
        public ICollection<CertificateLog> CertificateLogs { get; set; } = new List<CertificateLog>();

        public Guid Id { get; set; }
        public long Uln { get; set; }
        public int StandardCode { get; set; }
        public string StandardUId { get; set; }
        public int ProviderUkPrn { get; set; }
        public Guid OrganisationId { get; set; }

        public string CertificateReference { get; set; }
        public int? CertificateReferenceId { get; set; }

        public int? BatchNumber { get; set; }

        public string Status { get; set; }

        public DateTime? ToBePrinted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public string LearnRefNumber { get; set; }

        public DateTime CreateDay { get; set; }

        public string FullName { get; set; }

        public string ContactOrganisation { get; set; }

        public string ProviderName { get; set; }

        public string ContactName { get; set; }

        public string CourseOption { get; set; }

        public string OverallGrade { get; set; }

        public string StandardReference { get; set; }

        public string StandardName { get; set; }

        public string Version { get; set; }

        public int StandardLevel { get; set; }

        public DateTime? AchievementDate { get; set; }

        public DateTime? LearningStartDate { get; set; }

        public string ContactAddLine1 { get; set; }

        public string ContactAddLine2 { get; set; }

        public string ContactAddLine3 { get; set; }

        public string ContactAddLine4 { get; set; }

        public string ContactPostCode { get; set; }

        public int? EndPointAssessorUkprn { get; set; }

        public DateTime? StatusAt { get; set; }
        public string ReasonForChange { get; set; }
    }
}