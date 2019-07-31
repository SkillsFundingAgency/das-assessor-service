using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.ExternalApiDataSync.Infrastructure
{
    public class WebConfiguration : IWebConfiguration
    {
        [JsonRequired] public string SqlConnectionString { get; set; }
        [JsonRequired] public string SandboxSqlConnectionString { get; set; }
        [JsonRequired] public Settings.ExternalApiDataSync ExternalApiDataSync { get; set; }
    }
}