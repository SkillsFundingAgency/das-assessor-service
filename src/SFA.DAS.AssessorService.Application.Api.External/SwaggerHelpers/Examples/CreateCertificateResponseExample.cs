using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Certificates;
using SFA.DAS.AssessorService.Domain.Consts;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Examples
{
    public class CreateCertificateResponseExample : IExamplesProvider<List<CreateCertificateResponse>>
    {
        public List<CreateCertificateResponse> GetExamples()
        {
            return new List<CreateCertificateResponse>
            {
                new CreateCertificateResponse
                {
                    RequestId = "1",
                    ValidationErrors = new List<string>(),
                    Certificate = new Certificate
                    {
                        CertificateData = new CertificateData
                        {
                            CertificateReference = "09876543",
                            Standard = new Standard { StandardCode = 1, StandardReference = "ST0001", Level = 1, StandardName = "Example Standard" },
                            Learner = new Learner { GivenNames = "John", FamilyName = "Smith", Uln = 1234567890 },
                            LearningDetails = new LearningDetails { CourseOption = "French", Version="1.0", OverallGrade = CertificateGrade.Pass, AchievementDate = DateTime.UtcNow, LearningStartDate = DateTime.UtcNow.AddYears(-1), ProviderUkPrn = 12345678, ProviderName = "Example Provider" }
                        },
                        Status = new Status { CurrentStatus = CertificateStatus.Draft },
                        Created = new Created { CreatedAt = DateTime.UtcNow, CreatedBy = "Fred Bloggs" }
                    }
                },
                new CreateCertificateResponse
                {
                    RequestId = "2",
                    ValidationErrors = new List<string>(),
                    Certificate = new Certificate
                    {
                        CertificateData = new CertificateData
                        {
                            CertificateReference = "99999999",
                            Standard = new Standard { StandardCode = 99, StandardReference = "ST0099", Level = 1, StandardName = "Example Standard" },
                            Learner = new Learner { GivenNames = "James", FamilyName = "Hamilton", Uln = 9999999999 },
                            LearningDetails = new LearningDetails { CourseOption = null, Version="1.0", OverallGrade = CertificateGrade.Merit, AchievementDate = DateTime.UtcNow, LearningStartDate = DateTime.UtcNow.AddYears(-1), ProviderUkPrn = 12345678, ProviderName = "Example Provider" }
                        },
                        Status = new Status { CurrentStatus = CertificateStatus.Draft },
                        Created = new Created { CreatedAt = DateTime.UtcNow, CreatedBy = "Fred Bloggs" }
                    }
                },
                new CreateCertificateResponse
                {
                    RequestId = "3",
                    ValidationErrors = new List<string>(),
                    Certificate = new Certificate
                    {
                        CertificateData = new CertificateData
                        {
                            CertificateReference = "333333333",
                            Standard = new Standard { StandardReference = "ST0287" },
                            Learner = new Learner { FamilyName = "Jones", Uln = 6666666666 },
                            LearningDetails = new LearningDetails { CourseOption = "Mechanics", Version="1.0", OverallGrade = CertificateGrade.Pass, AchievementDate = DateTime.UtcNow }
                        },
                        Status = new Status { CurrentStatus = CertificateStatus.Draft },
                        Created = new Created { CreatedAt = DateTime.UtcNow, CreatedBy = "Fred Bloggs" }
                    }
                },
                new CreateCertificateResponse
                {
                    RequestId = "4",
                    ValidationErrors = new List<string>{ "ULN, FamilyName and Standard not found" },
                    Certificate = null
                }
            };
        }
    }
}