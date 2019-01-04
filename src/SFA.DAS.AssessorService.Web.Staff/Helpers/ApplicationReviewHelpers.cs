using SFA.DAS.AssessorService.ApplyTypes;

namespace SFA.DAS.AssessorService.Web.Staff.Helpers
{
    public static class ApplicationReviewHelpers
    {
        public static string TranslateApplicationStatus(string sequenceStatus)
        {
            switch (sequenceStatus)
            {
                case ApplicationSectionStatus.Submitted:
                    return "Not started";
                case ApplicationSectionStatus.InProgress:
                    return "Evaluation started";
                case ApplicationSectionStatus.Evaluated:
                    return "Evaluated";
            }

            return "";
        }

        public static string TranslateFinanceStatus(string financeStatus)
        {
            switch (financeStatus)
            {
                case ApplicationSectionStatus.Submitted:
                    return "Not started";
                case ApplicationSectionStatus.InProgress:
                    return "Evaluation started";
                case ApplicationSectionStatus.Graded:
                case ApplicationSectionStatus.Evaluated:
                    return "Evaluated";
            }

            return "";
        }

        public static string ReadyToFeedback(string financeStatus, string sequenceStatus)
        {
            return (financeStatus == ApplicationSectionStatus.Evaluated && sequenceStatus == ApplicationSectionStatus.Evaluated) ? "Yes" : "No";
        }

        public static string ApplicationLink(string status)
        {
            return status == ApplicationSectionStatus.Submitted ? "Evaluate application" : "Continue";
        }
    }
}