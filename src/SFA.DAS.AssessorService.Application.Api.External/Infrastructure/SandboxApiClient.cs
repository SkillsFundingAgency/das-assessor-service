using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi.Learners;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.External.Models.Internal;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GetBatchLearnerRequest = SFA.DAS.AssessorService.Application.Api.External.Models.Internal.GetBatchLearnerRequest;

namespace SFA.DAS.AssessorService.Application.Api.External.Infrastructure
{
    public sealed class SandboxApiClient : ApiClient
    {
        private readonly ILogger<SandboxApiClient> _logger;
        public SandboxApiClient(HttpClient client, ILogger<SandboxApiClient> logger, ITokenService tokenService) : base(client, logger, tokenService)
        {
            _logger = logger;
        }

        private async Task<LearnerDetailForExternalApi> GetLearnerDetail(string standard, long uln)
        {
            return await Get<LearnerDetailForExternalApi>($"/api/v1/externalapi/learnerDetails?standard={standard}&uln={uln}");
        }

        public override async Task<GetLearnerResponse> GetLearner(GetBatchLearnerRequest request)
        {
            if (request != null)
            {
                _logger.LogInformation($"GetLearner called with request: {JsonConvert.SerializeObject(request)}");
                var details = await GetLearnerDetail(request.Standard, request.Uln);

                if (details != null)
                {
                    request.Standard = details.Standard?.IFateReferenceNumber ?? details.Standard?.LarsCode.ToString();
                    request.UkPrn = details.UkPrn;
                }
            }

            var response = await base.GetLearner(request);
            _logger.LogInformation($"GetLearner returned with response: {JsonConvert.SerializeObject(response)}");
            return response;
        }

        public override async Task<IEnumerable<CreateEpaResponse>> CreateEpas(IEnumerable<CreateBatchEpaRequest> request)
        {
            if (request != null)
            {
                _logger.LogInformation($"CreateEpas called with request: {JsonConvert.SerializeObject(request)}");
                var newRequest = request.ToList();

                foreach (var req in newRequest.Where(r => r.Learner != null && r.Standard != null))
                {
                    var standard = req.Standard.StandardCode.HasValue ? req.Standard.StandardCode.ToString() : req.Standard.StandardReference;
                    var details = await GetLearnerDetail(standard, req.Learner.Uln);

                    if (details != null)
                    {
                        req.Standard.StandardReference = details.Standard?.IFateReferenceNumber;
                        req.Standard.StandardCode = details.Standard?.LarsCode;
                        req.UkPrn = details.UkPrn;
                    }
                }

                var response = await base.CreateEpas(newRequest);
                _logger.LogInformation($"CreateEpas returned with response: {JsonConvert.SerializeObject(response)}");
                return response;
            }

            return await base.CreateEpas(request);
        }

        public override async Task<IEnumerable<UpdateEpaResponse>> UpdateEpas(IEnumerable<UpdateBatchEpaRequest> request)
        {
            if (request != null)
            {
                _logger.LogInformation($"UpdateEpas called with request: {JsonConvert.SerializeObject(request)}");
                var newRequest = request.ToList();

                foreach (var req in newRequest.Where(r => r.Learner != null && r.Standard != null))
                {
                    var standard = req.Standard.StandardCode.HasValue ? req.Standard.StandardCode.ToString() : req.Standard.StandardReference;
                    var details = await GetLearnerDetail(standard, req.Learner.Uln);

                    if (details != null)
                    {
                        req.Standard.StandardReference = details.Standard?.IFateReferenceNumber;
                        req.Standard.StandardCode = details.Standard?.LarsCode;
                        req.UkPrn = details.UkPrn;
                    }
                }

                var response = await base.UpdateEpas(newRequest);
                _logger.LogInformation($"UpdateEpas returned with response: {JsonConvert.SerializeObject(response)}");
                return response;
            }

            return await base.UpdateEpas(request);
        }

        public override async Task<ApiResponse> DeleteEpa(DeleteBatchEpaRequest request)
        {
            if (request != null)
            {
                _logger.LogInformation($"DeleteEpa called with request: {JsonConvert.SerializeObject(request)}");
                var details = await GetLearnerDetail(request.Standard, request.Uln);

                if (details != null)
                {
                    request.Standard = details.Standard?.IFateReferenceNumber ?? details.Standard?.LarsCode.ToString();
                    request.UkPrn = details.UkPrn;
                }
            }

            var response = await base.DeleteEpa(request);
            _logger.LogInformation($"DeleteEpa returned with response: {JsonConvert.SerializeObject(response)}");
            return response;
        }

