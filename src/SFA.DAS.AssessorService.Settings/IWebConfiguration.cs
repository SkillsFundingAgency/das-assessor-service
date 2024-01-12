namespace SFA.DAS.AssessorService.Settings
{
    public interface IWebConfiguration
    {
        AzureActiveDirectoryClientConfiguration AssessorApiAuthentication { get; set; }
        AzureApiAuthentication AzureApiAuthentication { get; set; }
        ManagedIdentityClientConfiguration QnaApiAuthentication { get; set; }
        
        LoginServiceConfig LoginService { get; set; }

        AzureActiveDirectoryClientConfiguration RoatpApiAuthentication { get; set; }

        string FeedbackUrl { get; set; }
        string ReferenceFormat { get; set; }
        string ServiceLink { get; set; }
        string SessionRedisConnectionString { get; set; }

        string ZenDeskSnippetKey { get; set; }
        string ZenDeskSectionId { get; set; }
        string ZenDeskCobrowsingSnippetKey { get; set; }
        
        /// <summary>
        /// Property to hold the value of GovSignIn enabled/disabled.
        /// </summary>
        bool UseGovSignIn { get; set; }
    }
}