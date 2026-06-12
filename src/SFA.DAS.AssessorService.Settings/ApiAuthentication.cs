using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Settings
{
    public class ApiAuthentication : IApiAuthentication
    {
        [JsonRequired] public string Tenant { get; set; }

        [JsonRequired] public string Audiences { get; set; }
    }
}