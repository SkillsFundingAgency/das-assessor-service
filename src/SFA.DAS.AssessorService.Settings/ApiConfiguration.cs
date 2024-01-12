using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Settings
{
    public class ApiConfiguration
        : IApiConfiguration
    {
        [JsonIgnore] public string Environment { get; set; }

        [JsonRequired] public ApiAuthentication ApiAuthentication { get; set; }
        [JsonRequired] public ApiAuthentication SandboxApiAuthentication { get; set; }

        [JsonRequired] public NotificationsApiClientConfiguration NotificationsApiClientConfiguration { get; set; }

        [JsonRequired] public AzureActiveDirectoryClientConfiguration QnaApiAuthentication { get; set; }

        [JsonRequired] public LoginServiceConfig LoginService { get; set; }

        [JsonRequired] public AzureActiveDirectoryClientConfiguration RoatpApiAuthentication { get; set; }
        [JsonRequired] public AzureActiveDirectoryClientConfiguration ReferenceDataApiAuthentication { get; set; }
        [JsonRequired] public CompaniesHouseApiAuthentication CompaniesHouseApiAuthentication { get; set; }
        [JsonRequired] public CharityCommissionApiAuthentication CharityCommissionApiAuthentication { get; set; }

        [JsonRequired] public OuterApiConfiguration OuterApi { get; set; }

        [JsonRequired] public string SqlConnectionString { get; set; }
        [JsonRequired] public string SandboxSqlConnectionString { get; set; }

        [JsonRequired] public int PipelineCutoff { get; set; }
        [JsonRequired] public string ServiceLink { get; set; }
        [JsonRequired] public EmailTemplatesConfig EmailTemplatesConfig { get; set; }
        public bool UseGovSignIn { get; set; }
    }
}