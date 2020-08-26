using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Certificates;
using SFA.DAS.AssessorService.Domain.Consts;
using Swashbuckle.AspNetCore.Examples;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Examples
{
    public class GetCertificateExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<Certificate>
            {
                new Certificate
                {
                    CertificateData = new CertificateData
                    {
                        CertificateReference = "09876543",
                        Standard = new Standard { StandardCode = 1, StandardReference = "ST0001", Level = 1, StandardName = "Example Standard" },
                        Learner = new Learner { GivenNames = "John", FamilyName = "Smith", Uln = 1234567890 },
                        LearningDetails = new LearningDetails { CourseOption = "French", OverallGrade = CertificateGrade.Pass, AchievementDate = DateTime.UtcNow, LearningStartDate = DateTime.UtcNow.AddYears(-1), ProviderUkPrn = 12345678, ProviderName = "Example Provider" },
                        PostalContact = new PostalContact { ContactName = "Shreya Smith", Department = "Human Resources", Organisation = "Contoso Ltd", AddressLine1 = "123 Test Road", AddressLine2 = "Green Park", City = "Townsville", PostCode = "ZY9 9ZZ" }
                    },
                    Status = new Status { CurrentStatus = CertificateStatus.Submitted },
                    Created = new Created { CreatedAt = DateTime.UtcNow, CreatedBy = "Fred Bloggs" },
                    Submitted = new Submitted { SubmittedAt = DateTime.UtcNow, SubmittedBy = "Fred Bloggs" }
                },
                new Certificate
                {
                    CertificateData = new CertificateData
                    {
                        CertificateReference = "333333333",
                        Standard = new Standard { StandardCode = 2, StandardReference = "ST0287", Level = 1, StandardName = "Other Example Standard" },
                        Learner = new Learner { FamilyName = "Jones", Uln = 6666666666 },
                        LearningDetails = new LearningDetails { CourseOption = "Mechanics", OverallGrade = CertificateGrade.Pass, AchievementDate = DateTime.UtcNow, LearningStartDate = DateTime.UtcNow.AddYears(-1), ProviderUkPrn = 12345678, ProviderName = "Example Provider" },
                        PostalContact = new PostalContact { ContactName = "Shreya Smith", Department = null, Organisation = null, AddressLine1 = "88 Thinking Road", AddressLine2 = "Green Lane", City = "Brillville", PostCode = "BR9 8YE" }
                    },
                    Status = new Status { CurrentStatus = CertificateStatus.Submitted },
                    Created = new Created { CreatedAt = DateTime.UtcNow, CreatedBy = "Fred Bloggs" },
                    Submitted = new Submitted { SubmittedAt = DateTime.UtcNow, SubmittedBy = "Fred Bloggs" }
                }
            };
        }
    }
}
