using SFA.DAS.Http.Configuration;

namespace SFA.DAS.AssessorService.Application.Api.Client.Configuration
{
    public class AssessorApiClientConfiguration : IManagedIdentityClientConfiguration
    {
        public string IdentifierUri { get; set; }
        public string ApiBaseUrl { get; set; }
    }
}
