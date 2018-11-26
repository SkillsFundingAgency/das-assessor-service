using SFA.DAS.AssessorService.Application.Api.External.Models.Certificates;
using Swashbuckle.AspNetCore.Examples;
using System;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Examples
{
    public class UpdateCertificateExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new UpdateCertificate
            {
                RequestId = "1",
                CertificateReference = "09876543",
                Standard = new Standard { StandardCode = 1 },
                Learner = new Learner { FamilyName = "Smith", Uln = 1234567890 },
                LearningDetails = new LearningDetails { CourseOption = "French", OverallGrade = "Pass", AchievementDate = DateTime.Today },
                PostalContact = new PostalContact { ContactName = "Shreya Smith", Department = "Human Resources", Organisation = "Contoso Ltd", AddressLine1 = "123 Test Road", AddressLine2 = "Green Park", City = "Townsville", PostCode = "ZY9 9ZZ" }
            };
        }
    }
}
