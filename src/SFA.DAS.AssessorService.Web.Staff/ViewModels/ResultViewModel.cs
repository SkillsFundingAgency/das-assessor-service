using System;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels
{
    public class ResultViewModel
    {
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }
        public string Uln { get; set; }
        public string Standard { get; set; }
        public string StdCode { get; set; } 
        public string OverallGrade { get; set; }
        public string CertificateReference { get; set; }
        public string Level { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public string SubmittedBy { get; set; }
        public DateTime? AchDate { get; set; }
        public DateTime? LearnStartDate { get; set; }
        public bool ShowExtraInfo { get; set; }
    }
}