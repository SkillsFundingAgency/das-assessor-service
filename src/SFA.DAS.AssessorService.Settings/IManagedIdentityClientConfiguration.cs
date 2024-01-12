namespace SFA.DAS.AssessorService.Settings
{
    public interface IManagedIdentityClientConfiguration : IClientConfiguration
    {
        string IdentifierUri { get; set; }
        string ApiBaseUrl { get; set; }
    }
}
