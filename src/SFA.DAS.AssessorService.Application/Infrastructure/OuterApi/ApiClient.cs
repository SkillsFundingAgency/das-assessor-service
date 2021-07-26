using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.Infrastructure.OuterApi
{
    public class OuterApiClient : IOuterApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly OuterApiConfiguration _config;
        const string SubscriptionKeyRequestHeaderKey = "Ocp-Apim-Subscription-Key";
        const string VersionRequestHeaderKey = "X-Version";

        public OuterApiClient (HttpClient httpClient, IWebConfiguration config)
        {
            _httpClient = httpClient;
            _config = config.OuterApi;
            _httpClient.BaseAddress = new Uri(_config.BaseUrl);
        }
        
        public async Task<TResponse> Get<TResponse>(IGetApiRequest request) 
        {
            AddHeaders();

            var response = await _httpClient.GetAsync(request.GetUrl).ConfigureAwait(false);

            if (response.StatusCode.Equals(HttpStatusCode.NotFound))
            {
                return default;
            }

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<TResponse>(json);
            }

            response.EnsureSuccessStatusCode();
            return default;
        }

        private void AddHeaders()
        {
            //The http handler life time is set to 5 minutes
            //hence once the headers are added they don't need added again
            if (_httpClient.DefaultRequestHeaders.Contains(SubscriptionKeyRequestHeaderKey)) return;

            _httpClient.DefaultRequestHeaders.Add(SubscriptionKeyRequestHeaderKey, _config.Key);
            _httpClient.DefaultRequestHeaders.Add(VersionRequestHeaderKey, "1");
        }
    }

    public interface IOuterApiClient
    {
        Task<TResponse> Get<TResponse>(IGetApiRequest request);
    }
}