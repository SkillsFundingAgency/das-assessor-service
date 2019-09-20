using AutoMapper;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.External.Models.Internal;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Standards;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.External.Infrastructure
{
    public class ApiClient : IApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<ApiClient> _logger;
        private readonly ITokenService _tokenService;

        public ApiClient(HttpClient client, ILogger<ApiClient> logger, ITokenService tokenService)
        {
            _client = client;
            _logger = logger;
            _tokenService = tokenService;
        }

        protected async Task<T> Get<T>(string uri)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

            try
            {
                using (var response = await _client.GetAsync(new Uri(uri, UriKind.Relative)))
                {
                    try
                    {
                        return await response.Content.ReadAsAsync<T>();
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
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"GET: HTTP Error when processing request to: {uri}");
                throw;
            }
        }

        protected async Task<U> Post<T, U>(string uri, T model)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
            var serializeObject = JsonConvert.SerializeObject(model);

            try
            {
                using (var response = await _client.PostAsync(new Uri(uri, UriKind.Relative), new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json")))
                {
                    try
                    {
                        return await response.Content.ReadAsAsync<U>();
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
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"POST: HTTP Error when processing request to: {uri}");
                throw;
            }
        }

        protected async Task<U> Put<T, U>(string uri, T model)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
            var serializeObject = JsonConvert.SerializeObject(model);

            try
            {
                using (var response = await _client.PutAsync(new Uri(uri, UriKind.Relative), new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json")))
                {
                    try
                    {
                        return await response.Content.ReadAsAsync<U>();
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
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"PUT: HTTP Error when processing request to: {uri}");
                throw;
            }
        }

        protected async Task<T> Delete<T>(string uri)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

            try
            {
                using (var response = await _client.DeleteAsync(new Uri(uri, UriKind.Relative)))
                {
                    try
                    {
                        return await response.Content.ReadAsAsync<T>();
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
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"GET: DELETE Error when processing request to: {uri}");
                throw;
            }
        }

        public virtual async Task<GetLearnerResponse> GetLearner(GetBatchLearnerRequest request)
        {
            var apiResponse = await Get<AssessorService.Api.Types.Models.ExternalApi.Learners.GetBatchLearnerResponse>($"/api/v1/learners/batch/{request.Uln}/{request.FamilyName}/{request.Standard}/{request.UkPrn}");

            return Mapper.Map<AssessorService.Api.Types.Models.ExternalApi.Learners.GetBatchLearnerResponse, GetLearnerResponse>(apiResponse);
        }


        public virtual async Task<IEnumerable<CreateEpaResponse>> CreateEpas(IEnumerable<CreateBatchEpaRequest> request)
        {
            var apiRequest = Mapper.Map<IEnumerable<CreateBatchEpaRequest>, IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Epas.CreateBatchEpaRequest>>(request);

            var apiResponse = await Post<IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Epas.CreateBatchEpaRequest>, IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Epas.BatchEpaResponse>>("/api/v1/epas/batch", apiRequest);

            return Mapper.Map<IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Epas.BatchEpaResponse>, IEnumerable<CreateEpaResponse>>(apiResponse);
        }

        public virtual async Task<IEnumerable<UpdateEpaResponse>> UpdateEpas(IEnumerable<UpdateBatchEpaRequest> request)
        {
            var apiRequest = Mapper.Map<IEnumerable<UpdateBatchEpaRequest>, IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Epas.UpdateBatchEpaRequest>>(request);

            var apiResponse = await Put<IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Epas.UpdateBatchEpaRequest>, IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Epas.BatchEpaResponse>>("/api/v1/epas/batch", apiRequest);

            return Mapper.Map<IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Epas.BatchEpaResponse>, IEnumerable<UpdateEpaResponse>>(apiResponse);
        }

        public virtual async Task<ApiResponse> DeleteEpa(DeleteBatchEpaRequest request)
        {
            var apiResponse = await Delete<ApiResponse>($"/api/v1/epas/batch/{request.Uln}/{request.FamilyName}/{request.Standard}/{request.EpaReference}/{request.UkPrn}");

            return apiResponse;
        }


        public virtual async Task<GetCertificateResponse> GetCertificate(GetBatchCertificateRequest request)
        {
            var apiResponse = await Get<AssessorService.Api.Types.Models.ExternalApi.Certificates.GetBatchCertificateResponse>($"/api/v1/certificates/batch/{request.Uln}/{request.FamilyName}/{request.Standard}/{request.UkPrn}");

            return Mapper.Map<AssessorService.Api.Types.Models.ExternalApi.Certificates.GetBatchCertificateResponse, GetCertificateResponse>(apiResponse);
        }

        public virtual async Task<IEnumerable<CreateCertificateResponse>> CreateCertificates(IEnumerable<CreateBatchCertificateRequest> request)
        {
            var apiRequest = Mapper.Map<IEnumerable<CreateBatchCertificateRequest>, IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Certificates.CreateBatchCertificateRequest>>(request);

            var apiResponse = await Post<IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Certificates.CreateBatchCertificateRequest>, IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Certificates.BatchCertificateResponse>>("/api/v1/certificates/batch", apiRequest);

            return Mapper.Map<IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Certificates.BatchCertificateResponse>, IEnumerable<CreateCertificateResponse>>(apiResponse);
        }

        public virtual async Task<IEnumerable<UpdateCertificateResponse>> UpdateCertificates(IEnumerable<UpdateBatchCertificateRequest> request)
        {
            var apiRequest = Mapper.Map<IEnumerable<UpdateBatchCertificateRequest>, IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Certificates.UpdateBatchCertificateRequest>>(request);

            var apiResponse = await Put<IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Certificates.UpdateBatchCertificateRequest>, IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Certificates.BatchCertificateResponse>>("/api/v1/certificates/batch", apiRequest);

            return Mapper.Map<IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Certificates.BatchCertificateResponse>, IEnumerable<UpdateCertificateResponse>>(apiResponse);
        }

        public virtual async Task<IEnumerable<SubmitCertificateResponse>> SubmitCertificates(IEnumerable<SubmitBatchCertificateRequest> request)
        {
            var apiRequest = Mapper.Map<IEnumerable<SubmitBatchCertificateRequest>, IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Certificates.SubmitBatchCertificateRequest>>(request);

            var apiResponse = await Post<IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Certificates.SubmitBatchCertificateRequest>, IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Certificates.SubmitBatchCertificateResponse>>("/api/v1/certificates/batch/submit", apiRequest);

            return Mapper.Map<IEnumerable<AssessorService.Api.Types.Models.ExternalApi.Certificates.SubmitBatchCertificateResponse>, IEnumerable<SubmitCertificateResponse>>(apiResponse);
        }

        public virtual async Task<ApiResponse> DeleteCertificate(DeleteBatchCertificateRequest request)
        {
            var apiResponse = await Delete<ApiResponse>($"/api/v1/certificates/batch/{request.Uln}/{request.FamilyName}/{request.Standard}/{request.CertificateReference}/{request.UkPrn}");

            return apiResponse;
        }


        public virtual async Task<IEnumerable<StandardOptions>> GetStandards()
        {
            var apiResponse = await Get<IEnumerable<AssessorService.Api.Types.Models.Standards.StandardCollation>>($"/api/ao/assessment-organisations/collated-standards");

            return Mapper.Map<IEnumerable<AssessorService.Api.Types.Models.Standards.StandardCollation>, IEnumerable<StandardOptions>>(apiResponse);
        }

        public virtual async Task<StandardOptions> GetStandard(string standard)
        {
            AssessorService.Api.Types.Models.Standards.StandardCollation apiResponse = null;

            if (int.TryParse(standard, out int standardId))
            {
                apiResponse = await Get<AssessorService.Api.Types.Models.Standards.StandardCollation>($"/api/ao/assessment-organisations/collated-standards/{standardId}");
            }

            if (apiResponse is null)
            {
                apiResponse = await Get<AssessorService.Api.Types.Models.Standards.StandardCollation>($"/api/ao/assessment-organisations/collated-standards/by-reference/{standard}");
            }

            return Mapper.Map<AssessorService.Api.Types.Models.Standards.StandardCollation, StandardOptions>(apiResponse);
        }
    }
}
