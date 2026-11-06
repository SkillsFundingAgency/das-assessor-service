using SFA.DAS.AssessorService.Application.Api.External.Models.Request;
using SFA.DAS.AssessorService.Application.Api.External.Models.Request.Certificates;
using SFA.DAS.AssessorService.Domain.Consts;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Examples
{
    public class UpdateCertificateExample : IExamplesProvider<List<UpdateCertificateRequest>>
    {
        public List<UpdateCertificateRequest> GetExamples()
        {
            return new List<UpdateCertificateRequest>
            {
                new UpdateCertificateRequest
                {
                    RequestId = "1",
                    CertificateReference = "09876543",
                    Standard = new Standard { StandardCode = 1 },
                    Learner = new Learner { FamilyName = "Smith", Uln = 1234567890 },
                    LearningDetails = new LearningDetails { CourseOption = "French", Version="1.0", OverallGrade = CertificateGrade.Pass, AchievementDate = DateTime.UtcNow }
                },
                new UpdateCertificateRequest
                {
                    RequestId = "2",
                    CertificateReference = "99999999",
                    Standard = new Standard { StandardReference = "ST0099" },
                    Learner = new Learner { FamilyName = "Hamilton", Uln = 9999999999 },
                    LearningDetails = new LearningDetails { CourseOption = null, Version = "1.0", OverallGrade = CertificateGrade.Merit, AchievementDate = DateTime.UtcNow }
                },
                new UpdateCertificateRequest
                {
                    RequestId = "3",
                    CertificateReference = "55555555",
                    Standard = new Standard { StandardCode = 555, StandardReference = "ST0555" },
                    Learner = new Learner { FamilyName = "Unknown", Uln = 5555555555 },
                    LearningDetails = new LearningDetails { CourseOption = null, Version="1.0", OverallGrade = CertificateGrade.Credit, AchievementDate = DateTime.UtcNow }
                },
                new UpdateCertificateRequest
                {
                    RequestId = "4",
                    CertificateReference = "333333333",
                    Standard = new Standard { StandardReference = "ST0287" },
                    Learner = new Learner { FamilyName = "Jones", Uln = 6666666666 },
                    LearningDetails = new LearningDetails { CourseOption = "Mechanics", Version="1.0", OverallGrade = CertificateGrade.Pass, AchievementDate = DateTime.UtcNow }
                }
            };
        }
    }
}
