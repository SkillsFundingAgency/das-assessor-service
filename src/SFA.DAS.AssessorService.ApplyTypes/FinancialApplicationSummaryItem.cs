using System;

namespace SFA.DAS.AssessorService.ApplyTypes
{
    public class FinancialApplicationSummaryItem
    {
        public Guid ApplicationId { get; set; }
        public int SequenceNo { get; set; }
        public int SectionNo { get; set; }
        public string OrganisationName { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public DateTime? FeedbackAddedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public int SubmissionCount { get; set; }
        public string ApplicationStatus { get; set; }
        public string ReviewStatus { get; set; }
        public string FinancialStatus { get; set; }
        public FinancialGrade Grade { get; set; }
    }
}
