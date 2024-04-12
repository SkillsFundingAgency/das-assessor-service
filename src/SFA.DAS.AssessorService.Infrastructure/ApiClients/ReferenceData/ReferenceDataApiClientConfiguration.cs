using SFA.DAS.Http.Configuration;

namespace SFA.DAS.AssessorService.Infrastructure.ApiClients.ReferenceData
{
    public class ReferenceDataApiClientConfiguration : IAzureActiveDirectoryClientConfiguration
    {
        public string IdentifierUri { get; set; }
        public string ApiBaseUrl { get; set; }
        public string Tenant { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
