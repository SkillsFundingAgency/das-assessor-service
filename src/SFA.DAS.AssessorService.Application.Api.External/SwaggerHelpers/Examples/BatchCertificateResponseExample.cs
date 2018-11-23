using SFA.DAS.AssessorService.Application.Api.External.Messages;
using SFA.DAS.AssessorService.Application.Api.External.Models.Certificates;
using Swashbuckle.AspNetCore.Examples;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Examples
{
    public class BatchCertificateResponseExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new BatchCertificateResponse
            {
                RequestId = "1",
                ValidationErrors = new List<string>(),
                Certificate = new Certificate
                {
                    CertificateData = new CertificateData
                    {
                        CertificateReference = "09876543",
                        Standard = new Standard { StandardCode = 1, Level = 1, StandardName = "Example Standard" },
                        Learner = new Learner { GivenNames = "John", FamilyName = "Smith", Uln = 1234567890 },
                        LearningDetails = new LearningDetails { CourseOption = "French", OverallGrade = "Pass", AchievementDate = DateTime.Today, LearningStartDate = DateTime.Today.AddYears(-1), ProviderUkPrn = 123456, ProviderName = "Example Provider" },
                        PostalContact = new PostalContact { ContactName = "Shreya Smith", Department = "Human Resources", Organisation = "Contoso Ltd", AddressLine1 = "123 Test Road", AddressLine2 = "Green Park", City = "Townsville", PostCode = "ZY9 9ZZ" }
                    },
                    Status = new Status { CurrentStatus = "Draft" },
                    Created = new Created { CreatedAt = DateTime.UtcNow, CreatedBy = "Fred Bloggs" }
                }
            };
        }
    }
}
