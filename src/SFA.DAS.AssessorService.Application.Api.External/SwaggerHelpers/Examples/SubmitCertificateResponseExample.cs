﻿using SFA.DAS.AssessorService.Application.Api.External.Models.Response;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Certificates;
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
                            Learner = new Learner { GivenNames = "John", FamilyName = "Smith", Uln = 1234567890 },
                            LearningDetails = new LearningDetails { CourseOption = "French", OverallGrade = "Pass", AchievementDate = DateTime.UtcNow, LearningStartDate = DateTime.UtcNow.AddYears(-1), ProviderUkPrn = 123456, ProviderName = "Example Provider" },
                            PostalContact = new PostalContact { ContactName = "Shreya Smith", Department = "Human Resources", Organisation = "Contoso Ltd", AddressLine1 = "123 Test Road", AddressLine2 = "Green Park", City = "Townsville", PostCode = "ZY9 9ZZ" }
                        },
                        Status = new Status { CurrentStatus = "Submitted" },
                        Created = new Created { CreatedAt = DateTime.UtcNow, CreatedBy = "Fred Bloggs" },
                        Submitted = new Submitted { SubmittedAt = DateTime.UtcNow, SubmittedBy = "Fred Bloggs" }
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
                            LearningDetails = new LearningDetails { CourseOption = null, OverallGrade = "Merit", AchievementDate = DateTime.UtcNow, LearningStartDate = DateTime.UtcNow.AddYears(-1), ProviderUkPrn = 123456, ProviderName = "Example Provider" },
                            PostalContact = new PostalContact { ContactName = "Ken Sanchez", Department = "Human Resources", Organisation = "AdventureWorks Cycles", AddressLine1 = "Silicon Business Park", City = "Bothell", PostCode = "ZY9 9ZZ" }
                        },
                        Status = new Status { CurrentStatus = "Submitted" },
                        Created = new Created { CreatedAt = DateTime.UtcNow, CreatedBy = "Fred Bloggs" },
                        Submitted = new Submitted { SubmittedAt = DateTime.UtcNow, SubmittedBy = "Fred Bloggs" }
                    }
                },
                new SubmitCertificateResponse
                {
                    RequestId = "3",
                    ValidationErrors = new List<string>{ "Cannot find apprentice with the specified Uln, FamilyName & Standard" },
                    Certificate = null
                },
            };
        }
    }
}
