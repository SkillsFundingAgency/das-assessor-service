using SFA.DAS.AssessorService.Application.Api.External.Models.Certificates;
using Swashbuckle.AspNetCore.Examples;
using System;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Examples
{
    public class CertificateExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new Certificate
            {
                CertificateData = new CertificateData
                {
                    CertificateReference = "09876543",
                    Standard = new Standard { StandardCode = 1, Level = 1, StandardName = "Example Standard" },
                    Learner = new Learner { GivenNames = "John", FamilyName = "Smith", Uln = 1234567890 },
                    LearningDetails = new LearningDetails { CourseOption = "French", OverallGrade = "Pass", AchievementDate = DateTime.UtcNow, LearningStartDate = DateTime.UtcNow.AddYears(-1), ProviderUkPrn = 123456, ProviderName = "Example Provider" },
                    PostalContact = new PostalContact { ContactName = "Shreya Smith", Department = "Human Resources", Organisation = "Contoso Ltd", AddressLine1 = "123 Test Road", AddressLine2 = "Green Park", City = "Townsville", PostCode = "ZY9 9ZZ" }
                },
                Status = new Status { CurrentStatus = "Submitted" },
                Created = new Created { CreatedAt = DateTime.UtcNow, CreatedBy = "Fred Bloggs" },
                Submitted = new Submitted { SubmittedAt = DateTime.UtcNow, SubmittedBy = "Fred Bloggs" }
            };
        }
    }
}
