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
            var learningDetails = new LearningDetails { CourseOption = "French", OverallGrade = CertificateGrade.Pass, AchievementDate = DateTime.UtcNow, LearningStartDate = DateTime.UtcNow.AddYears(-1), ProviderUkPrn = 12345678, ProviderName = "Example Provider" };
            var status = new Status { CurrentStatus = CertificateStatus.Submitted };
            var created = new Created { CreatedAt = DateTime.UtcNow, CreatedBy = "Fred Bloggs" };
            var submitted = new Submitted { SubmittedAt = DateTime.UtcNow, SubmittedBy = "Fred Bloggs" };


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
                            Learner = new Learner { GivenNames = "John", FamilyName = "Smith", Uln = 1234567890 },
                            LearningDetails = learningDetails,
                            PostalContact = new PostalContact { ContactName = "Shreya Smith", Department = "Human Resources", Organisation = "Contoso Ltd", AddressLine1 = "123 Test Road", AddressLine2 = "Green Park", City = "Townsville", PostCode = "ZY9 9ZZ" }
                        },
                        Status = status,
                        Created = created,
                        Submitted = submitted
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
                            Learner = new Learner { GivenNames = "James", FamilyName = "Hamilton", Uln = 9999999999 },
                            LearningDetails = learningDetails,
                            PostalContact = new PostalContact { ContactName = "Ken Sanchez", Department = "Human Resources", Organisation = "AdventureWorks Cycles", AddressLine1 = "Silicon Business Park", City = "Bothell", PostCode = "ZY9 9ZZ" }
                        },
                        Status = status,
                        Created = created,
                        Submitted = submitted
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
                            Learner = new Learner { FamilyName = "Jones", Uln = 6666666666 },
                            LearningDetails = learningDetails,
                            PostalContact = new PostalContact { ContactName = "Ken Sanchez", Department = null, Organisation = null, AddressLine1 = "88 Thinking Road", AddressLine2 = "Green Lane", City = "Brillville", PostCode = "BR9 8YE" }
                        },
                         Status = status,
                        Created = created,
                        Submitted = submitted
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
    }
}
