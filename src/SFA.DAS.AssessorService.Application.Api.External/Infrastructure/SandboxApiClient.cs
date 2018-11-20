using SFA.DAS.AssessorService.Application.Api.External.Messages;
using SFA.DAS.AssessorService.Application.Api.External.Middleware;
using SFA.DAS.AssessorService.Application.Api.External.Models.Certificates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;


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
                CertificateData = new CertificateData { CertificateReference = "SANDBOX" },
                Status = new Status { CurrentStatus = "Draft" },
                Created = new Created { CreatedAt = DateTime.UtcNow, CreatedBy = request.Email },
            };

            var response = new GetCertificateResponse
            {
                Certificate = certificate,
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
                var validationErrors = PerformBasicValidation(req.CertificateData);
                var responseItem = new BatchCertificateResponse
                {
                    ProvidedCertificateData = req.CertificateData,
                    ValidationErrors = validationErrors,
                    Certificate = validationErrors.Count > 0 ? null : new Certificate
                    {
                        CertificateData = req.CertificateData ?? new CertificateData { },
                        Status = new Status { CurrentStatus = "Draft" },
                        Created = new Created { CreatedAt = DateTime.UtcNow, CreatedBy = req.Email },
                    }
                };

                if (responseItem.Certificate != null) responseItem.Certificate.CertificateData.CertificateReference = "SANDBOX";

                response.Add(responseItem);
            }

            return Task.FromResult(response.AsEnumerable());
        }

        public Task<IEnumerable<BatchCertificateResponse>> UpdateCertificates(IEnumerable<BatchCertificateRequest> request)
        {
            var response = new List<BatchCertificateResponse>();

            foreach (var req in request)
            {
                var validationErrors = PerformBasicValidation(req.CertificateData);
                var responseItem = new BatchCertificateResponse
                {
                    ProvidedCertificateData = req.CertificateData,
                    ValidationErrors = validationErrors,
                    Certificate = validationErrors.Count > 0 ? null : new Certificate
                    {
                        CertificateData = req.CertificateData ?? new CertificateData { },
                        Status = new Status { CurrentStatus = "Draft" },
                        Created = new Created { CreatedAt = DateTime.UtcNow.AddHours(-1), CreatedBy = req.Email }
                    }
                };

                if (responseItem.Certificate != null) responseItem.Certificate.CertificateData.CertificateReference = "SANDBOX";
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
                        CertificateData = new CertificateData { },
                        Status = new Status { CurrentStatus = "Submitted" },
                        Created = new Created { CreatedAt = DateTime.UtcNow.AddHours(-1), CreatedBy = req.Email },
                        Submitted = new Submitted { SubmittedAt = DateTime.UtcNow, SubmittedBy = req.Email }
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

        private List<string> PerformBasicValidation(CertificateData cert)
        {
            List<string> validationErrors = new List<string>();

            if(cert.Learner?.Uln < 1000000000 || cert.Learner?.Uln > 9999999999)
            {
                validationErrors.Add("The apprentice's ULN should contain exactly 10 numbers");
            }
            if(string.IsNullOrEmpty(cert.Learner?.FamilyName))
            {
                validationErrors.Add("Enter the apprentice's last name");
            }
            if (cert.Standard?.StandardCode < 0)
            {
                validationErrors.Add("A standard should be selected");
            }

            if (string.IsNullOrEmpty(cert.PostalContact?.ContactName))
            {
                validationErrors.Add("Enter a contact name");
            }
            if (string.IsNullOrEmpty(cert.PostalContact?.Organisation))
            {
                validationErrors.Add("Enter an organisation");
            }
            if (string.IsNullOrEmpty(cert.PostalContact?.AddressLine1))
            {
                validationErrors.Add("Enter an address");
            }
            if (string.IsNullOrEmpty(cert.PostalContact?.City))
            {
                validationErrors.Add("Enter a city or town");
            }
            if (string.IsNullOrEmpty(cert.PostalContact?.PostCode))
            {
                validationErrors.Add("Enter a postcode");
            }

            if (string.IsNullOrEmpty(cert.LearningDetails?.OverallGrade))
            {
                validationErrors.Add("Select the grade the apprentice achieved");
            }

            if (cert.LearningDetails?.AchievementDate is null)
            {
                validationErrors.Add("Enter the achievement date");
            }
            else if (cert.LearningDetails.AchievementDate < new DateTime(2017, 1, 1))
            {
                validationErrors.Add("An achievement date cannot be before 01 01 2017");
            }
            else if (cert.LearningDetails.AchievementDate > DateTime.UtcNow)
            {
                validationErrors.Add("An achievement date cannot be in the future");
            }

            return validationErrors;
        }
    }
}
