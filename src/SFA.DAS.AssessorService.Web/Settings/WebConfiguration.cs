using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Web.Settings
{
    public class WebConfiguration : IWebConfiguration
    {
        [JsonRequired]
        public AuthSettings Authentication { get; set; }
        [JsonRequired]
        public ApiSettings Api { get; set; }
    }

    public interface IWebConfiguration
    {
        AuthSettings Authentication { get; set; }
        ApiSettings Api { get; set; }
    }

    public class ApiSettings : IApiSettings
    {
        [JsonRequired]
        public string ApiBaseAddress { get; set; }
        [JsonRequired]
        public string TokenEncodingKey { get; set; }
    }

    public interface IApiSettings
    {
        string TokenEncodingKey { get; set; }
        string ApiBaseAddress { get; set; }
    }

    public class AuthSettings : IAuthSettings
    {
        [JsonRequired]
        public string WtRealm { get; set; }
        [JsonRequired]
        public string MetadataAddress { get; set; }
    }

    public interface IAuthSettings
    {
        string WtRealm { get; set; }
        string MetadataAddress { get; set; }
    }
}