using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Domain.Entities;
using SearchQuery = SFA.DAS.AssessorService.Application.Api.External.Models.Search.SearchQuery;
using SearchResult = SFA.DAS.AssessorService.Application.Api.External.Models.Search.SearchResult;

namespace SFA.DAS.AssessorService.Application.Api.External.Infrastructure
{
    public class ApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<ApiClient> _logger;
        private readonly ITokenService _tokenService;

        private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        public ApiClient(HttpClient client, ILogger<ApiClient> logger, ITokenService tokenService)
        {
            _client = client;
            _logger = logger;
            _tokenService = tokenService;
        }

        protected async Task<T> Get<T>(string uri)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
            var res = await _client.GetAsync(new Uri(uri, UriKind.Relative));
            return await res.Content.ReadAsAsync<T>();
        }

        protected async Task<U> PostPutRequestWithResponse<T, U>(HttpRequestMessage requestMessage, T model)
        {
            var serializeObject = JsonConvert.SerializeObject(model);
            requestMessage.Content = new StringContent(serializeObject,
                System.Text.Encoding.UTF8, "application/json");

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

            using (var response = await _client.SendAsync(requestMessage))
            {
                var json = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == HttpStatusCode.OK
                    || response.StatusCode == HttpStatusCode.Created
                    || response.StatusCode == HttpStatusCode.NoContent)
                {
                    return await Task.Factory.StartNew<U>(() => JsonConvert.DeserializeObject<U>(json, _jsonSettings));
                }
                else
                {
                    _logger.LogInformation($"HttpRequestException: Status Code: {response.StatusCode} Body: {json}");
                    throw new HttpRequestException(json);
                }
            }
        }

        public virtual async Task<List<SearchResult>> Search(SearchQuery searchQuery, int? stdCodeFilter = null)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/search"))
            {
                List<SearchResult> results = await PostPutRequestWithResponse<SearchQuery, List<SearchResult>>(request, searchQuery);

                return results.Where(s => stdCodeFilter is null || s.StdCode == stdCodeFilter).ToList();
            }
        }

        public async Task<Certificate> StartCertificate(StartCertificateRequest request)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/v1/certificates/start"))
            {
                return await PostPutRequestWithResponse<StartCertificateRequest, Certificate>(httpRequest, request);
            }
        }

        public async Task<Certificate>GetCertificateForUln(GetCertificateForUlnRequest request)
        {
            return await Get<Certificate> ($"/api/v1/certificates/{request.Uln}/{request.StandardCode}");
        }

        public async Task<Certificate> UpdateCertificate(UpdateCertificateRequest certificateRequest)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Put, "api/v1/certificates/update"))
            {
                return await PostPutRequestWithResponse<UpdateCertificateRequest, Certificate>(httpRequest, certificateRequest);
            }
        }
    }
}
