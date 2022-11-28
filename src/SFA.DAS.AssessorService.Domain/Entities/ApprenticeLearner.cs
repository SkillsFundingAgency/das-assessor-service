using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    /// <summary>
    /// Domain model object over Learner + Provider for Learner endpoint
    /// </summary>
    public class ApprenticeLearner
    {
        public long? ApprenticeshipId { get; set; }
        public int UkPrn { get; set; }
        public DateTime LearnStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public int StdCode { get; set; }
        public string StandardUId { get; set; }
        public string StandardReference { get; set; }
        public string StandardName { get; set; }
        public int? CompletionStatus { get; set; }
        public DateTime? ApprovalsStopDate { get; set; }
        public DateTime? ApprovalsPauseDate { get; set; }
        public DateTime? EstimatedEndDate { get; set; }
        public DateTime? LearnActEndDate { get; set; }
        public long Uln { get; set; }
        public string GivenNames { get; set; }
        public string FamilyName { get; set; }
        public bool IsTransfer { get; set; }
        public DateTime? DateTransferIdentified { get; set; }
        /// <summary>
        /// From Provider Table
        /// </summary>
        public string ProviderName { get; set; }
        
    }
}