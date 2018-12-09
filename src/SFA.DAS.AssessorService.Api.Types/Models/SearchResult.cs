using System;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class SearchResult
    {
        public long Uln { get; set; }
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int StdCode { get; set; }
        public string Standard { get; set; }
        public int Level { get; set; }
        public int FundingModel { get; set; }
        public int UkPrn { get; set; }
        public string Option { get; set; }
        public DateTime? LearnStartDate { get; set; }
        public string OverallGrade { get; set; }
        public DateTime? AchDate { get; set; }
        public Guid CertificateId { get; set; }
        public string CertificateReference { get; set; }
        public string CertificateStatus { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public string SubmittedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public bool ShowExtraInfo { get; set; }
        public bool RemoveFromCollection { get; set; }
    }
}