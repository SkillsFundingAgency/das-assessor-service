namespace SFA.DAS.AssessorService.Settings
{
    public interface IApiConfiguration
    {
        string Environment { get; set; }

        ApiAuthentication ApiAuthentication { get; set; }
        ApiAuthentication SandboxApiAuthentication { get; set; }
        
        NotificationsApiClientConfiguration NotificationsApiClientConfiguration { get; set; }

        ManagedIdentityClientConfiguration QnaApiAuthentication { get; set; }

        LoginServiceConfig LoginService { get; set; }
        
        AzureActiveDirectoryClientConfiguration RoatpApiAuthentication { get; set; }
        AzureActiveDirectoryClientConfiguration ReferenceDataApiAuthentication { get; set; }
        CompaniesHouseApiAuthentication CompaniesHouseApiAuthentication { get; set; }
        CharityCommissionApiAuthentication CharityCommissionApiAuthentication { get; set; }

        OuterApiConfiguration OuterApi { get; set; }

        string SqlConnectionString { get; set; }
        string SandboxSqlConnectionString { get; set; }

        int PipelineCutoff { get; set; }
        string ServiceLink { get; set; }
        EmailTemplatesConfig EmailTemplatesConfig { get; set; }
        bool UseGovSignIn { get; set; }
    }
}