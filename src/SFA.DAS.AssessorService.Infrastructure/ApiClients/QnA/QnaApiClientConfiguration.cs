using SFA.DAS.Http.Configuration;

namespace SFA.DAS.AssessorService.Infrastructure.ApiClients.QnA
{
    public class QnaApiClientConfiguration : IManagedIdentityClientConfiguration
    {
        public string IdentifierUri { get; set; }
        public string ApiBaseUrl { get; set; }
    }
}
