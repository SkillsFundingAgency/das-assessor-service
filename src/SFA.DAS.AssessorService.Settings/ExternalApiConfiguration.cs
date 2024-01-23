using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Settings
{
    public class ExternalApiConfiguration
        : IExternalApiConfiguration

    {
        [JsonRequired] public AzureActiveDirectoryClientConfiguration AssessorApiAuthentication { get; set; }

        [JsonRequired] public AzureActiveDirectoryClientConfiguration SandboxAssessorApiAuthentication { get; set; }
    }
}