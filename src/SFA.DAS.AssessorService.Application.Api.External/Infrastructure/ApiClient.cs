using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.External.Messages;
using SFA.DAS.AssessorService.Application.Api.External.Middleware;
using SFA.DAS.AssessorService.Application.Api.External.Models.Certificates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SearchQuery = SFA.DAS.AssessorService.Application.Api.External.Models.Search.SearchQuery;
using SearchResult = SFA.DAS.AssessorService.Application.Api.External.Models.Search.SearchResult;

namespace SFA.DAS.AssessorService.Application.Api.External.Infrastructure
{
    public class ApiClient : IApiClient
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

            using (var response = await _client.GetAsync(new Uri(uri, UriKind.Relative)))
            {
                return await response.Content.ReadAsAsync<T>();
            }
        }

        protected async Task<U> Post<T, U>(string uri, T model)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
            var serializeObject = JsonConvert.SerializeObject(model);

            using (var response = await _client.PostAsync(new Uri(uri, UriKind.Relative), new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json")))
            {
                return await response.Content.ReadAsAsync<U>();
            }
        }

        protected async Task<U> Put<T, U>(string uri, T model)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
            var serializeObject = JsonConvert.SerializeObject(model);

            using (var response = await _client.PutAsync(new Uri(uri, UriKind.Relative), new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json")))
            {
                return await response.Content.ReadAsAsync<U>();
            }
        }

        protected async Task<T> Delete<T>(string uri)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

            using (var response = await _client.DeleteAsync(new Uri(uri, UriKind.Relative)))
            {
                return await response.Content.ReadAsAsync<T>();
            }
        }

        public async Task<List<SearchResult>> Search(SearchQuery searchQuery, int? standardCode = null)
        {
            List<SearchResult> results = await Post<SearchQuery, List<SearchResult>>("/api/v1/search", searchQuery);
            return results.Where(s => standardCode is null || s.StdCode == standardCode).ToList();
        }

        public async Task<Certificate> GetCertificate(GetCertificateRequest request)
        { 
            var apiResponse = await Get<Domain.Entities.Certificate>($"/api/v1/certificates/batch/{request.Uln}/{request.FamilyName}/{request.StandardCode}/{request.CertificateReference}/{request.UkPrn}/{request.Email}");

            return Mapper.Map<Domain.Entities.Certificate, Certificate>(apiResponse);
        }

        public async Task<IEnumerable<BatchCertificateResponse>> CreateCertificates(IEnumerable<BatchCertificateRequest> request)
        {
            var apiRequest = Mapper.Map<IEnumerable<BatchCertificateRequest>,IEnumerable<AssessorService.Api.Types.Models.Certificates.Batch.CreateBatchCertificateRequest>>(request);

            var apiResponse = await Post<IEnumerable<AssessorService.Api.Types.Models.Certificates.Batch.CreateBatchCertificateRequest>, IEnumerable<AssessorService.Api.Types.Models.Certificates.Batch.BatchCertificateResponse>>("/api/v1/certificates/batch", apiRequest);

            return Mapper.Map<IEnumerable<AssessorService.Api.Types.Models.Certificates.Batch.BatchCertificateResponse>, IEnumerable<BatchCertificateResponse>>(apiResponse);
        }

        public async Task<IEnumerable<BatchCertificateResponse>> UpdateCertificates(IEnumerable<BatchCertificateRequest> request)
        {
            var apiRequest = Mapper.Map<IEnumerable<BatchCertificateRequest>, IEnumerable<AssessorService.Api.Types.Models.Certificates.Batch.UpdateBatchCertificateRequest>>(request);

            var apiResponse = await Put<IEnumerable<AssessorService.Api.Types.Models.Certificates.Batch.UpdateBatchCertificateRequest>, IEnumerable<AssessorService.Api.Types.Models.Certificates.Batch.BatchCertificateResponse>>("/api/v1/certificates/batch", apiRequest);

            return Mapper.Map<IEnumerable<AssessorService.Api.Types.Models.Certificates.Batch.BatchCertificateResponse>, IEnumerable<BatchCertificateResponse>>(apiResponse);
        }

        public async Task<IEnumerable<SubmitBatchCertificateResponse>> SubmitCertificates(IEnumerable<SubmitBatchCertificateRequest> request)
        {
            var apiRequest = Mapper.Map<IEnumerable<SubmitBatchCertificateRequest>, IEnumerable<AssessorService.Api.Types.Models.Certificates.Batch.SubmitBatchCertificateRequest>>(request);

            var apiResponse = await Post<IEnumerable<AssessorService.Api.Types.Models.Certificates.Batch.SubmitBatchCertificateRequest>, IEnumerable<AssessorService.Api.Types.Models.Certificates.Batch.SubmitBatchCertificateResponse>>("/api/v1/certificates/batch/submit", apiRequest);

            return Mapper.Map<IEnumerable<AssessorService.Api.Types.Models.Certificates.Batch.SubmitBatchCertificateResponse>, IEnumerable<SubmitBatchCertificateResponse>>(apiResponse);
        }

        public async Task<ApiResponse> DeleteCertificate(DeleteCertificateRequest request)
        {
            var apiResponse = await Delete<ApiResponse>($"/api/v1/certificates/batch/{request.Uln}/{request.FamilyName}/{request.StandardCode}/{request.CertificateReference}/{request.UkPrn}/{request.Email}");

            return apiResponse;
        }
    }
}
