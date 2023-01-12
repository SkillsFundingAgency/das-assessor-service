namespace SFA.DAS.AssessorService.Domain.Consts
{
    public class ApplicationTypes
    {
        /// <summary>
        /// The original EPAO application type which includes: 
        ///     Organisation and Standard with Financial Health Checks
        ///     Organisation and Standard without Financial Health Checks (Exempt org type)
        ///     Additional Standard with Financial Health Checks 
        ///     Additional Standard without Financial Health Checks (Exempt org type or financials not expired)
        /// </summary>
        public const string Initial = "Initial";

        public const string InitialWithFinancialHealthChecks = "InitialWithFinancialHealthChecks";

        public const string InitialWithoutFinancialHealthChecks = "InitialWithFinancialHealthChecks";

        public const string AdditionalStandard = "AdditionalStandard";

        public const string AdditionalStandardWithFinancialHealthChecks = "AdditionalStandardWithFinancialHealthChecks";

        public const string AdditionalStandardWithoutFinancialHealthChecks = "AdditionalStandardWithoutFinancialHealthChecks";

        public const string Organisation = "Organisation";

        public const string Standard = "Standard";

        public const string Withdrawal = "Withdrawl";

        public const string OrganisationWithdrawal = "OrganisationWithdrawal";

        public const string StandardWithdrawal = "StandardWithdrawal";
    }
}
