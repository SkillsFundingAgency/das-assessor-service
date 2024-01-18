using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Api.Common.Settings;

namespace SFA.DAS.AssessorService.Settings
{
    public interface IApiConfiguration
    {
        string Environment { get; set; }

        ApiAuthentication ApiAuthentication { get; set; }
        ApiAuthentication SandboxApiAuthentication { get; set; }
        
        NotificationsApiClientConfiguration NotificationsApiClientConfiguration { get; set; }

        AzureActiveDirectoryClientConfiguration QnaApiAuthentication { get; set; }

        LoginServiceConfig LoginService { get; set; }
        
        ManagedIdentityClientConfiguration RoatpApiAuthentication { get; set; }
        AzureActiveDirectoryClientConfiguration ReferenceDataApiAuthentication { get; set; }
        CompaniesHouseApiClientConfiguration CompaniesHouseApiAuthentication { get; set; }
        CharityCommissionApiClientConfiguration CharityCommissionApiAuthentication { get; set; }

        OuterApiClientConfiguration OuterApi { get; set; }

        string SqlConnectionString { get; set; }
        string SandboxSqlConnectionString { get; set; }

        int PipelineCutoff { get; set; }
        string ServiceLink { get; set; }
        EmailTemplatesConfig EmailTemplatesConfig { get; set; }
        bool UseGovSignIn { get; set; }
    }
}