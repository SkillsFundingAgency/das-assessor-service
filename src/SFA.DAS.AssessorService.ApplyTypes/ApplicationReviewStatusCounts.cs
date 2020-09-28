namespace SFA.DAS.AssessorService.ApplyTypes
{
    public class ApplicationReviewStatusCounts
    {
        public int OrganisationApplicationsNew { get; set; }
        public int OrganisationApplicationsInProgress { get; set; }
        public int OrganisationApplicationsHasFeedback { get; set; }
        public int OrganisationApplicationsApproved { get; set; }

        public int StandardApplicationsNew { get; set; }
        public int StandardApplicationsInProgress { get; set; }
        public int StandardApplicationsHasFeedback { get; set; }
        public int StandardApplicationsApproved { get; set; }

        public int WithdrawalApplicationsNew { get; set; }
        public int WithdrawalApplicationsInProgress { get; set; }
        public int WithdrawalApplicationsHasFeedback { get; set; }
        public int WithdrawalApplicationsApproved { get; set; }
    }
}
