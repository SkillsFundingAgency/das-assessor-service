using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.AssessorService.Api.Types.Models
{
    public class ImportLearnerDetailRequest : IRequest<ImportLearnerDetailResponse>
    {
        public List<ImportLearnerDetail> ImportLearnerDetails { get; set; }
    }

    public class ImportLearnerDetail
    {
        [Required]
        public string Source { get; set; }
        [Required]
        public int? Ukprn { get; set; }
        [Required]
        public long? Uln { get; set; }
        [Required]
        public int? StdCode { get; set; }
        [Required]
        public int? FundingModel { get; set; }
        [Required]
        public string GivenNames { get; set; }
        [Required]
        public string FamilyName { get; set; }
        public string EpaOrgId { get; set; }
        [Required]
        public DateTime? LearnStartDate { get; set; }
        [Required]
        public DateTime? PlannedEndDate { get; set; }
        [Required]
        public int? CompletionStatus { get; set; }
        [Required]
        public string LearnRefNumber { get; set; }
        [Required]
        public string DelLocPostCode { get; set; }
        public DateTime? LearnActEndDate { get; set; }
        public int? WithdrawReason { get; set; }
        public int? Outcome { get; set; }
        public DateTime? AchDate { get; set; }
        public string OutGrade{ get; set; }
    }
}