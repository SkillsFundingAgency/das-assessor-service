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
                case "Evaluated":
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
                    return "Evaluated";
            }

            return "";
        }

        public static string ReadyToFeedback(string financeStatus, string sequenceStatus)
        {
            return (financeStatus == "Graded" && sequenceStatus == "Evaluated") ? "Yes" : "No";
        }
    }
}