using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Settings
{
    public class WebConfiguration
        : IWebConfiguration
    {
        [JsonRequired] public ClientApiAuthentication AssessorApiAuthentication { get; set; }
        [JsonRequired] public AzureApiAuthentication AzureApiAuthentication { get; set; }
        [JsonRequired] public ClientApiAuthentication QnaApiAuthentication { get; set; }

        [JsonRequired] public LoginServiceConfig LoginService { get; set; }

        [JsonRequired] public ClientApiAuthentication RoatpApiAuthentication { get; set; }

        [JsonRequired] public string FeedbackUrl { get; set; }
        [JsonRequired] public string ReferenceFormat { get; set; }
        [JsonRequired] public string ServiceLink { get; set; }
        [JsonRequired] public string SessionRedisConnectionString { get; set; }

        [JsonRequired] public string ZenDeskSnippetKey { get; set; }
        [JsonRequired] public string ZenDeskSectionId { get; set; }
        [JsonRequired] public string ZenDeskCobrowsingSnippetKey { get; set; }

        /// <inheritdoc />
        [JsonRequired] public bool UseGovSignIn { get; set; }
    }
}