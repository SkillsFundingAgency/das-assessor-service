namespace SFA.DAS.AssessorService.Api.Types.Consts
{
    public class EmailTemplateNames
    {
        public const string EPAOPrimaryContactAmended = "EPAOPrimaryContactAmended";
        public const string EPAOOrganisationDetailsAmended = "EPAOOrganisationDetailsAmended";
        public const string ApplyEPAOInitialSubmission = "ApplyEPAOInitialSubmission";
        public const string ApplyEPAOStandardSubmission = "ApplyEPAOStandardSubmission";
        public const string ApplyEPAOAlertSubmission = "ApplyEPAOAlertSubmission";
        public const string EPAOWithdrawalSubmission = "EPAOWithdrawalSubmission";
        public const string EPAOWithdrawalFeedback = "EPAOWithdrawalFeedback";
        public const string EPAOCancelApplication = "EPAOCancelApplication";
        public const string EPAOStandardWithdrawalApproval = "EPAOStandardWithdrawalApproval";
        public const string EPAOOrganisationWithdrawalApproval = "EPAOOrganisationWithdrawalApproval";

        /// <summary>
        /// Requires tokens: { contactname }
        /// </summary>
        public const string APPLY_EPAO_UPDATE = "ApplyEPAOUpdate";

        /// <summary>
        /// Requires tokens: { contactname, standard }
        /// </summary>
        public const string APPLY_EPAO_RESPONSE = "ApplyEPAOResponse";

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

        /// <summary>
        /// Requires tokens: { contactname, standard, standardreference, version, servicename }
        /// </summary>
        public const string EPAOStandardConfimOptIn = "EPAOStandardConfimOptIn";

        /// <summary>
        /// Requires tokens: { contactname, standard, standardreference, version, servicename }
        /// </summary>
        public const string EPAOStandardConfimOptOut = "EPAOStandardConfimOptOut";
    }
}
