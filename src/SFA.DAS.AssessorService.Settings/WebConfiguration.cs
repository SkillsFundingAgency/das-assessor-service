using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Settings
{
    public class WebConfiguration : IWebConfiguration
    {
        [JsonRequired] public ApiAuthentication ApiAuthentication { get; set; }

        [JsonRequired] public AzureApiAuthentication AzureApiAuthentication { get; set; }

        [JsonRequired] public ClientApiAuthentication AssessorApiAuthentication { get; set; }

        [JsonRequired] public NotificationsApiClientConfiguration NotificationsApiClientConfiguration { get; set; }

        [JsonRequired] public string AssessmentOrgsApiClientBaseUrl { get; set; }

        [JsonRequired] public string IfaApiClientBaseUrl { get; set; }

        [JsonRequired] public string IFATemplateStorageConnectionString { get; set; } 

        [JsonRequired] public string SqlConnectionString { get; set; }

        [JsonRequired] public string SessionRedisConnectionString { get; set; }
        
        [JsonRequired] public ClientApiAuthentication QnaApiAuthentication { get; set; }
        [JsonRequired] public string ServiceLink { get; set; }
        [JsonRequired] public LoginServiceConfig LoginService { get; set; }

        [JsonRequired] public ClientApiAuthentication RoatpApiAuthentication { get; set; }
        [JsonRequired] public string FeedbackUrl { get; set; }
        [JsonRequired] public string ReferenceFormat { get; set; }

        #region For External API Sandbox
        [JsonRequired] public string SandboxSqlConnectionString { get; set; }
        [JsonRequired] public ApiAuthentication SandboxApiAuthentication { get; set; }
        [JsonRequired] public ClientApiAuthentication SandboxClientApiAuthentication { get; set; }
        [JsonRequired] public bool ExternalApiDataSyncEnabled { get; set; }
        #endregion

        [JsonRequired] public ProviderRegisterApiAuthentication ProviderRegisterApiAuthentication { get; set; }
        [JsonRequired] public ReferenceDataApiAuthentication ReferenceDataApiAuthentication { get; set; }

        [JsonRequired] public CompaniesHouseApiAuthentication CompaniesHouseApiAuthentication { get; set; }
        [JsonRequired] public CharityCommissionApiAuthentication CharityCommissionApiAuthentication { get; set; }

        [JsonRequired] public string ZenDeskSnippetKey { get; set; }
        [JsonRequired] public string ZenDeskSectionId { get; set; }
        [JsonRequired] public string ZenDeskCobrowsingSnippetKey { get; set; }
    }
}