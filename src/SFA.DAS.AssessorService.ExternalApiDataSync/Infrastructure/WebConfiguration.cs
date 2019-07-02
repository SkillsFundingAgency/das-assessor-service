using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.ExternalApiDataSync.Infrastructure
{
    public class WebConfiguration : IWebConfiguration
    {
        [JsonRequired] public Settings.ExternalApiDataSync ExternalApiDataSync { get; set; }
    }
}