using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Api.Common.Settings;

namespace SFA.DAS.AssessorService.Settings
{
    public interface IWebConfiguration
    {
        AzureActiveDirectoryClientConfiguration AssessorApiAuthentication { get; set; }
        AzureApiClientConfiguration AzureApiAuthentication { get; set; }
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

    }
}