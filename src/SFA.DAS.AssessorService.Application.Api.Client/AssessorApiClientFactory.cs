using SFA.DAS.AssessorService.Application.Api.Client.Configuration;
using SFA.DAS.Http;
using System.Net.Http;

namespace SFA.DAS.AssessorService.Application.Api.Client
{
    public class AssessorApiClientFactory : IAssessorApiClientFactory
    {
        private readonly AssessorApiClientConfiguration _assessorApiClientConfiguration;

        public AssessorApiClientFactory(AssessorApiClientConfiguration assessorApiClientConfiguration)
        {
            _assessorApiClientConfiguration = assessorApiClientConfiguration;
        }

        public HttpClient CreateHttpClient()
        {
            var httpClient = new ManagedIdentityHttpClientFactory(_assessorApiClientConfiguration).CreateHttpClient();
            return httpClient;
        }
    }
}
