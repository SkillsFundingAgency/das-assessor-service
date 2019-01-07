using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SFA.DAS.AssessorService.EpaoDataSync.Data.Types;
using SFA.DAS.AssessorService.EpaoDataSync.Logger;

namespace SFA.DAS.AssessorService.EpaoDataSync.Data
{
    public class ProviderEventsServiceApi: IProviderEventServiceApi
    {
        private readonly HttpClient _httpClient;
        private readonly IAggregateLogger _aggregateLogger;

        public ProviderEventsServiceApi(HttpClient httpClient, IAggregateLogger aggregateLogger)
        {
            _httpClient = httpClient;
            _aggregateLogger = aggregateLogger;
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
                
                using (var response = _httpClient.SendAsync(request))
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

        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };
    }
}
