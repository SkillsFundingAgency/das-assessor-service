﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Functions.InfrastructureServices;
using SFA.DAS.AssessorService.Functions.Logger;
using SFA.DAS.AssessorService.Functions.Settings;

namespace SFA.DAS.AssessorService.Functions.Data
{
    public class MockCertificatesRepository : ICertificatesRepository
    {
        private readonly IAggregateLogger _aggregateLogger;
        private readonly HttpClient _httpClient;
        private readonly IWebConfiguration _webConfiguration;
        private readonly TokenService _tokenService;

        public MockCertificatesRepository(IAggregateLogger aggregateLogger,
            HttpClient httpClient,
            IWebConfiguration webConfiguration,
            TokenService tokenService)
        {
            _aggregateLogger = aggregateLogger;
            _httpClient = httpClient;
            _webConfiguration = webConfiguration;
            _tokenService = tokenService;
        }

        public async Task<IEnumerable<CertificateResponse>> GetCertificatesToBePrinted()
        {
            var certificates = await CreateDefaultData();

            // Optionally create Dummy Data for Performance Testing
            var generateDummyData = "false";
            return generateDummyData == "true" ? certificates.Union(GenerateDummyData()) : certificates;
        }

        public Task ChangeStatusToPrinted(string batchNumber, IEnumerable<CertificateResponse> responses)
        {
            return Task.CompletedTask;
        }

        public Task<int> GenerateBatchNumber()
        {
            return Task.FromResult(1);
        }

        private static Task<List<CertificateResponse>> CreateDefaultData()
        {
            var certificates = CreateCertificates();
            return Task.FromResult(AddCertificateData(certificates));
        }

        private static List<CertificateResponse> CreateCertificates()
        {
            var certificates = new List<CertificateResponse>();
            for (var sequenceNumber = 1; sequenceNumber <= 8; sequenceNumber++)
            {
                var number = sequenceNumber;
                certificates.Add(
                    Builder<CertificateResponse>.CreateNew().With(q => q.CertificateReference = number.ToString().PadLeft(8, '0'))
                        .Build()
                );
            }

            return certificates;
        }

