using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Settings
{
    public class ExternalApiDataSync : IExternalApiDataSync
    {
        [JsonRequired] public bool IsEnabled { get; set; }
        [JsonRequired] public string SourceSqlConnectionString { get; set; }
    }
}
