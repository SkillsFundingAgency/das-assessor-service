using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Settings
{
    public class ClientApiAuthentication : IClientApiAuthentication
    {
        [JsonRequired] public string ClientId { get; set; }

        [JsonRequired] public string ClientSecret { get; set; }

        [JsonRequired] public string ResourceId { get; set; }

        [JsonRequired] public string TenantId { get; set; }

        public string ApiBaseUrl { get; set; }

        public string Instance { get; set; }

        public string IdentifierUri { get; set; }
        public string ApiBaseAddress { get; set; }
    }
}