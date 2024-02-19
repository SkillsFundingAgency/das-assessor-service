using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Api.Common.Settings;

namespace SFA.DAS.AssessorService.Settings
{
    public class WebConfiguration : IWebConfiguration
    {
        [JsonRequired] public AzureActiveDirectoryClientConfiguration AssessorApiAuthentication { get; set; }
        [JsonRequired] public AzureApiClientConfiguration AzureApiAuthentication { get; set; }
        [JsonRequired] public ManagedIdentityClientConfiguration QnaApiAuthentication { get; set; }

        [JsonRequired] public LoginServiceConfig LoginService { get; set; }

        [JsonRequired] public AzureActiveDirectoryClientConfiguration RoatpApiAuthentication { get; set; }

        [JsonRequired] public string FeedbackUrl { get; set; }
        [JsonRequired] public string ReferenceFormat { get; set; }
        [JsonRequired] public string ServiceLink { get; set; }
        [JsonRequired] public string SessionRedisConnectionString { get; set; }

        [JsonRequired] public string ZenDeskSnippetKey { get; set; }
        [JsonRequired] public string ZenDeskSectionId { get; set; }
        [JsonRequired] public string ZenDeskCobrowsingSnippetKey { get; set; }

    }
}