using System;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class SearchResult
    {
        public long Uln { get; set; }
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string StdCode { get; set; }
        public string Standard { get; set; }
        public DateTime LearnStartDate { get; set; }
        public string Outcome { get; set; }
        public DateTime AchDate { get; set; }
        public string OutGrade { get; set; }
    }
}