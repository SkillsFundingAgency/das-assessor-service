using SFA.DAS.AssessorService.Application.Api.External.Messages;
using SFA.DAS.AssessorService.Application.Api.External.Middleware;
using SFA.DAS.AssessorService.Application.Api.External.Models.Certificates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            var validationErrors = PerformBasicGetCertificateValidation(request);

            var response = new GetCertificateResponse
            {
                ValidationErrors = validationErrors,
                Certificate = validationErrors.Count > 0 ? null : new Certificate
                {
                    CertificateData = new CertificateData
                    {
                        CertificateReference = "SANDBOX",
                        Learner = new Learner { FamilyName = request.FamilyName, GivenNames = "FIRSTNAME", Uln = request.Uln },
                        LearningDetails = new LearningDetails { OverallGrade = "Pass", AchievementDate = DateTime.UtcNow, ProviderName = "PROVIDER", ProviderUkPrn = request.UkPrn },
                        Standard = new Standard { Level = 1, StandardCode = request.StandardCode, StandardName = "STANDARD" },
                        PostalContact = new PostalContact { AddressLine1 = "ADDRESS1", City = "CITY", ContactName = "CONTACT", Organisation = "ORGANISATION", PostCode = "AB1 1AA" }
                    },
                    Status = new Status { CurrentStatus = "Draft" },
                    Created = new Created { CreatedAt = DateTime.UtcNow, CreatedBy = request.Email },
                },
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
                var validationErrors = PerformBasicBatchCertificateRequestValidation(req);

                var responseItem = new BatchCertificateResponse
                {
                    RequestId = req.RequestId,
                    ValidationErrors = validationErrors,
                    Certificate = validationErrors.Count > 0 ? null : new Certificate
                    {
                        CertificateData = req.CertificateData ?? new CertificateData { },
                        Status = new Status { CurrentStatus = "Draft" },
                        Created = new Created { CreatedAt = DateTime.UtcNow, CreatedBy = req.Email },
                    }
                };

                if (responseItem.Certificate != null)
                {
                    responseItem.Certificate.CertificateData.CertificateReference = "SANDBOX";
                    responseItem.Certificate.CertificateData.Learner.GivenNames = "FIRSTNAME";
                    responseItem.Certificate.CertificateData.Standard.Level = 1;
                    responseItem.Certificate.CertificateData.Standard.StandardName = "STANDARD";
                    responseItem.Certificate.CertificateData.LearningDetails.ProviderName = "PROVIDER";
                    responseItem.Certificate.CertificateData.LearningDetails.ProviderUkPrn = req.UkPrn;
                    responseItem.Certificate.CertificateData.LearningDetails.LearningStartDate = DateTime.UtcNow.AddYears(-1);
                }

                response.Add(responseItem);
            }

            return Task.FromResult(response.AsEnumerable());
        }

        public Task<IEnumerable<BatchCertificateResponse>> UpdateCertificates(IEnumerable<BatchCertificateRequest> request)
        {
            var response = new List<BatchCertificateResponse>();

            foreach (var req in request)
            {
                var validationErrors = PerformBasicBatchCertificateRequestValidation(req);

                if (req.CertificateData != null && string.IsNullOrEmpty(req.CertificateData.CertificateReference))
                {
                    validationErrors.Add("Enter the certificate reference");
                }

                var responseItem = new BatchCertificateResponse
                {
                    RequestId = req.RequestId,
                    ValidationErrors = validationErrors,
                    Certificate = validationErrors.Count > 0 ? null : new Certificate
                    {
                        CertificateData = req.CertificateData ?? new CertificateData { },
                        Status = new Status { CurrentStatus = "Draft" },
                        Created = new Created { CreatedAt = DateTime.UtcNow.AddHours(-1), CreatedBy = req.Email }
                    }
                };

                if (responseItem.Certificate != null)
                {
                    responseItem.Certificate.CertificateData.CertificateReference = "SANDBOX";
                    responseItem.Certificate.CertificateData.Learner.GivenNames = "FIRSTNAME";
                    responseItem.Certificate.CertificateData.Standard.Level = 1;
                    responseItem.Certificate.CertificateData.Standard.StandardName = "STANDARD";
                    responseItem.Certificate.CertificateData.LearningDetails.ProviderName = "PROVIDER";
                    responseItem.Certificate.CertificateData.LearningDetails.ProviderUkPrn = req.UkPrn;
                    responseItem.Certificate.CertificateData.LearningDetails.LearningStartDate = DateTime.UtcNow.AddYears(-1);
                }

                response.Add(responseItem);
            }

            return Task.FromResult(response.AsEnumerable());
        }

        public Task<IEnumerable<SubmitBatchCertificateResponse>> SubmitCertificates(IEnumerable<SubmitBatchCertificateRequest> request)
        {
            var response = new List<SubmitBatchCertificateResponse>();

            foreach (var req in request)
            {
                var validationErrors = PerformBasicSubmitCertificateValidation(req);

                var responseItem = new SubmitBatchCertificateResponse
                {
                    RequestId = req.RequestId,
                    ValidationErrors = validationErrors,
                    Certificate = validationErrors.Count > 0 ? null : new Certificate
                    {
                        CertificateData = new CertificateData
                        {
                            CertificateReference = "SANDBOX",
                            Learner = new Learner { FamilyName = req.FamilyName, GivenNames = "FIRSTNAME", Uln = req.Uln },
                            LearningDetails = new LearningDetails { OverallGrade = "Pass", AchievementDate = DateTime.UtcNow, LearningStartDate = DateTime.UtcNow.AddYears(-1), ProviderName = "PROVIDER", ProviderUkPrn = req.UkPrn },
                            Standard = new Standard { Level = 1, StandardCode = req.StandardCode, StandardName = "STANDARD" },
                            PostalContact = new PostalContact { AddressLine1 = "ADDRESS1", City = "CITY", ContactName = "CONTACT", Organisation = "ORGANISATION", PostCode = "AB1 1AA" }
                        },
                        Status = new Status { CurrentStatus = "Submitted" },
                        Created = new Created { CreatedAt = DateTime.UtcNow.AddHours(-1), CreatedBy = req.Email },
                        Submitted = new Submitted { SubmittedAt = DateTime.UtcNow, SubmittedBy = req.Email }
                    }
                };

                response.Add(responseItem);
            }

            return Task.FromResult(response.AsEnumerable());
        }

        public Task<ApiResponse> DeleteCertificate(DeleteCertificateRequest request)
        {
            var validationErrors = PerformBasicDeleteCertificateValidation(request);

            var response = validationErrors.Count == 0 ? null : new ApiResponse((int)HttpStatusCode.Forbidden, string.Join("; ", validationErrors));

            return Task.FromResult(response);
        }

        private List<string> PerformBasicGetCertificateValidation(GetCertificateRequest request)
        {
            List<string> validationErrors = new List<string>();

            if (request is null)
            {
                validationErrors.Add("Enter Certificate Data");
            }
            else
            {
                if (request.Uln < 1000000000 || request.Uln > 9999999999)
                {
                    validationErrors.Add("The apprentice's ULN should contain exactly 10 numbers");
                }
                if (string.IsNullOrEmpty(request.FamilyName))
                {
                    validationErrors.Add("Enter the apprentice's last name");
                }
                if (request.StandardCode < 1)
                {
                    validationErrors.Add("A standard should be selected");
                }
            }

            return validationErrors;
        }

        private List<string> PerformBasicBatchCertificateRequestValidation(BatchCertificateRequest request)
        {
            List<string> validationErrors = new List<string>();

            if (request?.CertificateData is null)
            {
                validationErrors.Add("Enter Certificate Data");
            }
            else
            {
                var cert = request.CertificateData;

                if (cert.Learner?.Uln < 1000000000 || cert.Learner?.Uln > 9999999999)
                {
                    validationErrors.Add("The apprentice's ULN should contain exactly 10 numbers");
                }
                if (string.IsNullOrEmpty(cert.Learner?.FamilyName))
                {
                    validationErrors.Add("Enter the apprentice's last name");
                }
                if (cert.Standard?.StandardCode < 1)
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
            }

            return validationErrors;
        }

        private List<string> PerformBasicSubmitCertificateValidation(SubmitBatchCertificateRequest request)
        {
            List<string> validationErrors = new List<string>();

            if (request is null)
            {
                validationErrors.Add("Enter Certificate Data");
            }
            else
            {
                if (request.Uln < 1000000000 || request.Uln > 9999999999)
                {
                    validationErrors.Add("The apprentice's ULN should contain exactly 10 numbers");
                }
                if (string.IsNullOrEmpty(request.FamilyName))
                {
                    validationErrors.Add("Enter the apprentice's last name");
                }
                if (request.StandardCode < 1)
                {
                    validationErrors.Add("A standard should be selected");
                }
                if (string.IsNullOrEmpty(request.CertificateReference))
                {
                    validationErrors.Add("Enter the certificate reference");
                }
            }

            return validationErrors;
        }

        private List<string> PerformBasicDeleteCertificateValidation(DeleteCertificateRequest request)
        {
            List<string> validationErrors = new List<string>();

            if (request is null)
            {
                validationErrors.Add("Enter Certificate Data");
            }
            else
            {
                if (request.Uln < 1000000000 || request.Uln > 9999999999)
                {
                    validationErrors.Add("The apprentice's ULN should contain exactly 10 numbers");
                }
                if (string.IsNullOrEmpty(request.FamilyName))
                {
                    validationErrors.Add("Enter the apprentice's last name");
                }
                if (request.StandardCode < 1)
                {
                    validationErrors.Add("A standard should be selected");
                }
                if (string.IsNullOrEmpty(request.CertificateReference))
                {
                    validationErrors.Add("Enter the certificate reference");
                }
            }

            return validationErrors;
        }
    }
}
