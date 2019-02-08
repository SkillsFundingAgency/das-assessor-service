using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Web.Staff.Controllers.Apply;

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

        public static string TranslateFinancialStatus(string financeStatus, string grade)
        {
            switch (financeStatus)
            {
                case ApplicationSectionStatus.Submitted:
                    return "Not started";
                case ApplicationSectionStatus.InProgress:
                    return "In Progress";
                case ApplicationSectionStatus.Graded:
                case ApplicationSectionStatus.Evaluated:
                    switch(grade)
                    {
                        case FinancialApplicationSelectedGrade.Outstanding:
                        case FinancialApplicationSelectedGrade.Good:
                        case FinancialApplicationSelectedGrade.Satisfactory:
                            return "Passed";
                        case FinancialApplicationSelectedGrade.Exempt:
                            return "Exempt";
                        case FinancialApplicationSelectedGrade.Inadequate:
                            return "Rejected";
                    }
                    break;
            }

            return "";
        }

        public static string ApplicationBacklinkAction(string sequenceStatus, int? sequenceId)
        {
            switch(sequenceStatus)
            {
                case ApplicationSequenceStatus.FeedbackAdded:
                    return nameof(ApplicationController.RejectedApplications);
                case ApplicationSequenceStatus.Approved:
                case ApplicationSequenceStatus.Rejected:
                    return nameof(ApplicationController.ClosedApplications);
                case null:
                default:
                    switch (sequenceId)
                    {
                        case 2:
                            return nameof(ApplicationController.StandardApplications);
                        case 1:
                        case null:
                        default:
                            return nameof(ApplicationController.MidpointApplications);
                    }
            }
        }
    }
}