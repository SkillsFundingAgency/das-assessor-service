using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Settings
{
    public class ApiAuthentication : IApiAuthentication
    {
        [JsonRequired] public string TenantId { get; set; }

        [JsonRequired] public string Audience { get; set; }
    }
}