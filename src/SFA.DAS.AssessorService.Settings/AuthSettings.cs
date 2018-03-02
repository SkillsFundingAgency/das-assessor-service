using Newtonsoft.Json;
using SFA.DAS.AssessorService.EpaoImporter.Settings;

namespace SFA.DAS.AssessorService.Settings
{
    public class AuthSettings : IAuthSettings
    {
        [JsonRequired]
        public string WtRealm { get; set; }
        [JsonRequired]
        public string MetadataAddress { get; set; }
        [JsonRequired]
        public string Role { get; set; }
    }
}