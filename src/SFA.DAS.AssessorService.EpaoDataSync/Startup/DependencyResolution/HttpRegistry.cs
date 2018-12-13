using System;
using System.Net.Http;
using System.Net.Http.Headers;
using SFA.DAS.AssessorService.EpaoDataSync.Infrastructure;
using StructureMap;

namespace SFA.DAS.AssessorService.EpaoDataSync.Startup.DependencyResolution
{
    public class HttpRegistry : Registry
    {
        public HttpRegistry()
        {
            var configuration = ConfigurationHelper.GetConfiguration();
            
            var baseAddress = configuration.ProviderEventsClientBaseUrl;

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };

            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            For<HttpClient>().Use<HttpClient>(httpClient).Singleton();
        }
    }
}