using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Settings
{
    public class WebConfiguration : IWebConfiguration
    {
        [JsonRequired]
        public AuthSettings Authentication { get; set; }
        [JsonRequired]
        public ApiSettings Api { get; set; }

        public string SqlConnectionString { get; set; }
    }
}