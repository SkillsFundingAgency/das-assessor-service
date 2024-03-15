using MediatR;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class ImportLearnerDetailRequest : IRequest<ImportLearnerDetailResponse>
    {
        public List<ImportLearnerDetail> ImportLearnerDetails { get; set; }
    }

    public class ImportLearnerDetail
    {
        public string Source { get; set; }
        public int? Ukprn { get; set; }
        public long? Uln { get; set; }
        public int? StdCode { get; set; }
        public int? FundingModel { get; set; }
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }
        public string EpaOrgId { get; set; }
        public DateTime? LearnStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public int? CompletionStatus { get; set; }
        public string LearnRefNumber { get; set; }
        public string DelLocPostCode { get; set; }
        public DateTime? LearnActEndDate { get; set; }
        public int? WithdrawReason { get; set; }
        public int? Outcome { get; set; }
        public DateTime? AchDate { get; set; }
        public string OutGrade { get; set; }
    }
}