using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Application.Api.Client.Exceptions;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IApiClient
    {
        Task<PaginatedList<CertificateHistoryResponse>> GetCertificateHistory(int pageIndex, string userName);
        Task<PaginatedList<CertificateHistoryResponse>> GetCertificateHistoryXXX(int pageIndex, string userName);
    }

    public class ApiClient : IApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<ApiClient> _logger;
        private readonly ITokenService _tokenService;

        public ApiClient(HttpClient client,           
            ILogger<ApiClient> logger,
            ITokenService tokenService)
        {
            _client = client;
            _logger = logger;
            _tokenService = tokenService;            
        }

        protected readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        private async Task<T> Get<T>(string uri)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
            var result = await _client.GetAsync(new Uri(uri, UriKind.Relative));

            if (result.StatusCode == HttpStatusCode.OK)
            {
                var json = await result.Content.ReadAsStringAsync();
                return await Task.Factory.StartNew<T>(() => JsonConvert.DeserializeObject<T>(json, _jsonSettings));
            }
            else
            {
                throw new ApplicationException("General Errro ...");
            }
        }

        //protected async Task<U> Post<T, U>(string uri, T model)
        //{
        //    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

        //    var serializeObject = JsonConvert.SerializeObject(model);

        //    using (var response = await _client.PostAsync(new Uri(uri, UriKind.Relative), new StringContent(serializeObject,System.Text.Encoding.UTF8, "application/json")))
        //    {
        //        return await response.Content.ReadAsStringAsync<U>();
        //    }
        //}

        public async Task<List<CertificateResponse>> GetCertificates()
        {
            return await Get<List<CertificateResponse>>("/api/v1/certificates?statusses=Submitted");
        }

        public async Task<PaginatedList<CertificateHistoryResponse>> GetCertificateHistory(int pageIndex, string userName)
        {
            return await Get<PaginatedList<CertificateHistoryResponse>>($"api/v1/certificates/history/?pageIndex={pageIndex}&userName={userName}");
        }

        public async Task<PaginatedList<CertificateHistoryResponse>> GetCertificateHistoryXXX(int pageIndex, string userName)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"api/v1/certificates/history/?pageIndex={pageIndex}&userName={userName}"))
            {
                return await RequestAndDeserialiseAsync<PaginatedList<CertificateHistoryResponse>>(httpRequest, "Could not get Certificate History");
            }
        }

        protected async Task<T> RequestAndDeserialiseAsync<T>(HttpRequestMessage request, string message = null) where T : class
        {
            request.Headers.Add("Accept", "application/json");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

            using (var response = _client.SendAsync(request))
            {
                var result = await response;
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    var json = await result.Content.ReadAsStringAsync();
                    return await Task.Factory.StartNew<T>(() => JsonConvert.DeserializeObject<T>(json, _jsonSettings));
                }               
            }

            return null;
        }
    }
}