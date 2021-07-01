using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.ApplyTypes
{
    public class ApplicationSummaryItem
    {
        public Guid ApplicationId { get; set; }
        public int SequenceNo { get; set; }
        public string OrganisationName { get; set; }
        public string StandardName { get; set; }
        public int? StandardCode { get; set; }
        public string StandardReference { get; set; }
        public string Standard => StandardCode.HasValue ? $"{StandardName} ({StandardCode})" : StandardName;
        public List<string> Versions { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public DateTime? FeedbackAddedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public int SubmissionCount { get; set; }
        public string ApplicationStatus { get; set; }
        public string ReviewStatus { get; set; }
        public string FinancialStatus { get; set; }
        public string FinancialGrade { get; set; }
        public string SequenceStatus { get; set; } // NOTE: Only used for Closed Applications
        public string StandardApplicationType { get; set; }
        public string WithdrawalType { get; set; }
    }
}
