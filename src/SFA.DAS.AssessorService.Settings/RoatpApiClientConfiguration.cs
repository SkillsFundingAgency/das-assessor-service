namespace SFA.DAS.AssessorService.Settings
{
    public class RoatpApiClientConfiguration : SFA.DAS.Http.Configuration.IManagedIdentityClientConfiguration
    {
        public string IdentifierUri { get; set; }
        public string ApiBaseUrl { get; set; }
    }
}
