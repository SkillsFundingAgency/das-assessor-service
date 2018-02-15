using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Settings
{
    public class ApiSettings : IApiSettings
    {
        [JsonRequired]
        public string ApiBaseAddress { get; set; }
        [JsonRequired]
        public string TokenEncodingKey { get; set; }
    }
}