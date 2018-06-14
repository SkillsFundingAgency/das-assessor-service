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
        public DateTime? LearnStartDate { get; set; }
        public string OverallGrade { get; set; }
        public DateTime? AchDate { get; set; }
        public string OutGrade { get; set; }
        public string CertificateReference { get; set; }
        public int Level { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public string SubmittedBy { get; set; }
        public bool ShowExtraInfo { get; set; }
    }
}