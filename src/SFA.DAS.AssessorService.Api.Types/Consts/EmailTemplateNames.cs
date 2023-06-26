namespace SFA.DAS.AssessorService.Api.Types.Consts
{
    public class EmailTemplateNames
    {
        public const string EPAOPrimaryContactAmended = "EPAOPrimaryContactAmended";
        public const string EPAOOrganisationDetailsAmended = "EPAOOrganisationDetailsAmended";
        public const string ApplyEPAOInitialSubmission = "ApplyEPAOInitialSubmission";
        public const string ApplyEPAOStandardSubmission = "ApplyEPAOStandardSubmission";
        public const string ApplyEPAOAlertSubmission = "ApplyEPAOAlertSubmission";
        public const string WithdrawalEPAOSubmission = "WithdrawalEPAOSubmission";
        public const string EPAOWithdrawalFeedbackNotification = "EPAOWithdrawalFeedbackNotification";
        public const string EPAOCancelApplication = "EPAOCancelApplication";
        public const string EPAOStandardWithdrawalApproval = "EPAOStandardWithdrawalApproval";
        public const string EPAORegisterWithdrawalApproval = "EPAORegisterWithdrawalApproval";

        /// <summary>
        /// Requires tokens: { contactname }
        /// </summary>
        public const string APPLY_EPAO_UPDATE = "ApplyEPAOUpdate";

        /// <summary>
        /// Requires tokens: { contactname, standard }
        /// </summary>
        public const string APPLY_EPAO_RESPONSE = "ApplyEPAOResponse";

        /// <summary>
        /// Requires tokens: { contactname, standard, standardreference, version }
        /// </summary>
        public const string ApplyEPAOStandardOptin = "ApplyEPAOStandardOptin";

        /// <summary>
        /// Requires tokens: { primaryEPAO, contactName, effectiveToDate }
        /// </summary>
        public const string MergeConfirmationForPrimaryEpao = "MergeConfirmationForPrimaryEpao";

        /// <summary>
        /// Requires tokens: { secondaryEPAO, contactName, effectiveToDate }
        /// </summary>
        public const string MergeConfirmationForSecondaryEpao = "MergeConfirmationForSecondaryEpao";

        /// <summary>
        /// Requires tokens: { standardreference, standard, standardversioninfo}
        /// </summary>
        public const string EPAOStandardAdd = "EPAOStandardAdd";
    }
}