        private static List<CertificateResponse> AddCertificateData(List<CertificateResponse> certificates)
        {
            certificates[0].CertificateData =
                new CertificateDataResponse
                {
                    AchievementDate = DateTime.Now,
                    LearnerGivenNames = "ZACHARY",
                    LearnerFamilyName = "QUIRK",
                    StandardName = "FIRE EMERGENCY & SECURITY SYSTEMS TECHNICIAN",
                    CourseOption = "SPECIFIC OPTION",
                    StandardLevel = 2,
                    Department = "Human Resources",
                    OverallGrade = "MERIT",
                    ContactName = "Mr James Green",
                    ContactOrganisation = "Employer 1",
                    ContactAddLine1 = "The Qaudrant",
                    ContactAddLine2 = "123 Scotts Road",
                    ContactAddLine3 = "Selly Oak",
                    ContactAddLine4 = "Birmingham",
                    ContactPostCode = "B34 1JK"
                };

            certificates[1].CertificateData =
                new CertificateDataResponse
                {
                    AchievementDate = DateTime.Now,
                    LearnerGivenNames = "ZACHARY",
                    LearnerFamilyName = "QUIRK",
                    StandardName = "FIRE EMERGENCY & SECURITY SYSTEMS TECHNICIAN",
                    CourseOption = "SPECIFIC OPTION",
                    StandardLevel = 2,
                    Department = "Human Resources",
                    OverallGrade = "MERIT",
                    ContactName = "Mr James Green",
                    ContactOrganisation = "Employer 2",
                    ContactAddLine1 = "The Qaudrant",
                    ContactAddLine2 = "123 Scotts Road",
                    ContactAddLine3 = "Selly Oak",
                    ContactAddLine4 = "Birmingham",
                    ContactPostCode = "B34 1JK"
                };


            certificates[2].CertificateData =
                new CertificateDataResponse
                {
                    AchievementDate = DateTime.Now,
                    LearnerGivenNames = "ZACHARY",
                    LearnerFamilyName = "QUIRK",
                    StandardName = "FIRE EMERGENCY & SECURITY SYSTEMS TECHNICIAN",
                    CourseOption = "",
                    StandardLevel = 2,
                    OverallGrade = "MERIT",
                    ContactName = "Mr James Green",
                    ContactOrganisation = "Employer 3",
                    Department = "Human Resources",
                    ContactAddLine1 = "The Qaudrant",
                    ContactAddLine2 = "123 Scotts Road",
                    ContactAddLine3 = "Selly Oak",
                    ContactAddLine4 = "Birmingham",
                    ContactPostCode = "B34 1JK"
                };



            certificates[3].CertificateData =
                new CertificateDataResponse
                {
                    AchievementDate = DateTime.Now,
                    LearnerGivenNames = "ZACHARY",
                    LearnerFamilyName = "QUIRK",
                    StandardName = "FIRE EMERGENCY & SECURITY SYSTEMS TECHNICIAN",
                    CourseOption = "",
                    StandardLevel = 2,
                    OverallGrade = "MERIT",
                    ContactName = "Miss Millicent Martin",
                    ContactOrganisation = "Employer 2",
                    Department = "Human Resources",
                    ContactAddLine1 = "The Qaudrant",
                    ContactAddLine2 = "123 Scotts Road",
                    ContactAddLine3 = "Selly Oak",
                    ContactAddLine4 = "Birmingham",
                    ContactPostCode = "B34 1FG"
                };


            certificates[4].CertificateData =
                new CertificateDataResponse
                {
                    AchievementDate = DateTime.Now,
                    LearnerGivenNames = "ZACHARY",
                    LearnerFamilyName = "QUIRK",
                    StandardName = "FIRE EMERGENCY & SECURITY SYSTEMS TECHNICIAN",
                    CourseOption = "",
                    StandardLevel = 2,
                    OverallGrade = "MERIT",
                    ContactName = "Susan Smith",
                    ContactOrganisation = "Employer 1",
                    Department = "Human Resources",
                    ContactAddLine1 = "The Qaudrant",
                    ContactAddLine2 = "123 Scotts Road",
                    ContactAddLine3 = "Selly Oak",
                    ContactAddLine4 = "Birmingham",
                    ContactPostCode = "B34 1JK"
                };

            certificates[5].CertificateData =
                new CertificateDataResponse
                {
                    AchievementDate = DateTime.Now,
                    LearnerGivenNames = "JANE",
                    LearnerFamilyName = "CLARKE",
                    StandardName = "FIRE EMERGENCY & SECURITY SYSTEMS TECHNICIAN",
                    CourseOption = "SPECIFIC OPTION",
                    StandardLevel = 3,
                    OverallGrade = "DISTINCTION",
                    ContactName = "Alan Bold",
                    ContactOrganisation = "Employer 3",
                    Department = "Human Resources",
                    ContactAddLine1 = "The Qaudrant",
                    ContactAddLine2 = "123 Scotts Road",
                    ContactAddLine3 = "Selly Oak",
                    ContactAddLine4 = "Birmingham",
                    ContactPostCode = "B34 1JK"
                };


            certificates[6].CertificateData =
                new CertificateDataResponse
                {
                    AchievementDate = DateTime.Now,
                    LearnerGivenNames = "JOHN",
                    LearnerFamilyName = "SMITH",
                    StandardName = "FIRE EMERGENCY & SECURITY SYSTEMS TECHNICIAN",
                    CourseOption = "",
                    StandardLevel = 4,
                    OverallGrade = "MERIT",
                    ContactName = "Mr Stephen Long",
                    ContactOrganisation = "Employer 4",
                    Department = "Human Resources",
                    ContactAddLine1 = "The Qaudrant",
                    ContactAddLine2 = "123 Scotts Road",
                    ContactAddLine3 = "Selly Oak",
                    ContactAddLine4 = "Birmingham",
                    ContactPostCode = "B34 1JK"
                };

            certificates[7].CertificateData =
                new CertificateDataResponse
                {
                    AchievementDate = DateTime.Now,
                    LearnerGivenNames = "ANNE",
                    LearnerFamilyName = "JENKINS",
                    StandardName = "FIRE EMERGENCY & SECURITY SYSTEMS TECHNICIAN",
                    CourseOption = "",
                    StandardLevel = 2,
                    OverallGrade = "MERIT",
                    ContactName = "Mary Green Ward",
                    ContactOrganisation = "Employer 5",
                    ContactAddLine1 = "The Qaudrant",
                    ContactAddLine2 = "123 Scotts Road",
                    ContactAddLine3 = "Selly Oak",
                    ContactAddLine4 = "Birmingham",
                    ContactPostCode = "B34 1JK"
                };

            return certificates;
        }

        private IEnumerable<CertificateResponse> GenerateDummyData()
        {
            var size = Convert.ToInt32(10);
            var dummyCertificates = Builder<CertificateResponse>.CreateListOfSize(size)
                .All()
                .With(
                    q => q.CertificateData =
                            Builder<CertificateDataResponse>
                                .CreateNew()
                                .With(x => x.ContactName = "Kim Fucious")
                                .With(x => x.ContactPostCode = "B61 4FE")
                                .Build())
                .Build();

            return dummyCertificates;
        }
    }
}
