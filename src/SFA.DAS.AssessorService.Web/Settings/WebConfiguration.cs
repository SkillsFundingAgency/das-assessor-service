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
        public string ApiResourceId { get; set; }
        [JsonRequired]
        public string ApiBaseAddress { get; set; }
    }

    public interface IApiSettings
    {
        string ApiResourceId { get; set; }
        string ApiBaseAddress { get; set; }
    }

    public class AuthSettings : IAuthSettings
    {
        [JsonRequired]
        public string Instance { get; set; }
        [JsonRequired]
        public string Domain { get; set; }
        [JsonRequired]
        public string TenantId { get; set; }
        [JsonRequired]
        public string ClientId { get; set; }
        [JsonRequired]
        public string CallbackPath { get; set; }
        [JsonRequired]
        public string ClientSecret { get; set; }
        [JsonRequired]
        public string TokenEncodingKey { get; set; }
    }

    public interface IAuthSettings
    {
        string Instance { get; set; }
        string Domain { get; set; }
        string TenantId { get; set; }
        string ClientId { get; set; }
        string CallbackPath { get; set; }
        string ClientSecret { get; set; }
        string TokenEncodingKey { get; set; }

    }
    

}