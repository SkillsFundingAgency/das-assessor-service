using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Web.Staff.Infrastructure
{
    public class ApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<ApiClient> _logger;
        private readonly ITokenService _tokenService;

        protected readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
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

        private async Task<T> Get<T>(string uri)
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
                //var result = await response;
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

        public async Task<List<CertificateResponse>> GetCertificates()
        {
            return await Get<List<CertificateResponse>>("/api/v1/certificates?statusses=Submitted");
        }

        public async Task<PaginatedList<StaffSearchResult>> Search(string searchString, int page)
        {
            return await Get<PaginatedList<StaffSearchResult>>($"/api/v1/staffsearch?searchQuery={searchString}&page={page}");
        }

        public async Task<LearnerDetail> GetLearner(int stdCode, long uln)
        {
            return await Get<LearnerDetail>($"/api/v1/learnerDetails?stdCode={stdCode}&uln={uln}");
        }

        public async Task<Certificate> GetCertificate(Guid certificateId)
        {
            return await Get<Certificate>($"api/v1/certificates/{certificateId}");
        }

        public async Task<ScheduleRun> GetNextScheduledRun()
        {
            return await Get<ScheduleRun>($"api/v1/schedule");
        }

        public async Task<Certificate> PostReprintRequest(StaffUIReprintRequest staffUiReprintRequest)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/v1/staffcertificatereprint"))
            {
                return await PostPutRequestWithResponse<StaffUIReprintRequest, Certificate>(httpRequest, staffUiReprintRequest);
            }
        }
    }
}