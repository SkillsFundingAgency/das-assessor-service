using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Settings
{
    public class OuterApiConfiguration
    {
        [JsonRequired] public string BaseUrl { get; set; }
        [JsonRequired] public string Key { get; set; }
    }
}