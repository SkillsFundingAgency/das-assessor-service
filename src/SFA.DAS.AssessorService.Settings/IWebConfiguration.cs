namespace SFA.DAS.AssessorService.Settings
{
    public interface IWebConfiguration
    {
        ApiAuthentication ApiAuthentication { get; set; }
        AzureApiAuthentication AzureApiAuthentication { get; set; }
        ClientApiAuthentication AssessorApiAuthentication { get; set; }
        NotificationsApiClientConfiguration NotificationsApiClientConfiguration { get; set; }
        string AssessmentOrgsApiClientBaseUrl { get; set; }
        string IfaApiClientBaseUrl { get; set; }
        string IFATemplateStorageConnectionString { get; set; } 
        string SqlConnectionString { get; set; }
        string SessionRedisConnectionString { get; set; }
        ClientApiAuthentication QnaApiAuthentication { get; set; }
        string ServiceLink { get; set; }
        LoginServiceConfig LoginService { get; set; }
        
        ClientApiAuthentication RoatpApiAuthentication { get; set; }

        ReferenceDataApiAuthentication ReferenceDataApiAuthentication { get; set; }

        CompaniesHouseApiAuthentication CompaniesHouseApiAuthentication { get; set; }
        CharityCommissionApiAuthentication CharityCommissionApiAuthentication { get; set; }

        string ReferenceFormat { get; set; }
        string FeedbackUrl { get; set; }
        
        #region For External API Sandbox
        string SandboxSqlConnectionString { get; set; }
        ApiAuthentication SandboxApiAuthentication { get; set; }
        ClientApiAuthentication SandboxClientApiAuthentication { get; set; }
        #endregion

        string ZenDeskSnippetKey { get; set; }
        string ZenDeskSectionId { get; set; }
        string ZenDeskCobrowsingSnippetKey { get; set; }
        
        OuterApiConfiguration OuterApi { get; set; }
    }
}