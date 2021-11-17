using SFA.DAS.AssessorService.Api.Types.Models.AO;
using System;

namespace SFA.DAS.AssessorService.Application.Helpers
{
    public static class FinancialReviewStatusHelper
    {
        public static bool IsFinancialExempt(bool? financialExempt, DateTime? financialDueDate, OrganisationType orgType)
        {
            bool exempt = financialExempt ?? false;

            bool orgTypeFinancialExempt = (orgType != null) && orgType.FinancialExempt;

            bool financialIsNotDue = (financialDueDate?.Date ?? DateTime.MinValue) > DateTime.Today;

            return exempt || financialIsNotDue || orgTypeFinancialExempt;
        }
    }
}
