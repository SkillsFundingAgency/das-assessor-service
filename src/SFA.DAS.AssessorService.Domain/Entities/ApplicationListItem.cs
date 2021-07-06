using System;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    // Represents a row in the Apply_List_Applications stored proc response
    public class ApplicationListItem
    {
        public Guid ApplicationId { get; set; }
        public int SequenceNo { get; set; }
        public string OrganisationName { get; set; }
        public string EndPointAssessorOrganisationId { get; set; }
        public string StandardName { get; set; }
        public int? StandardCode { get; set; }
        public string StandardReference { get; set; }
        public string Versions { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public DateTime? FeedbackAddedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public int SubmissionCount { get; set; }
        public string ApplicationStatus { get; set; }
        public string StandardApplicationType { get; set; }
        public string ReviewStatus { get; set; }
        public string FinancialStatus { get; set; }
        public string FinancialGrade { get; set; }
        public string SequenceStatus { get; set; }
    }
}
