using SFA.DAS.AssessorService.Settings;
using SFA.DAS.Http;
using System.Net.Http;

namespace SFA.DAS.AssessorService.Infrastructure.ApiClients.Roatp
{
    public class RoatpApiClientFactory : IRoatpApiClientFactory
    {
        private readonly RoatpApiClientConfiguration _roatpApiClientConfiguration;

        public RoatpApiClientFactory(RoatpApiClientConfiguration roatpApiClientConfiguration) 
        {
            _roatpApiClientConfiguration = roatpApiClientConfiguration;
        }

        public HttpClient CreateHttpClient()
        {
            var httpClient = new ManagedIdentityHttpClientFactory(_roatpApiClientConfiguration).CreateHttpClient();
            return httpClient;
        }
    }
}
