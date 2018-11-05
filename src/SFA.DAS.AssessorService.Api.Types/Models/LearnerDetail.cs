using System;
using System.Collections.Generic;
using SFA.DAS.AssessorService.Domain.DTOs.Staff;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class LearnerDetail
    {
        public long Uln { get; set; }
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }
        public int StandardCode { get; set; }
        public string Standard { get; set; }
        public int Level { get; set; }
        public int FundingModel { get; set; }
        public string Option { get; set; }
        public string OrganisationName { get; set; }
        public string OrganisationId { get; set; }
        public string CertificateReference { get; set; }
        public string CertificateStatus { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public DateTime? LearnStartDate { get; set; }
        public string OverallGrade { get; set; }
        public DateTime? AchievementDate { get; set; }
        public Guid? CertificateId { get; set; }
        public bool IsPrivatelyFunded { get; set; }

        public List<CertificateLogSummary> CertificateLogs { get; set; }
    }
}