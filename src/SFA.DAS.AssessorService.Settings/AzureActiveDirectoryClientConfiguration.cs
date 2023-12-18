using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Settings
{
    public class AzureActiveDirectoryClientConfiguration : IAzureActiveDirectoryClientConfiguration
    {
        [JsonRequired] public string ClientId { get; set; }

        [JsonRequired] public string ClientSecret { get; set; }

        [JsonRequired] public string ResourceId { get; set; }

        [JsonRequired] public string TenantId { get; set; }

        public string ApiBaseAddress { get; set; }
    }
}