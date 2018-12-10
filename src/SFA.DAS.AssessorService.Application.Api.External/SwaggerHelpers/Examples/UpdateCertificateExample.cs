﻿using SFA.DAS.AssessorService.Application.Api.External.Models.Certificates;
using Swashbuckle.AspNetCore.Examples;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.Api.External.SwaggerHelpers.Examples
{
    public class UpdateCertificateExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new List<UpdateCertificate>
            {
                new UpdateCertificate
                {
                    RequestId = "1",
                    CertificateReference = "09876543",
                    Standard = new Standard { StandardCode = 1 },
                    Learner = new Learner { FamilyName = "Smith", Uln = 1234567890 },
                    LearningDetails = new LearningDetails { CourseOption = "French", OverallGrade = "Pass", AchievementDate = DateTime.UtcNow },
                    PostalContact = new PostalContact { ContactName = "Shreya Smith", Department = "Human Resources", Organisation = "Contoso Ltd", AddressLine1 = "123 Test Road", AddressLine2 = "Green Park", City = "Townsville", PostCode = "ZY9 9ZZ" }
                },
                new UpdateCertificate
                {
                    RequestId = "2",
                    CertificateReference = "99999999",
                    Standard = new Standard { StandardCode = 99 },
                    Learner = new Learner { Uln = 9999999999 },
                    LearningDetails = new LearningDetails { CourseOption = null, OverallGrade = "Merit", AchievementDate = DateTime.UtcNow },
                    PostalContact = new PostalContact { ContactName = "Ken Sanchez", Department = "Human Resources", Organisation = "AdventureWorks Cycles", AddressLine1 = "Silicon Business Park", City = "Bothell", PostCode = "ZY9 9ZZ" }
                }
            };
        }
    }
}
