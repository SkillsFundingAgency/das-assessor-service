namespace SFA.DAS.AssessorService.ExternalApis.AssessmentOrgs.Types
{
    public class Provider
    {
        /// <summary>
        /// UK provider reference number which is not unique
        /// </summary>
        public long Ukprn { get; set; }

        public string ProviderName { get; set; }
    }
}