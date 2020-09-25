namespace SFA.DAS.AssessorService.Domain.Consts
{
    public class ApplicationTypes
    {
        /// <summary>
        /// The original EPAO application type which includes: 
        ///     Organisation + Initial Standard with Financial Health Checks
        ///     Organisation + Initial Standard without Financial Health Checks (Exempt org type)
        ///     Additional Standard with Financial Health Checks 
        ///     Additional Standard without Financial Health Checks (Exempt org type or financials not expired)
        /// </summary>
        public const string Combined = "Apply";

        public const string Organisation = "Organisation";

        public const string Standard = "Standard";

        public const string Withdrawal = "Withdrawl";

        public const string OrganisationWithdrawal = "OrganisationWithdrawal";

        public const string StandardWithdrawal = "StandardWithdrawal";
    }
}
