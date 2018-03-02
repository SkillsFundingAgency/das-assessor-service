using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.EpaoImporter.Settings
{
    public class WebConfiguration : IWebConfiguration
    {
        [JsonRequired]
        public AuthSettings Authentication { get; set; }
        [JsonRequired]
        public ApiAuthentication ApiAuthentication { get; set; }
        [JsonRequired]
        public ClientApiAuthentication ClientApiAuthentication { get; set; }
        [JsonRequired]
        public string SqlConnectionString { get; set; }
    }
}