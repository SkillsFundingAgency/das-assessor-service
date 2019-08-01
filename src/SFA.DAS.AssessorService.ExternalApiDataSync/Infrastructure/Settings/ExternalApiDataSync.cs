using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.ExternalApiDataSync.Infrastructure.Settings
{
    public class ExternalApiDataSync : IExternalApiDataSync
    {
        [JsonRequired] public bool IsEnabled { get; set; }
    }
}