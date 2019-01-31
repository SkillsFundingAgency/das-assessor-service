using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SFA.DAS.AssessorService.Domain.Extensions;
using SFA.DAS.AssessorService.EpaoDataSync.Data.Types;
using SFA.DAS.AssessorService.EpaoDataSync.Infrastructure;
using SFA.DAS.AssessorService.EpaoDataSync.Logger;

namespace SFA.DAS.AssessorService.EpaoDataSync.Data
{
    public class ProviderEventsServiceApi: IProviderEventServiceApi
    {
        private readonly HttpClient _httpLearnerClient;
        private readonly HttpClient  _httpSubmissionClient;
        private readonly IAggregateLogger _aggregateLogger;
      

        public ProviderEventsServiceApi(IAggregateLogger aggregateLogger)
        {
            _aggregateLogger = aggregateLogger;

            var configuration = ConfigurationHelper.GetConfiguration();
            if (configuration.ProviderEventsClientConfiguration != null && configuration.ProviderEventsSubmissionClientConfig != null)
            {
                Initialise(out _httpLearnerClient, configuration.ProviderEventsClientConfiguration.ApiBaseUrl, configuration.ProviderEventsClientConfiguration.ClientToken, configuration.ProviderEventsClientConfiguration.ApiVersion);
                Initialise(out _httpSubmissionClient, configuration.ProviderEventsSubmissionClientConfig.ApiBaseUrl,
                    configuration.ProviderEventsSubmissionClientConfig.ClientToken, null);
            }
            else
            {
                _aggregateLogger.LogInfo($"Failed to read configurations.");
            }
        }

        public async Task<List<SubmissionEvent>> GetLatestLearnerEventForStandards(long uln, long sinceEventId = 0)
        {
           
            var url = $"/api/learners?uln={uln}";
            if (sinceEventId > 0)
            {
                url += $"&sinceEventId={sinceEventId}";
            }

            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                request.Headers.Add("Accept", "application/json");
                
                using (var response = _httpLearnerClient.SendAsync(request))
                {
                  
                    var result = await response;
                    switch (result.StatusCode)
                    { 
                        case HttpStatusCode.OK:
                        {
                            var json = await result.Content.ReadAsStringAsync();
                            _aggregateLogger.LogInfo($"Status code returned: {result.StatusCode} for Endpoint {url}");
                            return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<List<SubmissionEvent>>(json, _jsonSettings));
                        }
                        case HttpStatusCode.NotFound when result.IsSuccessStatusCode:
                        case HttpStatusCode.NotFound:
                            _aggregateLogger.LogInfo($"Status code returned: {result.StatusCode} for Endpoint {url}");
                            break;
                    }
                }

                return null;
            }
        }

        public async Task<SubmissionEvents> GetSubmissionsEventsByTime(string sinceTime, long pageNumber)
        {

            var url = $"/api/submissions?sinceTime={sinceTime}&pageNumber={pageNumber}";
           
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                request.Headers.Add("Accept", "application/json");

                using (var response = _httpSubmissionClient.SendAsync(request))
                {

                    var result = await response;
                    switch (result.StatusCode)
                    {
                        case HttpStatusCode.OK:
                        {
                            var json = await result.Content.ReadAsStringAsync();
                            _aggregateLogger.LogInfo($"Status code returned: {result.StatusCode} for Endpoint {url}");
                            return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<SubmissionEvents>(json, _jsonSettings));
                        }
                        case HttpStatusCode.NotFound when result.IsSuccessStatusCode:
                        case HttpStatusCode.NotFound:
                            _aggregateLogger.LogInfo($"Status code returned: {result.StatusCode} for Endpoint {url}");
                            break;
                    }
                }

                return null;
            }
        }

        public async Task<SubmissionEvents> GetSubmissionsEventsByEventId(long sinceEventId, long pageNumber)
        {
            var url = $"/api/submissions?sinceEventId={sinceEventId}&pageNumber={pageNumber}";
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            {
                request.Headers.Add("Accept", "application/json");

                using (var response = _httpSubmissionClient.SendAsync(request))
                {
                    var result = await response;
                    switch (result.StatusCode)
                    {
                        case HttpStatusCode.OK:
                        {
                            var json = await result.Content.ReadAsStringAsync();
                            _aggregateLogger.LogInfo(
                                $"Status code returned: {result.StatusCode} for Endpoint {url}");
                            return await Task.Factory.StartNew(() =>
                                JsonConvert.DeserializeObject<SubmissionEvents>(json, _jsonSettings));
                        }
                        case HttpStatusCode.NotFound when result.IsSuccessStatusCode:
                        case HttpStatusCode.NotFound:
                            _aggregateLogger.LogInfo(
                                $"Status code returned: {result.StatusCode} for Endpoint {url}");
                            break;
                    }
                }

                return null;
            }

        }


        private static void Initialise(out HttpClient httpClient, string baseAddress, string clientToken, string apiVersion)
        {
            httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };

            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            if (!string.IsNullOrEmpty(apiVersion))
            {
                httpClient.DefaultRequestHeaders.Add("api-version", apiVersion);
            }
            if (!string.IsNullOrEmpty(clientToken))
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", clientToken);
            }
        }

        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

    }
}
