namespace SFA.DAS.AssessorService.Domain.Consts
{
    /// <summary>
    /// This is intended to be different from the ApplicationTypes constant. This is not to be used
    /// as part of the ApplicationTypeResolver, It was originally designed to indicate whether an 
    /// application in the assessors database was a short or full based on it being a version 
    /// application or not.
    /// </summary>
    public static class StandardApplicationTypes
    {
        /// <summary>
        /// An apply for an standard, currently the only type of apply that can be started, all 
        /// questions will be asked
        /// </summary>
        public const string Full = "full";

        /// <summary>
        /// An apply for a single standard version when the assessment plan had changed from the 
        /// last standard versions, where a subset of questions will be asked. No longer required as 
        /// all standard versions can be opted-in, included as historical records will exist 
        /// for these types of apply
        /// </summary>
        public const string Version = "version";

        /// <summary>
        /// An apply for withdrawal of all versions of a standard, currently the 
        /// only type of withdrawal
        /// </summary>
        public const string StandardWithdrawal = "standardWithdrawal";

        /// <summary>
        /// An apply for withdrawal of some versions of a standard. No longer required as 
        /// stanard versions can be opted-out, included as historical records will exist 
        /// for these types of withdrawal
        /// </summary>
        public const string VersionWithdrawal = "versionWithdrawal";
    }
}
