using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Certificates;
using SFA.DAS.AssessorService.Domain.Consts;
using Swashbuckle.AspNetCore.Examples;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Examples
{
    public class SubmitCertificateResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<SubmitCertificateResponse>
            {
                new SubmitCertificateResponse
                {
                    RequestId = "1",
                    ValidationErrors = new List<string>(),
                    Certificate = new Certificate
                    {
                        CertificateData = new CertificateData
                        {
                            CertificateReference = "09876543",
                            Standard = new Standard { StandardCode = 1, StandardReference = "ST0001", Level = 1, StandardName = "Example Standard" },
                            Learner = GetLearner("John", "Smith"),
                            LearningDetails = new LearningDetails { CourseOption = "French", Version="1.0", OverallGrade = CertificateGrade.Pass, AchievementDate = DateTime.UtcNow, LearningStartDate = DateTime.UtcNow.AddYears(-1), ProviderUkPrn = 12345678, ProviderName = "Example Provider" },
                            PostalContact = new PostalContact { ContactName = "Shreya Smith", Department = "Human Resources", Organisation = "Contoso Ltd", AddressLine1 = "123 Test Road", AddressLine2 = "Green Park", City = "Townsville", PostCode = "ZY9 9ZZ" }
                        },
                        Status = GetStatus(),
                        Created = GetCreated(DateTime.UtcNow),
                        Submitted = GetSubmitted(DateTime.UtcNow)
                    }
                },
                new SubmitCertificateResponse
                {
                    RequestId = "2",
                    ValidationErrors = new List<string>(),
                    Certificate = new Certificate
                    {
                        CertificateData = new CertificateData
                        {
                            CertificateReference = "99999999",
                            Standard = new Standard { StandardCode = 99, StandardReference = "ST0099", Level = 1, StandardName = "Example Standard" },
                            Learner = GetLearner("John", "Doe", 0123456789),
                            LearningDetails = new LearningDetails { CourseOption = null, Version="1.0", OverallGrade = CertificateGrade.Merit, AchievementDate = DateTime.UtcNow, LearningStartDate = DateTime.UtcNow.AddYears(-1), ProviderUkPrn = 12345678, ProviderName = "Example Provider" },
                            PostalContact = new PostalContact { ContactName = "Ken Sanchez", Department = "Human Resources", Organisation = "AdventureWorks Cycles", AddressLine1 = "Silicon Business Park", City = "Bothell", PostCode = "ZY9 9ZZ" }
                        },
                        Status = GetStatus(),
                        Created = GetCreated(DateTime.UtcNow),
                        Submitted = GetSubmitted(DateTime.UtcNow)
                    }
                },
                new SubmitCertificateResponse
                {
                    RequestId = "3",
                    ValidationErrors = new List<string>(),
                    Certificate = new Certificate
                    {
                        CertificateData = new CertificateData
                        {
                            CertificateReference = "333333333",
                            Standard = new Standard { StandardCode = 2, StandardReference = "ST0287", Level = 1, StandardName = "Other Example Standard" },
                            Learner = GetLearner("Smith", "Doe", 0123456785),
                            LearningDetails = new LearningDetails { CourseOption = "Mechanics", Version="1.0", OverallGrade = CertificateGrade.Pass, AchievementDate = DateTime.UtcNow, LearningStartDate = DateTime.UtcNow.AddYears(-1), ProviderUkPrn = 12345678, ProviderName = "Example Provider" },
                            PostalContact = new PostalContact { ContactName = "Ken Sanchez", Department = null, Organisation = null, AddressLine1 = "88 Thinking Road", AddressLine2 = "Green Lane", City = "Brillville", PostCode = "BR9 8YE" }
                        },
                        Status = GetStatus(),
                        Created = GetCreated(DateTime.UtcNow),
                        Submitted = GetSubmitted(DateTime.UtcNow)
                    }
                },
                new SubmitCertificateResponse
                {
                    RequestId = "4",
                    ValidationErrors = new List<string>{ "ULN, FamilyName and Standard not found" },
                    Certificate = null
                },
            };
        }

        private static Submitted GetSubmitted(DateTime submittedAt, string submittedBy = "Fred Bloggs")
        {
            return new Submitted { SubmittedAt = submittedAt, SubmittedBy = submittedBy };
        }

        private static Created GetCreated(DateTime createdAt, string createdBy = "Fred Bloggs")
        {
            return new Created { CreatedAt = createdAt, CreatedBy = createdBy };
        }

        private static Status GetStatus(string currentStatus = CertificateStatus.Submitted)
        {
            return new Status { CurrentStatus = currentStatus };
        }

        private static Learner GetLearner(string giveName = "John", string familyName = "Smith", long uln = 1234567890)
        {
            return new Learner { GivenNames = giveName, FamilyName = familyName, Uln = uln };
        }
    }
}
