using SFA.DAS.Http.Configuration;

namespace SFA.DAS.AssessorService.Infrastructure.ApiClients.Roatp
{
    public class RoatpApiClientConfiguration : IManagedIdentityClientConfiguration
    {
        public string IdentifierUri { get; set; }
        public string ApiBaseUrl { get; set; }
    }
}
