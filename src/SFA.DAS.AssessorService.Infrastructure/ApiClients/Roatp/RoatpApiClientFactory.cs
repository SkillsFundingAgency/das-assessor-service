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
        private readonly IManagedIdentityClientConfiguration _managedIdentityClientConfiguration;

        public RoatpApiClientFactory(IManagedIdentityClientConfiguration managedIdentityClientConfiguration) 
        {
            _managedIdentityClientConfiguration = managedIdentityClientConfiguration;
        }  
        public HttpClient CreateHttpClient()
        {
            var httpClient = new ManagedIdentityHttpClientFactory(_managedIdentityClientConfiguration).CreateHttpClient();
            httpClient.DefaultRequestHeaders.Add("Accept", "Application/json");

            return httpClient;
        }
    }
}
