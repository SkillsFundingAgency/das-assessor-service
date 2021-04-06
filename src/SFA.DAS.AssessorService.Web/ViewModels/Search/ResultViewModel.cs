using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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
        public List<StandardVersion> Versions { get; set; } = new List<StandardVersion>();
        public string OverallGrade { get; set; }
        public string CertificateReference { get; set; }
        public string Level { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public string SubmittedBy { get; set; }
        public DateTime? AchDate { get; set; }
        public DateTime? LearnStartDate { get; set; }
        public bool ShowExtraInfo { get; set; }
        public bool UlnAlreadyExists { get; set; }
        public bool IsNoMatchingFamilyName { get; set; }

        public class StandardVersion
        {
            public string StandardUId { get; set; }
            public string Title { get; set; }
            public string Version { get; set; }
        }
    }
}