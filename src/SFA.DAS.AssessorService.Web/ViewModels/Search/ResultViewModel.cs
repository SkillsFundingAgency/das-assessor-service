using SFA.DAS.AssessorService.Web.ViewModels.Shared;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.ViewModels.Search
{
    public class ResultViewModel
    {
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }
        public string Uln { get; set; }
        public string Standard { get; set; }
        public string StdCode { get; set; }
        public string StandardReferenceNumber { get; set; }
        public IEnumerable<StandardVersionViewModel> Versions { get; set; } = new List<StandardVersionViewModel>();
        public string OverallGrade { get; set; }
        public string CertificateReference { get; set; }
        public string CertificateStatus { get; set; }
        public bool IsPrivatelyFunded { get; set; }
        public string Level { get; set; }
        public string Version { get; set; }
        public bool VersionConfirmed { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public string SubmittedBy { get; set; }
        public DateTime? AchDate { get; set; }
        public DateTime? LearnStartDate { get; set; }
        public bool ShowExtraInfo { get; set; }
        public bool UlnAlreadyExists { get; set; }
        public bool IsNoMatchingFamilyName { get; set; }
    }
}