﻿using SFA.DAS.AssessorService.Application.Api.External.Messages;
using SFA.DAS.AssessorService.Application.Api.External.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Certificate = SFA.DAS.AssessorService.Application.Api.External.Models.Certificates.Certificate;
using SearchQuery = SFA.DAS.AssessorService.Application.Api.External.Models.Search.SearchQuery;
using SearchResult = SFA.DAS.AssessorService.Application.Api.External.Models.Search.SearchResult;

namespace SFA.DAS.AssessorService.Application.Api.External.Infrastructure
{
    public class SandboxApiClient : IApiClient
    {
        private readonly HttpClient _client;

        public SandboxApiClient(HttpClient client)
        {
            _client = client;
        }

        public Task<GetCertificateResponse> GetCertificate(GetCertificateRequest request)
        {
            var certificate = new Certificate
            {
                CertificateData = new Models.Certificates.CertificateData { CertificateReference = "SANDBOX" },
                Status = new Models.Certificates.CertificateStatus { CurrentStatus = "Draft", CreatedAt = DateTime.UtcNow, CreatedBy = request.Email },
            };

            var response = new GetCertificateResponse
            {
                Certificate = certificate,
                CertificateReference = request.CertificateReference,
                FamilyName = request.FamilyName,
                StandardCode = request.StandardCode,
                Uln = request.Uln
            };

            return Task.FromResult(response);
        }

        public Task<IEnumerable<BatchCertificateResponse>> CreateCertificates(IEnumerable<BatchCertificateRequest> request)
        {
            var response = new List<BatchCertificateResponse>();

            foreach (var req in request)
            {
                var responseItem = new BatchCertificateResponse
                {
                    ProvidedCertificateData = req.CertificateData,
                    ValidationErrors = new List<string>(),
                    Certificate = new Certificate
                    {
                        CertificateData = req.CertificateData ?? new Models.Certificates.CertificateData { },
                        Status = new Models.Certificates.CertificateStatus { CurrentStatus = "Draft", CreatedAt = DateTime.UtcNow, CreatedBy = req.Email },
                    }
                };

                responseItem.Certificate.CertificateData.CertificateReference = "SANDBOX";

                response.Add(responseItem);
            }

            return Task.FromResult(response.AsEnumerable());
        }

        public Task<IEnumerable<BatchCertificateResponse>> UpdateCertificates(IEnumerable<BatchCertificateRequest> request)
        {
            var response = new List<BatchCertificateResponse>();

            foreach (var req in request)
            {
                var responseItem = new BatchCertificateResponse
                {
                    ProvidedCertificateData = req.CertificateData,
                    ValidationErrors = new List<string>(),
                    Certificate = new Certificate
                    {
                        CertificateData = req.CertificateData ?? new Models.Certificates.CertificateData { },
                        Status = new Models.Certificates.CertificateStatus { CurrentStatus = "Draft", CreatedAt = DateTime.UtcNow.AddHours(-1), CreatedBy = req.Email, UpdatedAt = DateTime.UtcNow, UpdatedBy = req.Email },
                    }
                };

                responseItem.Certificate.CertificateData.CertificateReference = "SANDBOX";
                response.Add(responseItem);
            }

            return Task.FromResult(response.AsEnumerable());
        }

        public Task<IEnumerable<SubmitBatchCertificateResponse>> SubmitCertificates(IEnumerable<SubmitBatchCertificateRequest> request)
        {
            var response = new List<SubmitBatchCertificateResponse>();

            foreach (var req in request)
            {
                var responseItem = new SubmitBatchCertificateResponse
                {
                    Uln = req.Uln,
                    FamilyName = req.FamilyName,
                    StandardCode = req.StandardCode,
                    CertificateReference = req.CertificateReference,
                    ValidationErrors = new List<string>(),
                    Certificate = new Certificate
                    {
                        CertificateData = new Models.Certificates.CertificateData { },
                        Status = new Models.Certificates.CertificateStatus { CurrentStatus = "Submitted", CreatedAt = DateTime.UtcNow.AddHours(-1), CreatedBy = req.Email, UpdatedAt = DateTime.UtcNow, UpdatedBy = req.Email }
                    }
                };

                responseItem.Certificate.CertificateData.CertificateReference = "SANDBOX";
                response.Add(responseItem);
            }

            return Task.FromResult(response.AsEnumerable());
        }
        public Task<ApiResponse> DeleteCertificate(DeleteCertificateRequest request)
        {
            ApiResponse response = null;

            return Task.FromResult(response);
        }

        public Task<List<SearchResult>> Search(SearchQuery searchQuery, int? standardCode = null)
        {
            SearchResult result1 = new SearchResult { Uln = searchQuery.Uln, GivenNames = "SANDBOX", FamilyName = searchQuery.Surname, StdCode = standardCode ?? 99, CertificateStatus = "Draft", CertificateReference = "SANDBOX" };
            SearchResult result2 = new SearchResult { Uln = searchQuery.Uln, GivenNames = "SANDBOX", FamilyName = searchQuery.Surname, StdCode = 1 };

            List<SearchResult> response = new List<SearchResult> { result1 };

            if (!standardCode.HasValue)
            {
                response.Add(result2);
            }

            return Task.FromResult(response);
        }
    }
}
