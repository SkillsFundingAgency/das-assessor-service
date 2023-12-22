namespace SFA.DAS.AssessorService.Api.Common.Settings
{
    public interface IManagedIdentityClientConfiguration : IClientConfiguration
    {
        string IdentifierUri { get; set; }

        string ApiBaseUrl { get; set; }
    }
}
