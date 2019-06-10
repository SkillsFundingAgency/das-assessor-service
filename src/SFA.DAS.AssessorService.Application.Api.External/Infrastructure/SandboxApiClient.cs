using AutoMapper;
using SFA.DAS.AssessorService.Application.Api.External.Models.Internal;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Certificates;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Standards;
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

        public Task<GetCertificateResponse> GetCertificate(GetBatchCertificateRequest request)
        {
            var validationErrors = PerformBasicGetCertificateValidation(request);

            if (!int.TryParse(request.Standard, out int standardCode))
            {
                standardCode = 9999;
            }

            var response = new GetCertificateResponse
            {
                ValidationErrors = validationErrors,
                Certificate = validationErrors.Count > 0 ? null : new Certificate
                {
                    CertificateData = new CertificateData
                    {
                        CertificateReference = "SANDBOX",
                        Learner = new Learner { FamilyName = request.FamilyName, GivenNames = "FIRSTNAME", Uln = request.Uln },
                        LearningDetails = new LearningDetails { CourseOption = "COURSEOPTION", OverallGrade = "Pass", AchievementDate = DateTime.UtcNow, LearningStartDate = DateTime.UtcNow.AddYears(-1), ProviderName = "PROVIDER", ProviderUkPrn = request.UkPrn },
                        Standard = new Standard { Level = 1, StandardCode = standardCode, StandardReference = request.Standard, StandardName = "STANDARD" },
                        PostalContact = new PostalContact { AddressLine1 = "ADDRESS1", City = "CITY", ContactName = "CONTACT", Organisation = "ORGANISATION", PostCode = "AB1 1AA" }
                    },
                    Status = new Status { CurrentStatus = "Draft" },
                    Created = new Created { CreatedAt = DateTime.UtcNow, CreatedBy = request.Email },
                }
            };

            return Task.FromResult(response);
        }

        public Task<IEnumerable<CreateCertificateResponse>> CreateCertificates(IEnumerable<CreateBatchCertificateRequest> request)
        {
            var response = new List<CreateCertificateResponse>();

            foreach (var req in request)
            {
                var validationErrors = PerformBasicCreateBatchCertificateRequestValidation(req);

                var responseItem = new CreateCertificateResponse
                {
                    RequestId = req.RequestId,
                    ValidationErrors = validationErrors,
                    Certificate = validationErrors.Count > 0 ? null : new Certificate
                    {
                        CertificateData = Mapper.Map<Models.Request.Certificates.CertificateData, CertificateData>(req.CertificateData),
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

                    if (responseItem.Certificate.CertificateData.Standard.StandardCode is null)
                    {
                        responseItem.Certificate.CertificateData.Standard.StandardCode = 9999;
                    }
                    if (string.IsNullOrEmpty(responseItem.Certificate.CertificateData.Standard.StandardReference))
                    {
                        responseItem.Certificate.CertificateData.Standard.StandardReference = "ST9999";
                    }
                }

                response.Add(responseItem);
            }

            return Task.FromResult(response.AsEnumerable());
        }

        public Task<IEnumerable<UpdateCertificateResponse>> UpdateCertificates(IEnumerable<UpdateBatchCertificateRequest> request)
        {
            var response = new List<UpdateCertificateResponse>();

            foreach (var req in request)
            {
                var validationErrors = PerformBasicUpdateBatchCertificateRequestValidation(req);

                if (req.CertificateData != null && string.IsNullOrEmpty(req.CertificateData.CertificateReference))
                {
                    validationErrors.Add("Enter the certificate reference");
                }

                var responseItem = new UpdateCertificateResponse
                {
                    RequestId = req.RequestId,
                    ValidationErrors = validationErrors,
                    Certificate = validationErrors.Count > 0 ? null : new Certificate
                    {
                        CertificateData = Mapper.Map<Models.Request.Certificates.CertificateData, CertificateData>(req.CertificateData),
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

                    if (responseItem.Certificate.CertificateData.Standard.StandardCode is null)
                    {
                        responseItem.Certificate.CertificateData.Standard.StandardCode = 9999;
                    }
                    if (string.IsNullOrEmpty(responseItem.Certificate.CertificateData.Standard.StandardReference))
                    {
                        responseItem.Certificate.CertificateData.Standard.StandardReference = "ST9999";
                    }
                }

                response.Add(responseItem);
            }

            return Task.FromResult(response.AsEnumerable());
        }

        public Task<IEnumerable<SubmitCertificateResponse>> SubmitCertificates(IEnumerable<SubmitBatchCertificateRequest> request)
        {
            var response = new List<SubmitCertificateResponse>();

            foreach (var req in request)
            {
                var validationErrors = PerformBasicSubmitCertificateValidation(req);

                var responseItem = new SubmitCertificateResponse
                {
                    RequestId = req.RequestId,
                    ValidationErrors = validationErrors,
                    Certificate = validationErrors.Count > 0 ? null : new Certificate
                    {
                        CertificateData = new CertificateData
                        {
                            CertificateReference = "SANDBOX",
                            Learner = new Learner { FamilyName = req.FamilyName, GivenNames = "FIRSTNAME", Uln = req.Uln },
                            LearningDetails = new LearningDetails { CourseOption = "COURSEOPTION", OverallGrade = "Pass", AchievementDate = DateTime.UtcNow, LearningStartDate = DateTime.UtcNow.AddYears(-1), ProviderName = "PROVIDER", ProviderUkPrn = req.UkPrn },
                            Standard = new Standard { Level = 1, StandardCode = req.StandardCode ?? 9999, StandardReference = req.StandardReference ?? "ST9999", StandardName = "STANDARD" },
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

        public Task<ApiResponse> DeleteCertificate(DeleteBatchCertificateRequest request)
        {
            var validationErrors = PerformBasicDeleteCertificateValidation(request);

            var response = validationErrors.Count == 0 ? null : new ApiResponse((int)HttpStatusCode.Forbidden, string.Join("; ", validationErrors));

            return Task.FromResult(response);
        }

        private List<string> PerformBasicGetCertificateValidation(GetBatchCertificateRequest request)
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
                if (string.IsNullOrEmpty(request.Standard))
                {
                    validationErrors.Add("A standard should be selected");
                }
                else if (int.TryParse(request.Standard, out int standardCode) && standardCode < 1)
                {
                    validationErrors.Add("A standard should be selected");
                }
            }

            return validationErrors;
        }

        private List<string> PerformBasicCreateBatchCertificateRequestValidation(CreateBatchCertificateRequest request)
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

                if (cert.Standard?.StandardCode is null && string.IsNullOrEmpty(cert.Standard?.StandardReference))
                {
                    validationErrors.Add("A standard should be selected");
                }
                else if (cert.Standard?.StandardCode != null && cert.Standard.StandardCode < 1)
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

        private List<string> PerformBasicUpdateBatchCertificateRequestValidation(UpdateBatchCertificateRequest request)
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

                if (cert.Standard?.StandardCode is null && string.IsNullOrEmpty(cert.Standard?.StandardReference))
                {
                    validationErrors.Add("A standard should be selected");
                }
                else if (cert.Standard?.StandardCode != null && cert.Standard.StandardCode < 1)
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
                if (request.StandardCode is null && string.IsNullOrEmpty(request.StandardReference))
                {
                    validationErrors.Add("A standard should be selected");
                }
                else if (request.StandardCode.HasValue && request.StandardCode < 1)
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

        private List<string> PerformBasicDeleteCertificateValidation(DeleteBatchCertificateRequest request)
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
                if (string.IsNullOrEmpty(request.Standard))
                {
                    validationErrors.Add("A standard should be selected");
                }
                else if (int.TryParse(request.Standard, out int standardCode) && standardCode < 1)
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


        private readonly List<StandardOptions> standards = new List<StandardOptions> {
                new StandardOptions { StandardCode = 6, StandardReference = "ST0156", CourseOption = new[] { "Overhead lines", "Substation fitting", "Underground cables" } },
                new StandardOptions { StandardCode = 7, StandardReference = "ST0184", CourseOption = new[] { "Card services", "Corporate/Commercial", "Retail", "Wealth", } },
                new StandardOptions { StandardCode = 314, StandardReference = "ST0018", CourseOption = new[] { "Container based system", "Soil based system" } } };


        public async Task<IEnumerable<StandardOptions>> GetStandards()
        {
            return await Task.FromResult(standards);
        }

        public async Task<StandardOptions> GetStandard(string standard)
        {
            StandardOptions standardOption = null;

            if (int.TryParse(standard, out int standardId))
            {
                standardOption = standards.Where(s => s.StandardCode == standardId).FirstOrDefault();
            }

            if (standardOption is null)
            {
                standardOption = standards.Where(s => s.StandardReference == standard).FirstOrDefault();
            }

            return await Task.FromResult(standardOption);
        }
    }
}
