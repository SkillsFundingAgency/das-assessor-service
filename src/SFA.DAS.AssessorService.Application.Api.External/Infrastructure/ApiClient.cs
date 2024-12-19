using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.External.Models.Internal;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Standards;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.External.Infrastructure
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiClient> _logger;
        private readonly IMapper _mapper;

        public ApiClient(IAssessorApiClientFactory clientFactory, ILogger<ApiClient> logger, IMapper mapper)
        {
            _httpClient = clientFactory.CreateHttpClient();
            _logger = logger;
            _mapper = mapper;
        }

        protected async Task<T> Get<T>(string uri)
        {
            using (var response = await _httpClient.GetAsync(new Uri(uri, UriKind.Relative)))
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
                }
                catch (Exception ex)
                {
                    var actualResponse = string.Empty;
                    try
                    {
                        actualResponse = await response.Content.ReadAsStringAsync();
                    }
                    catch
                    {
                        // safe to ignore any errors
                    }
                    _logger.LogError(ex, $"GET: HTTP {(int)response.StatusCode} Error getting response from: {uri} - ActualResponse: {actualResponse}");
                    throw;
                }
            }
        }

        protected async Task<U> Post<T, U>(string uri, T model)
        {
            var serializeObject = JsonConvert.SerializeObject(model);

            using (var response = await _httpClient.PostAsync(new Uri(uri, UriKind.Relative), new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json")))
            {
                try
                {
                    return JsonConvert.DeserializeObject<U>(await response.Content.ReadAsStringAsync());
                }
                catch (Exception ex)
                {
                    var actualResponse = string.Empty;
                    try
                    {
                        actualResponse = await response.Content.ReadAsStringAsync();
                    }
                    catch
                    {
                        // safe to ignore any errors
                    }
                    _logger.LogError(ex, $"POST: HTTP {(int)response.StatusCode} Error getting response from: {uri} - ActualResponse: {actualResponse}");
                    throw;
                }
            }
        }

        protected async Task<U> Put<T, U>(string uri, T model)
        {
            var serializeObject = JsonConvert.SerializeObject(model);

            using (var response = await _httpClient.PutAsync(new Uri(uri, UriKind.Relative), new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json")))
            {
                try
                {
                    return JsonConvert.DeserializeObject<U>(await response.Content.ReadAsStringAsync());
                }
                catch (Exception ex)
                {
                    var actualResponse = string.Empty;
                    try
                    {
                        actualResponse = await response.Content.ReadAsStringAsync();
                    }
                    catch
                    {
                        // safe to ignore any errors
                    }
                    _logger.LogError(ex, $"PUT: HTTP {(int)response.StatusCode} Error getting response from: {uri} - ActualResponse: {actualResponse}");
                    throw;
                }
            }
        }

        protected async Task<T> Delete<T>(string uri)
        {
            using (var response = await _httpClient.DeleteAsync(new Uri(uri, UriKind.Relative)))
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
                }
                catch (Exception ex)
                {
                    var actualResponse = string.Empty;
                    try
                    {
                        actualResponse = await response.Content.ReadAsStringAsync();
                    }
                    catch
                    {
                        // safe to ignore any errors
                    }
                    _logger.LogError(ex, $"DELETE: HTTP {(int)response.StatusCode} Error getting response from: {uri} - ActualResponse: {actualResponse}");
                    throw;
                }
            }
        }

        public virtual async Task<GetLearnerResponse> GetLearner(GetBatchLearnerRequest request)
        {
            var apiResponse = await Get<AssessorService.Api.Types.Models.ExternalApi.Learners.GetBatchLearnerResponse>($"/api/v1/learners/batch/{request.Uln}/{request.FamilyName}/{request.Standard}/{request.UkPrn}");

            return _mapper.Map<AssessorService.Api.Types.Models.ExternalApi.Learners.GetBatchLearnerResponse, GetLearnerResponse>(apiResponse);
        }


        public virtual async Task<IEnumerable<CreateEpaResponse>> CreateEpas(IEnumerable<CreateBatchEpaRequest> request)
        {
            var apiRequest = _mapper.Map<IEnumerable<CreateBatchEpaRequest>, IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Epas.CreateBatchEpaRequest>>(request);

            var apiResponse = await Post<IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Epas.CreateBatchEpaRequest>, IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Epas.BatchEpaResponse>>("/api/v1/epas/batch", apiRequest);

            return _mapper.Map<IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Epas.BatchEpaResponse>, IEnumerable<CreateEpaResponse>>(apiResponse);
        }

        public virtual async Task<IEnumerable<UpdateEpaResponse>> UpdateEpas(IEnumerable<UpdateBatchEpaRequest> request)
        {
            var apiRequest = _mapper.Map<IEnumerable<UpdateBatchEpaRequest>, IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Epas.UpdateBatchEpaRequest>>(request);

            var apiResponse = await Put<IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Epas.UpdateBatchEpaRequest>, IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Epas.BatchEpaResponse>>("/api/v1/epas/batch", apiRequest);

            return _mapper.Map<IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Epas.BatchEpaResponse>, IEnumerable<UpdateEpaResponse>>(apiResponse);
        }

        public virtual async Task<ApiResponse> DeleteEpa(DeleteBatchEpaRequest request)
        {
            var apiResponse = await Delete<ApiResponse>($"/api/v1/epas/batch/{request.Uln}/{request.FamilyName}/{request.Standard}/{request.EpaReference}/{request.UkPrn}");

            return apiResponse;
        }

        public virtual async Task<GetCertificateResponse> GetCertificate(GetBatchCertificateRequest request)
        {
            var apiResponse = await Get<AssessorService.Api.Types.Models.ExternalApi.Certificates.GetBatchCertificateResponse>($"/api/v1/certificates/batch/{request.Uln}/{request.FamilyName}/{request.Standard}/{request.UkPrn}");

            return _mapper.Map<AssessorService.Api.Types.Models.ExternalApi.Certificates.GetBatchCertificateResponse, GetCertificateResponse>(apiResponse);
        }

        public virtual async Task<IEnumerable<CreateCertificateResponse>> CreateCertificates(IEnumerable<CreateBatchCertificateRequest> request)
        {
            var apiRequest = _mapper.Map<IEnumerable<CreateBatchCertificateRequest>, IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Certificates.CreateBatchCertificateRequest>>(request);

            var apiResponse = await Post<IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Certificates.CreateBatchCertificateRequest>, IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Certificates.BatchCertificateResponse>>("/api/v1/certificates/batch", apiRequest);

            return _mapper.Map<IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Certificates.BatchCertificateResponse>, IEnumerable<CreateCertificateResponse>>(apiResponse);
        }

        public virtual async Task<IEnumerable<UpdateCertificateResponse>> UpdateCertificates(IEnumerable<UpdateBatchCertificateRequest> request)
        {
            var apiRequest = _mapper.Map<IEnumerable<UpdateBatchCertificateRequest>, IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Certificates.UpdateBatchCertificateRequest>>(request);

            var apiResponse = await Put<IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Certificates.UpdateBatchCertificateRequest>, IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Certificates.BatchCertificateResponse>>("/api/v1/certificates/batch", apiRequest);

            return _mapper.Map<IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Certificates.BatchCertificateResponse>, IEnumerable<UpdateCertificateResponse>>(apiResponse);
        }

        public virtual async Task<IEnumerable<SubmitCertificateResponse>> SubmitCertificates(IEnumerable<SubmitBatchCertificateRequest> request)
        {
            var apiRequest = _mapper.Map<IEnumerable<SubmitBatchCertificateRequest>, IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Certificates.SubmitBatchCertificateRequest>>(request);

            var apiResponse = await Post<IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Certificates.SubmitBatchCertificateRequest>, IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Certificates.SubmitBatchCertificateResponse>>("/api/v1/certificates/batch/submit", apiRequest);

            return _mapper.Map<IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Certificates.SubmitBatchCertificateResponse>, IEnumerable<SubmitCertificateResponse>>(apiResponse);
        }

        public virtual async Task<ApiResponse> DeleteCertificate(DeleteBatchCertificateRequest request)
        {
            var apiResponse = await Delete<ApiResponse>($"/api/v1/certificates/batch/{request.Uln}/{request.FamilyName}/{request.Standard}/{request.CertificateReference}/{request.UkPrn}");

            return apiResponse;
        }

        public virtual async Task<IEnumerable<StandardOptions>> GetStandardOptionsForLatestStandardVersions()
        {
            var response = await Get<IEnumerable<StandardOptions>>("/api/v1/standard-version/standard-options/latest-version");

            return response;
        }

        public virtual async Task<StandardOptions> GetStandardOptionsByStandard(string standard)
        {
            var response = await Get<StandardOptions>($"/api/v1/standard-version/standard-options/{standard}");

            return response;
        }

        public virtual async Task<StandardOptions> GetStandardOptionsByStandardIdAndVersion(string standard, string version)
        {
            var response = await Get<StandardOptions>($"/api/v1/standard-version/standard-options/{standard}/{version}");

            return response;
        }
    }
}
