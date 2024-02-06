namespace SFA.DAS.AssessorService.Api.Common.Settings
{
    public interface IAzureActiveDirectoryClientConfiguration : IClientConfiguration
    {
        string ClientId { get; set; }

        string ClientSecret { get; set; }

        string ResourceId { get; set; }

        string TenantId { get; set; }

        string ApiBaseAddress { get; set; }
    }
}