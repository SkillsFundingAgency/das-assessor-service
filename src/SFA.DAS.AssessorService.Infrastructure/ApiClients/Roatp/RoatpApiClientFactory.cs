using SFA.DAS.AssessorService.Settings;
using SFA.DAS.Http;
using SFA.DAS.Http.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
            httpClient.DefaultRequestHeaders.Add("Accept", "Application/json");

            return httpClient;
        }
    }
}
