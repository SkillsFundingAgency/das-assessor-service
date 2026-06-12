using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Api.Common
{
    public class OuterApiClientConfiguration : IOuterApiClientConfiguration
    {
        [JsonRequired] public string BaseUrl { get; set; }

        [JsonRequired] public string Key { get; set; }
    }
}