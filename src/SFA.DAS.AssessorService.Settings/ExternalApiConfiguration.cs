using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Settings
{
    public class ExternalApiConfiguration
        : IExternalApiConfiguration

    {
        [JsonRequired] public ClientApiAuthentication AssessorApiAuthentication { get; set; }

        [JsonRequired] public ClientApiAuthentication SandboxAssessorApiAuthentication { get; set; }
    }
}