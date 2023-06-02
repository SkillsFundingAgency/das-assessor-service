namespace SFA.DAS.AssessorService.Domain.Consts
{
    public static class ApprovedStatus
    {
        public const bool OptedIn = true;

        //The below application statuses are now irrelevant following the REPAO changes. 
        public const string Approved = "Approved";
        public const string ApplyInProgress = "Apply in progress";
        public const string NotYetApplied = "Not yet applied";
        public const string Withdrawn = "Withdrawn";
        public const string FeedbackAdded = "Feedback Added";
    }
}
