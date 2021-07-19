namespace SFA.DAS.AssessorService.Domain.Consts
{
    public static class StandardApplicationTypes
    {
        /// <summary>
        /// This is intended to be different from the ApplicationTypes constant. This is not to be used
        /// as part of the ApplicationTypeResolver, it's to indicate whether an application in the 
        /// assessors database is short or full based on it being a version application.
        /// 
        /// It's been put here to differentiate it from the ApplicationTypeResolver logic
        /// </summary>
        public const string Version = "version";
        public const string Full = "full";

        public const string StandardWithdrawal = "standardWithdrawal";
        public const string VersionWithdrawal = "versionWithdrawal";
    }
}
