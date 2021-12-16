using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class Learner
    {
        public Guid Id { get; set; }
        public long Uln { get; set; }
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }
        public long? EmployerId { get; set; }
        public string EmployerName { get; set; }

        /// <summary>
        /// Used for search handler, escaping special characters
        /// </summary>
        [NotMapped]
        public string FamilyNameForSearch { get; set; }

        public int UkPrn { get; set; }
        public int StdCode { get; set; }
        public DateTime LearnStartDate { get; set; }
        public string EpaOrgId { get; set; }
        public int? FundingModel { get; set; }
        public long? ApprenticeshipId { get; set; }
        public string Source { get; set; }
        public string LearnRefNumber { get; set; }
        public int? CompletionStatus { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public string DelLocPostCode { get; set; }
        public DateTime? LearnActEndDate { get; set; }
        public int? WithdrawReason { get; set; }
        public int? Outcome { get; set; }
        public DateTime? AchDate { get; set; }
        public string OutGrade { get; set; }

        public string Version { get; set;}
        public bool VersionConfirmed { get; set; }
        public string CourseOption { get; set; }
        public string StandardUId { get; set; }
        public string StandardReference { get; set; }
        public string StandardName { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime? EstimatedEndDate { get; set; }
        public DateTime? ApprovalsStopDate { get; set; }
        public DateTime? ApprovalsPauseDate { get; set; }
        public DateTime? ApprovalsCompletionDate { get; set; }
        public int ApprovalsPaymentStatus { get; set; }
    }
}