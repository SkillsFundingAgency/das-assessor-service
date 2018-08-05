using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Web.Staff.ViewModels
{
    public class SearchRequestViewModel
    {
        public string Uln { get; set; }
        public string Surname { get; set; }
        public int? Ukprn { get; set; }
        public IEnumerable<ResultViewModel> SearchResults { get; set; }
    }

    public class StandardViewModelRequest
    {
        public long Uln { get; set; }
        public string Surname { get; set; }
        public int UkPrn { get; set; }
        public string Username { get; set; }
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

    public class StandardSearchResult
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