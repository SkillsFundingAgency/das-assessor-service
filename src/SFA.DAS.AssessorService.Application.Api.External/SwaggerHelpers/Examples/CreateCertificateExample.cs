using SFA.DAS.AssessorService.Application.Api.External.Models.Request;
using SFA.DAS.AssessorService.Application.Api.External.Models.Request.Certificates;
using SFA.DAS.AssessorService.Domain.Consts;
using Swashbuckle.AspNetCore.Examples;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Examples
{
    public class CreateCertificateExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<CreateCertificateRequest>
            {
                new CreateCertificateRequest
                {
                    RequestId = "1",
                    Standard = new Standard { StandardCode = 1 },
                    Learner = new Learner { FamilyName = "Smith", Uln = 1234567890 },
                    LearningDetails = new LearningDetails { CourseOption = "French", OverallGrade = CertificateGrade.Pass, AchievementDate = DateTime.UtcNow },
                    PostalContact = new PostalContact { ContactName = "Shreya Smith", Department = "Human Resources", Organisation = "Contoso Ltd", AddressLine1 = "123 Test Road", AddressLine2 = "Green Park", City = "Townsville", PostCode = "ZY9 9ZZ" }
                },
                new CreateCertificateRequest
                {
                    RequestId = "2",
                    Standard = new Standard { StandardReference = "ST0099" },
                    Learner = new Learner { FamilyName = "Hamilton", Uln = 9999999999 },
                    LearningDetails = new LearningDetails { CourseOption = null, OverallGrade = CertificateGrade.Merit, AchievementDate = DateTime.UtcNow },
                    PostalContact = new PostalContact { ContactName = "Ken Sanchez", Department = "Human Resources", Organisation = "AdventureWorks Cycles", AddressLine1 = "Silicon Business Park", City = "Bothell", PostCode = "ZY9 9ZZ" }
                },
                new CreateCertificateRequest
                {
                    RequestId = "3",
                    Standard = new Standard { StandardCode = 555, StandardReference = "ST0555" },
                    Learner = new Learner { FamilyName = "Unknown", Uln = 5555555555 },
                    LearningDetails = new LearningDetails { CourseOption = null, OverallGrade = CertificateGrade.Credit, AchievementDate = DateTime.UtcNow },
                    PostalContact = new PostalContact { ContactName = "Alan Brewer", Department = "Human Resources", Organisation = "Fabrikam Inc", AddressLine1 = "Outlook Place", City = "Lorem Ipsum", PostCode = "ZY9 9ZZ" }
                },
                new CreateCertificateRequest
                {
                    RequestId = "4",
                    Standard = new Standard { StandardReference = "ST0287" },
                    Learner = new Learner { FamilyName = "Jones", Uln = 6666666666 },
                    LearningDetails = new LearningDetails { CourseOption = "Mechanics", OverallGrade = CertificateGrade.Pass, AchievementDate = DateTime.UtcNow },
                    PostalContact = new PostalContact { ContactName = "Shreya Smith", Department = null, Organisation = null, AddressLine1 = "88 Thinking Road", AddressLine2 = "Green Lane", City = "Brillville", PostCode = "BR9 8YE" }
                }
            };
        }
    }
}
