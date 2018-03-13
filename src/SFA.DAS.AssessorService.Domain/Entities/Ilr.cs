using System;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class Ilr
    {
        public Guid Id { get; set; }
        public long Uln { get; set; }
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Sex { get; set; }
        public long UkPrn { get; set; }
        public string StdCode { get; set; }
        public DateTime LearnStartDate { get; set; }
        public string EpaOrgId { get; set; }
        public string Outcome { get; set; }
        public DateTime AchDate { get; set; }
        public string OutGrade { get; set; }
    }
}