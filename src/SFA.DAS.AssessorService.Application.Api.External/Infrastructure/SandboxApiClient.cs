using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.ExternalApi;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.External.Models.Internal;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.External.Infrastructure
{
    public sealed class SandboxApiClient : ApiClient
    {
        public SandboxApiClient(HttpClient client, ILogger<SandboxApiClient> logger, ITokenService tokenService) : base(client, logger, tokenService)
        {}

        private async Task<LearnerDetailForExternalApi> GetLearnerDetail(string standard, long uln)
        {
            return await Get<LearnerDetailForExternalApi>($"/api/v1/externalapi/learnerDetails?standard={standard}&uln={uln}");
        }

        public override async Task<GetLearnerResponse> GetLearner(GetBatchLearnerRequest request)
        {
            if (request != null)
            {
                var details = await GetLearnerDetail(request.Standard, request.Uln);

                if (details != null)
                {
                    request.Standard = details.StandardReference ?? details.StandardCode.ToString();
                    request.UkPrn = details.UkPrn;
                    request.Email = details.PrimaryContactEmail ?? request.Email;
                }
            }

            return await base.GetLearner(request);
        }

        public override async Task<IEnumerable<CreateEpaResponse>> CreateEpas(IEnumerable<CreateBatchEpaRequest> request)
        {
            if (request != null)
            {
                var newRequest = request.ToList();

                //foreach (var req in newRequest.Where(r => r.CertificateData?.Learner != null && r.CertificateData?.Standard != null))
                //{
                //    var standard = req.CertificateData.Standard.StandardCode.HasValue ? req.CertificateData.Standard.StandardCode.ToString() : req.CertificateData.Standard.StandardReference;
                //    var details = await GetLearnerDetail(standard, req.CertificateData.Learner.Uln);

                //    if (details != null)
                //    {
                //        req.CertificateData.Standard.StandardReference = details.StandardReference;
                //        req.CertificateData.Standard.StandardCode = details.StandardCode;
                //        req.UkPrn = details.UkPrn;
                //        req.Email = details.PrimaryContactEmail ?? req.Email;
                //    }
                //}

                return await base.CreateEpas(newRequest);
            }

            return await base.CreateEpas(request);
        }

        public override async Task<IEnumerable<UpdateEpaResponse>> UpdateEpas(IEnumerable<UpdateBatchEpaRequest> request)
        {
            if (request != null)
            {
                var newRequest = request.ToList();

                //foreach (var req in newRequest.Where(r => r.CertificateData?.Learner != null && r.CertificateData?.Standard != null))
                //{
                //    var standard = req.CertificateData.Standard.StandardCode.HasValue ? req.CertificateData.Standard.StandardCode.ToString() : req.CertificateData.Standard.StandardReference;
                //    var details = await GetLearnerDetail(standard, req.CertificateData.Learner.Uln);

                //    if (details != null)
                //    {
                //        req.CertificateData.Standard.StandardReference = details.StandardReference;
                //        req.CertificateData.Standard.StandardCode = details.StandardCode;
                //        req.UkPrn = details.UkPrn;
                //        req.Email = details.PrimaryContactEmail ?? req.Email;
                //    }
                //}

                return await base.UpdateEpas(newRequest);
            }

            return await base.UpdateEpas(request);
        }

        public override async Task<ApiResponse> DeleteEpa(DeleteBatchEpaRequest request)
        {
            var details = await GetLearnerDetail(request.Standard, request.Uln);

            if (details != null)
            {
                request.Standard = details.StandardReference ?? details.StandardCode.ToString();
                request.UkPrn = details.UkPrn;
                request.Email = details.PrimaryContactEmail ?? request.Email;
            }

            return await base.DeleteEpa(request);
        }

        public override async Task<GetCertificateResponse> GetCertificate(GetBatchCertificateRequest request)
        { 
            if(request != null)
            {
                var details = await GetLearnerDetail(request.Standard, request.Uln);

                if (details != null)
                {
                    request.Standard = details.StandardReference ?? details.StandardCode.ToString();
                    request.UkPrn = details.UkPrn;
                    request.Email = details.PrimaryContactEmail ?? request.Email;
                }
            }

            return await base.GetCertificate(request);
        }

        public override async Task<IEnumerable<CreateCertificateResponse>> CreateCertificates(IEnumerable<CreateBatchCertificateRequest> request)
        {
            if (request != null)
            {
                var newRequest = request.ToList();

                foreach (var req in newRequest.Where(r => r.CertificateData?.Learner != null && r.CertificateData?.Standard != null))
                {
                    var standard = req.CertificateData.Standard.StandardCode.HasValue ? req.CertificateData.Standard.StandardCode.ToString() : req.CertificateData.Standard.StandardReference;
                    var details = await GetLearnerDetail(standard, req.CertificateData.Learner.Uln);

                    if (details != null)
                    {
                        req.CertificateData.Standard.StandardReference = details.StandardReference;
                        req.CertificateData.Standard.StandardCode = details.StandardCode;
                        req.UkPrn = details.UkPrn;
                        req.Email = details.PrimaryContactEmail ?? req.Email;
                    }
                }

                return await base.CreateCertificates(newRequest);
            }

            return await base.CreateCertificates(request);
        }

        public override async Task<IEnumerable<UpdateCertificateResponse>> UpdateCertificates(IEnumerable<UpdateBatchCertificateRequest> request)
        {
            if (request != null)
            {
                var newRequest = request.ToList();

                foreach (var req in newRequest.Where(r => r.CertificateData?.Learner != null && r.CertificateData?.Standard != null))
                {
                    var standard = req.CertificateData.Standard.StandardCode.HasValue ? req.CertificateData.Standard.StandardCode.ToString() : req.CertificateData.Standard.StandardReference;
                    var details = await GetLearnerDetail(standard, req.CertificateData.Learner.Uln);

                    if (details != null)
                    {
                        req.CertificateData.Standard.StandardReference = details.StandardReference;
                        req.CertificateData.Standard.StandardCode = details.StandardCode;
                        req.UkPrn = details.UkPrn;
                        req.Email = details.PrimaryContactEmail ?? req.Email;
                    }
                }

                return await base.UpdateCertificates(newRequest);
            }

            return await base.UpdateCertificates(request);
        }

        public override async Task<IEnumerable<SubmitCertificateResponse>> SubmitCertificates(IEnumerable<SubmitBatchCertificateRequest> request)
        {
            if (request != null)
            {
                var newRequest = request.ToList();

                foreach (var req in newRequest)
                {
                    var standard = req.StandardCode.HasValue ? req.StandardCode.ToString() : req.StandardReference;
                    var details = await GetLearnerDetail(standard, req.Uln);

                    if (details != null)
                    {
                        req.StandardReference = details.StandardReference;
                        req.StandardCode = details.StandardCode;
                        req.UkPrn = details.UkPrn;
                        req.Email = details.PrimaryContactEmail ?? req.Email;
                    }
                }

                return await base.SubmitCertificates(newRequest);
            }

            return await base.SubmitCertificates(request);
        }

        public override async Task<ApiResponse> DeleteCertificate(DeleteBatchCertificateRequest request)
        {
            var details = await GetLearnerDetail(request.Standard, request.Uln);

            if (details != null)
            {
                request.Standard = details.StandardReference ?? details.StandardCode.ToString();
                request.UkPrn = details.UkPrn;
                request.Email = details.PrimaryContactEmail ?? request.Email;
            }

            return await base.DeleteCertificate(request);
        }
    }
}
