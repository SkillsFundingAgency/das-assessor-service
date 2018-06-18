using System;
using System.Collections.Generic;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Web.ViewModels.Search
{
    public class SearchRequestViewModel
    {
        public string Uln { get; set; }
        public string Surname { get; set; }
        public IEnumerable<ResultViewModel> SearchResults { get; set; }
    }

    public class ChooseStandardViewModel
    {
        public string SelectedStandardCode { get; set; }
        public IEnumerable<ResultViewModel> SearchResults { get; set; }
    }

    public class SelectedStandardViewModel
    {
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }
        public string Uln { get; set; }
        public string Standard { get; set; }
        public string StdCode { get; set; } 
        public string OverallGrade { get; set; }
        public string CertificateReference { get; set; }
        public string Level { get; set; }
        public string SubmittedAt { get; set; }
        public string SubmittedBy { get; set; }
        public string LearnerStartDate { get; set; }
        public string AchievementDate { get; set; }
        
        public bool ShowExtraInfo { get; set; }
    }
}