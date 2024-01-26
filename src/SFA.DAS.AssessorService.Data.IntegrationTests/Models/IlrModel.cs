using System;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Models
{
    public class IlrModel : TestModel
    {
        public Guid? Id { get; set; }
        public long Uln { get; set; }
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }
        public int Ukprn { get; set; }
        public int StdCode { get; set; }
        public DateTime? LearnStartDate { get; set; }
        public int? FundingModel { get; set; }
        public string Source { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CompletionStatus { get; set; }
        public DateTime? PlannedEndDate { get; set; }
    }
}
