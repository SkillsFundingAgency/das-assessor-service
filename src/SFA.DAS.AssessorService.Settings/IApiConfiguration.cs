namespace SFA.DAS.AssessorService.Settings
{
    public interface IApiConfiguration
    {
        string Environment { get; set; }

        ApiAuthentication ApiAuthentication { get; set; }
        ApiAuthentication SandboxApiAuthentication { get; set; }
        
        NotificationsApiClientConfiguration NotificationsApiClientConfiguration { get; set; }

        ClientApiAuthentication QnaApiAuthentication { get; set; }

        LoginServiceConfig LoginService { get; set; }
        
        ClientApiAuthentication RoatpApiAuthentication { get; set; }
        ClientApiAuthentication ReferenceDataApiAuthentication { get; set; }
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