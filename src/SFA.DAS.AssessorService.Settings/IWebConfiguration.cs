namespace SFA.DAS.AssessorService.Settings
{
    public interface IWebConfiguration
    {
        AzureApiAuthentication AzureApiAuthentication { get; set; }
        ClientApiAuthentication AssessorApiAuthentication { get; set; }
        string SessionRedisConnectionString { get; set; }
        ClientApiAuthentication QnaApiAuthentication { get; set; }
        string ServiceLink { get; set; }
        LoginServiceConfig LoginService { get; set; }

        ClientApiAuthentication RoatpApiAuthentication { get; set; }

        string ReferenceFormat { get; set; }
        string FeedbackUrl { get; set; }

        string ZenDeskSnippetKey { get; set; }
        string ZenDeskSectionId { get; set; }
        string ZenDeskCobrowsingSnippetKey { get; set; }
    }
}