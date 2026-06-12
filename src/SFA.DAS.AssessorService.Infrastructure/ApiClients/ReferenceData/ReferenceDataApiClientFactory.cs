using SFA.DAS.Http;
using System.Net.Http;

namespace SFA.DAS.AssessorService.Infrastructure.ApiClients.ReferenceData
{
    public class ReferenceDataApiClientFactory : IReferenceDataApiClientFactory
    {
        private readonly ReferenceDataApiClientConfiguration _referenceDataApiClientConfiguration;

        public ReferenceDataApiClientFactory(ReferenceDataApiClientConfiguration referenceDataApiClientConfiguration)
        {
            _referenceDataApiClientConfiguration = referenceDataApiClientConfiguration;
        }

        public HttpClient CreateHttpClient()
        {
            var httpClient = new AzureActiveDirectoryHttpClientFactory(_referenceDataApiClientConfiguration).CreateHttpClient();
            return httpClient;
        }
    }
}
