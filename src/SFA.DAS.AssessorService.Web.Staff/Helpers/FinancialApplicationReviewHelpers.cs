using SFA.DAS.AssessorService.ApplyTypes;

namespace SFA.DAS.AssessorService.Web.Staff.Helpers
{
    public static class FinancialApplicationReviewHelpers
    {
        public static string TranslateApplicationStatus(string sequenceStatus)
        {
            switch (sequenceStatus)
            {
                case ApplicationSectionStatus.Submitted:
                    return "Not started";
                case ApplicationSectionStatus.InProgress:
                    return "Review started";
                case ApplicationSectionStatus.Graded:
                case ApplicationSectionStatus.Evaluated:
                    return "Reviewed";
            }

            return "";
        }

        public static string ApplicationLink(string status)
        {
            return status == ApplicationSectionStatus.Submitted ? "Evaluate application" : "Continue";
        }
    }
}