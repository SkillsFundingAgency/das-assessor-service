using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.ViewModels.Shared;
using System.Collections.Generic;
using System.Linq;

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
        public string StdCode { get; set; }
        public IEnumerable<ResultViewModel> SearchResults { get; set; }
        public bool AllAssessmentsCompleted => !SearchResults.Any(r => r.CertificateReference == null || r.OverallGrade == CertificateGrade.Fail);
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
        public string Version { get; set; }
        public string SubmittedAt { get; set; }
        public string SubmittedBy { get; set; }
        public string LearnerStartDate { get; set; }
        public string AchievementDate { get; set; }
        public bool UlnAlreadyExists { get; set; }
        public bool ShowExtraInfo { get; set; }
        public bool IsNoMatchingFamilyName { get; set; }
        public IEnumerable<StandardVersionViewModel> Versions { get; set; }

        public bool OnlySingleVersion => Versions != null && Versions.Count() == 1;
        public bool OnlySingleOption => OnlySingleVersion && Versions.Single().Options != null && Versions.Single().Options.Count() == 1;
    }
}