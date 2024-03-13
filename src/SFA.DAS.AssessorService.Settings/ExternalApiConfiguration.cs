using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Api.Client.Configuration;

namespace SFA.DAS.AssessorService.Settings
{
    public class ExternalApiConfiguration
        : IExternalApiConfiguration

    {
        [JsonRequired] public AssessorApiClientConfiguration AssessorApiAuthentication { get; set; }

        [JsonRequired] public AssessorApiClientConfiguration SandboxAssessorApiAuthentication { get; set; }
    }
}