        public override async Task<GetCertificateResponse> GetCertificate(GetBatchCertificateRequest request)
        {
            if (request != null)
            {
                _logger.LogInformation($"GetCertificate called with request: {JsonConvert.SerializeObject(request)}");
                var details = await GetLearnerDetail(request.Standard, request.Uln);

                if (details != null)
                {
                    request.Standard = details.Standard?.IFateReferenceNumber ?? details.Standard?.LarsCode.ToString();
                    request.UkPrn = details.UkPrn;
                }
            }

            var response = await base.GetCertificate(request);
            _logger.LogInformation($"GetCertificate returned with response: {JsonConvert.SerializeObject(response)}");
            return response;
        }

        public override async Task<IEnumerable<CreateCertificateResponse>> CreateCertificates(IEnumerable<CreateBatchCertificateRequest> request)
        {
            if (request != null)
            {
                _logger.LogInformation($"CreateCertificates called with request: {JsonConvert.SerializeObject(request)}");
                var newRequest = request.ToList();

                foreach (var req in newRequest.Where(r => r.CertificateData?.Learner != null && r.CertificateData?.Standard != null))
                {
                    var standard = req.CertificateData.Standard.StandardCode.HasValue ? req.CertificateData.Standard.StandardCode.ToString() : req.CertificateData.Standard.StandardReference;
                    var details = await GetLearnerDetail(standard, req.CertificateData.Learner.Uln);

                    if (details != null)
                    {
                        req.CertificateData.Standard.StandardReference = details.Standard?.IFateReferenceNumber;
                        req.CertificateData.Standard.StandardCode = details.Standard?.LarsCode;
                        req.UkPrn = details.UkPrn;
                    }
                }

                var response = await base.CreateCertificates(newRequest);
                _logger.LogInformation($"CreateCertificates returned with response: {JsonConvert.SerializeObject(response)}");
                return response;
            }

            return await base.CreateCertificates(request);
        }

        public override async Task<IEnumerable<UpdateCertificateResponse>> UpdateCertificates(IEnumerable<UpdateBatchCertificateRequest> request)
        {
            if (request != null)
            {
                _logger.LogInformation($"UpdateCertificates called with request: {JsonConvert.SerializeObject(request)}");
                var newRequest = request.ToList();

                foreach (var req in newRequest.Where(r => r.CertificateData?.Learner != null && r.CertificateData?.Standard != null))
                {
                    var standard = req.CertificateData.Standard.StandardCode.HasValue ? req.CertificateData.Standard.StandardCode.ToString() : req.CertificateData.Standard.StandardReference;
                    var details = await GetLearnerDetail(standard, req.CertificateData.Learner.Uln);

                    if (details != null)
                    {
                        req.CertificateData.Standard.StandardReference = details.Standard?.IFateReferenceNumber;
                        req.CertificateData.Standard.StandardCode = details.Standard?.LarsCode;
                        req.UkPrn = details.UkPrn;
                    }
                }

                var response = await base.UpdateCertificates(newRequest);
                _logger.LogInformation($"UpdateCertificates returned with response: {JsonConvert.SerializeObject(response)}");
                return response;
            }

            return await base.UpdateCertificates(request);
        }

        public override async Task<IEnumerable<SubmitCertificateResponse>> SubmitCertificates(IEnumerable<SubmitBatchCertificateRequest> request)
        {
            if (request != null)
            {
                _logger.LogInformation($"SubmitCertificates called with request: {JsonConvert.SerializeObject(request)}");
                var newRequest = request.ToList();

                foreach (var req in newRequest)
                {
                    var standard = req.StandardCode.HasValue ? req.StandardCode.ToString() : req.StandardReference;
                    var details = await GetLearnerDetail(standard, req.Uln);

                    if (details != null)
                    {
                        req.StandardReference = details.Standard?.IFateReferenceNumber;
                        req.StandardCode = details.Standard?.LarsCode;
                        req.UkPrn = details.UkPrn;
                    }
                }

                var response = await base.SubmitCertificates(newRequest);
                _logger.LogInformation($"SubmitCertificates returned with response: {JsonConvert.SerializeObject(response)}");
                return response;
            }

            return await base.SubmitCertificates(request);
        }

        public override async Task<ApiResponse> DeleteCertificate(DeleteBatchCertificateRequest request)
        {
            if (request != null)
            {
                _logger.LogInformation($"DeleteCertificate called with request: {JsonConvert.SerializeObject(request)}");
                var details = await GetLearnerDetail(request.Standard, request.Uln);

                if (details != null)
                {
                    request.Standard = details.Standard?.IFateReferenceNumber ?? details.Standard?.LarsCode.ToString();
                    request.UkPrn = details.UkPrn;
                }
            }

            var response = await base.DeleteCertificate(request);
            _logger.LogInformation($"DeleteCertificate returned with response: {JsonConvert.SerializeObject(response)}");
            return response;
        }
    }
}
