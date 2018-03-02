using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.EpaoImporter.Settings
{
    public class AuthSettings : IAuthSettings
    {
        [JsonRequired]
        public string WtRealm { get; set; }
        [JsonRequired]
        public string MetadataAddress { get; set; }
    }
}