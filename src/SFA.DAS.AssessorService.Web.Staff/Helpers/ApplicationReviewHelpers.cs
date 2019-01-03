namespace SFA.DAS.AssessorService.Web.Staff.Helpers
{
    public static class ApplicationReviewHelpers
    {
        public static string TranslateApplicationStatus(string sequenceStatus)
        {
            switch (sequenceStatus)
            {
                case "Submitted" :
                    return "Not started";
                case "In Progress" :
                    return "Evaluation started";
                case "Completed":
                    return "Evaluated";
            }

            return "";
        }

        public static string TranslateFinanceStatus(string financeStatus)
        {
            switch (financeStatus)
            {
                case "Submitted" :
                    return "Not started";
                case "In Progress" :
                    return "Evaluation started";
                case "Graded":
                case "Completed":
                    return "Evaluated";
            }

            return "";
        }

        public static string ReadyToFeedback(string financeStatus, string sequenceStatus)
        {
            return (financeStatus == "Completed" && sequenceStatus == "Completed") ? "Yes" : "No";
        }

        public static string ApplicationLink(string status)
        {
            return status == "Submitted" ? "Evaluate application" : "Continue";
        }
    }
}