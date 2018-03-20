﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.DocumentConversion.Prototype.Data
{
    public class CertificateData
    {
        public static IEnumerable<Certificate> GetData()
        {
            yield return new Certificate
            {
                Id = Guid.NewGuid(),
                OrganisationId = Guid.NewGuid(),
                CertificateData = JsonConvert.SerializeObject(
                        new Domain.JsonData.CertificateData
                        {
                            AchievementDate = DateTime.Now.AddDays(-1),
                            AchievementOutcome = "Successful",
                            ContactName = "David Gouge",
                            ContactOrganisation = "1234",
                            ContactAddLine1 = "1 Trianon Drive",
                            ContactAddLine2 = "Oakhalls",
                            ContactAddLine3 = "Malvern",
                            ContactAddLine4 = "Worcs",
                            ContactPostCode = "B60 2TY",
                            CourseOption = "French",
                            LearnerDateofBirth = DateTime.Now.AddYears(-22),
                            LearnerFamilyName = "Gouge",
                            LearnerSex = "Male",
                            LearnerGivenNames = "David",
                            OverallGrade = "PASS",
                            Registration = "Registered",
                            LearningStartDate = DateTime.Now.AddDays(10),
                            StandardLevel = 1,
                            StandardName = "Test",
                            StandardPublicationDate = DateTime.Now
                        }
                    ),
                Status = CertificateStatus.Ready,
                CreatedBy = "jcoxhead",
                CertificateReference = "ABC123",
                Uln = 1234567890,
                StandardCode = 93,
                ProviderUkPrn = 12345678
            };
            yield return new Certificate
            {
                Id = Guid.NewGuid(),
                OrganisationId = Guid.NewGuid(),
                CertificateData = JsonConvert.SerializeObject(
                    new Domain.JsonData.CertificateData
                    {
                        AchievementDate = DateTime.Now.AddDays(-1),
                        AchievementOutcome = "Succesful",
                        ContactName = "John Coxhead",
                        ContactOrganisation = "1234",
                        ContactAddLine1 = "1 July Road",
                        ContactAddLine2 = "Rednall",
                        ContactAddLine3 = "Malvern",
                        ContactAddLine4 = "Worcs",
                        ContactPostCode = "B60 2TY",
                        CourseOption = "French",
                        LearnerDateofBirth = DateTime.Now.AddYears(-22),
                        LearnerFamilyName = "Coxhead",
                        LearnerSex = "Male",
                        LearnerGivenNames = "David",
                        OverallGrade = "PASS",
                        Registration = "Registered",
                        LearningStartDate = DateTime.Now.AddDays(10),
                        StandardLevel = 1,
                        StandardName = "Test",
                        StandardPublicationDate = DateTime.Now,
                    }
                ),
                Status = CertificateStatus.Ready,
                CreatedBy = "jcoxhead",
                CertificateReference = "DEF456",
                Uln = 1234567890,
                StandardCode = 94,
                ProviderUkPrn = 12345678
            };
        }
    }
}
