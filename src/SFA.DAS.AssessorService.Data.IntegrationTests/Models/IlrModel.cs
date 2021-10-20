using System;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Models
{
    public class IlrModel : TestModel
    {
        public Guid Id { get; set; }
        public string Source { get; set; }
        public int UkPrn { get; set; }
        public long Uln { get; set; }
        public int StdCode { get; set; }
        public int? FundingModel { get; set; }
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }
        public string EpaOrgId { get; set; }
        public DateTime LearnStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public int? CompletionStatus { get; set; }
        public string LearnRefNumber { get; set; }
        public string DelLocPostCode { get; set; }
        public DateTime? LearnActEndDate { get; set; }
        public int? WithdrawReason { get; set; }
        public int? Outcome { get; set; }
        public DateTime? AchDate { get; set; }
        public string OutGrade { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
