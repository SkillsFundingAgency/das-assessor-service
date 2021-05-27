using SFA.DAS.AssessorService.Domain.DTOs.Staff;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models.Staff
{
    public class LearnerDetailResult
    {
        public long Uln { get; set; }
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }
        public int StandardCode { get; set; }
        public string Standard { get; set; }
        public string StandardVersion { get; set; }
        public int Level { get; set; }
        public int? FundingModel { get; set; }
        public int? CompletionStatus { get; set; }
        public string CompletionStatusDescription { get; set; }
        public bool? IsPrivatelyFunded { get; set; }
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
        public List<CertificateLogSummary> CertificateLogs { get; set; }
        public string ReasonForChange { get; set; }
        public DateTime? PrintStatusAt { get; set; }
        public string ContactName { get; set; }
        public string ContactOrganisation { get; set; }
        public string ContactAddLine1 { get; set; }
        public string ContactAddLine2 { get; set; }
        public string ContactAddLine3 { get; set; }
        public string ContactAddLine4 { get; set; }
        public string ContactPostCode { get; set; }
    }
}