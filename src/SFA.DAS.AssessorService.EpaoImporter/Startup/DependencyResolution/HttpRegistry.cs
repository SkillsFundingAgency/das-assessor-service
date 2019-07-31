using System;
using System.Net.Http;
using System.Net.Http.Headers;
using SFA.DAS.AssessorService.EpaoImporter.InfrastructureServices;
using StructureMap;

namespace SFA.DAS.AssessorService.EpaoImporter.Startup.DependencyResolution
{
    public class HttpRegistry : Registry
    {
        public HttpRegistry()
        {
            var configuration = ConfigurationHelper.GetConfiguration();

            var tokenService = new TokenService(configuration);
            var token = tokenService.GetToken();

            var baseAddress = configuration.ClientApiAuthentication.ApiBaseAddress;

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };

            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            For<HttpClient>().Use<HttpClient>(httpClient).Singleton();
        }
    }
}