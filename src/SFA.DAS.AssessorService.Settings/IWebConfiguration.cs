using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Application.Api.Client.Configuration;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.QnA;

namespace SFA.DAS.AssessorService.Settings
{
    public interface IWebConfiguration
    {
        AssessorApiClientConfiguration AssessorApiAuthentication { get; set; }
        AzureApiClientConfiguration AzureApiAuthentication { get; set; }
        QnaApiClientConfiguration QnaApiAuthentication { get; set; }

        string FeedbackUrl { get; set; }
        string ReferenceFormat { get; set; }
        string ServiceLink { get; set; }
        string SessionRedisConnectionString { get; set; }

        string ZenDeskSnippetKey { get; set; }
        string ZenDeskSectionId { get; set; }
        string ZenDeskCobrowsingSnippetKey { get; set; }

    }
}