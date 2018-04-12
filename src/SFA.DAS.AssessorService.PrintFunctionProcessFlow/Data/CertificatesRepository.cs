using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using FizzWare.NBuilder;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Extensions;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.InfrastructureServices;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Logger;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Settings;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.Data
{
    public class CertificatesRepository
    {
        private readonly IAggregateLogger _aggregateLogger;
        private readonly HttpClient _httpClient;
        private readonly IWebConfiguration _webConfiguration;
        private readonly TokenService _tokenService;

        public CertificatesRepository(IAggregateLogger aggregateLogger,
            HttpClient httpClient,
            IWebConfiguration webConfiguration,
            TokenService tokenService)
        {
            _aggregateLogger = aggregateLogger;
            _httpClient = httpClient;
            _webConfiguration = webConfiguration;
            _tokenService = tokenService;
        }

        public IEnumerable<Certificate> GetData()
        {
            //var certificates = CreateDefaultData();

            //// Optionally create Dummy Data for Performance Testing
            //var generateDummyData = "false";
            //return generateDummyData == "true" ? certificates.Union(GenerateDummyData()) : certificates;

            var response = _httpClient.GetAsync(
                "/api/v1/certificates").Result;

            var certificates = response.Deserialise<List<Certificate>>();
            if (response.IsSuccessStatusCode)
            {
                _aggregateLogger.LogInfo($"Status code returned: {response.StatusCode}. Content: {response.Content.ReadAsStringAsync().Result}");
            }
            else
            {
                _aggregateLogger.LogInfo($"Status code returned: {response.StatusCode}. Content: {response.Content.ReadAsStringAsync().Result}");
            }

            return certificates;
        }

        private static List<Certificate> CreateDefaultData()
        {
            var certificates = CreateCertificates();
            return AddCertificateData(certificates);
        }

        private static List<Certificate> CreateCertificates()
        {
            var certificates = new List<Certificate>();
            for (var sequenceNumber = 1; sequenceNumber <= 8; sequenceNumber++)
            {
                var number = sequenceNumber;
                certificates.Add(
                    Builder<Certificate>.CreateNew().With(q => q.CertificateReference = number.ToString().PadLeft(8, '0'))
                        .Build()
                );
            }

            return certificates;
        }

        private static List<Certificate> AddCertificateData(List<Certificate> certificates)
        {
            certificates[0].CertificateData = JsonConvert.SerializeObject(
                new CertificateData
                {
                    AchievementDate = DateTime.Now,
                    LearnerGivenNames = "ZACHARY",
                    LearnerFamilyName = "QUIRK",
                    StandardName = "FIRE EMERGENCY & SECURITY SYSTEMS TECHNICIAN",
                    CourseOption = "SPECIFIC OPTION",
                    StandardLevel = 2,
                    Department = "Human Resources",
                    AchievementOutcome = "acheiving a",
                    OverallGrade = "MERIT",
                    ContactName = "Mr James Green",
                    ContactOrganisation = "Employer 1",
                    ContactAddLine1 = "The Qaudrant",
                    ContactAddLine2 = "123 Scotts Road",
                    ContactAddLine3 = "Selly Oak",
                    ContactAddLine4 = "Birmingham",
                    ContactPostCode = "B34 1JK"
                }
            );


            certificates[1].CertificateData = JsonConvert.SerializeObject(
                new CertificateData
                {
                    AchievementDate = DateTime.Now,
                    LearnerGivenNames = "ZACHARY",
                    LearnerFamilyName = "QUIRK",
                    StandardName = "FIRE EMERGENCY & SECURITY SYSTEMS TECHNICIAN",
                    CourseOption = "SPECIFIC OPTION",
                    StandardLevel = 2,
                    Department = "Human Resources",
                    AchievementOutcome = "acheiving a",
                    OverallGrade = "MERIT",
                    ContactName = "Mr James Green",
                    ContactOrganisation = "Employer 1",
                    ContactAddLine1 = "The Qaudrant",
                    ContactAddLine2 = "123 Scotts Road",
                    ContactAddLine3 = "Selly Oak",
                    ContactAddLine4 = "Birmingham",
                    ContactPostCode = "B34 1JK"
                }
            );

            certificates[2].CertificateData = JsonConvert.SerializeObject(
                new CertificateData
                {
                    AchievementDate = DateTime.Now,
                    LearnerGivenNames = "ZACHARY",
                    LearnerFamilyName = "QUIRK",
                    StandardName = "FIRE EMERGENCY & SECURITY SYSTEMS TECHNICIAN",
                    CourseOption = "",
                    StandardLevel = 2,
                    AchievementOutcome = "acheiving a",
                    OverallGrade = "MERIT",
                    ContactName = "Mr James Green",
                    ContactOrganisation = "Employer 1",
                    Department = "Human Resources",
                    ContactAddLine1 = "The Qaudrant",
                    ContactAddLine2 = "123 Scotts Road",
                    ContactAddLine3 = "Selly Oak",
                    ContactAddLine4 = "Birmingham",
                    ContactPostCode = "B34 1JK"
                }
            );


            certificates[3].CertificateData = JsonConvert.SerializeObject(
                new CertificateData
                {
                    AchievementDate = DateTime.Now,
                    LearnerGivenNames = "ZACHARY",
                    LearnerFamilyName = "QUIRK",
                    StandardName = "FIRE EMERGENCY & SECURITY SYSTEMS TECHNICIAN",
                    CourseOption = "",
                    StandardLevel = 2,
                    AchievementOutcome = "acheiving a",
                    OverallGrade = "MERIT",
                    ContactName = "Mr James Green",
                    ContactOrganisation = "Employer 1",
                    Department = "Human Resources",
                    ContactAddLine1 = "The Qaudrant",
                    ContactAddLine2 = "123 Scotts Road",
                    ContactAddLine3 = "Selly Oak",
                    ContactAddLine4 = "Birmingham",
                    ContactPostCode = "B34 1JK"
                }
            );

            certificates[4].CertificateData = JsonConvert.SerializeObject(
                new CertificateData
                {
                    AchievementDate = DateTime.Now,
                    LearnerGivenNames = "ZACHARY",
                    LearnerFamilyName = "QUIRK",
                    StandardName = "FIRE EMERGENCY & SECURITY SYSTEMS TECHNICIAN",
                    CourseOption = "",
                    StandardLevel = 2,
                    AchievementOutcome = "acheiving a",
                    OverallGrade = "MERIT",
                    ContactName = "Susan Smith",
                    ContactOrganisation = "Employer 1",
                    Department = "Human Resources",
                    ContactAddLine1 = "The Qaudrant",
                    ContactAddLine2 = "123 Scotts Road",
                    ContactAddLine3 = "Selly Oak",
                    ContactAddLine4 = "Birmingham",
                    ContactPostCode = "B34 1JK"
                }
            );

            certificates[5].CertificateData = JsonConvert.SerializeObject(
                new CertificateData
                {
                    AchievementDate = DateTime.Now,
                    LearnerGivenNames = "JANE",
                    LearnerFamilyName = "CLARKE",
                    StandardName = "FIRE EMERGENCY & SECURITY SYSTEMS TECHNICIAN",
                    CourseOption = "SPECIFIC OPTION",
                    StandardLevel = 3,
                    AchievementOutcome = "acheiving a",
                    OverallGrade = "DISTINCTION",
                    ContactName = "Alan Bold",
                    ContactOrganisation = "Employer 1",
                    Department = "Human Resources",
                    ContactAddLine1 = "The Qaudrant",
                    ContactAddLine2 = "123 Scotts Road",
                    ContactAddLine3 = "Selly Oak",
                    ContactAddLine4 = "Birmingham",
                    ContactPostCode = "B34 1JK"
                }
            );

            certificates[6].CertificateData = JsonConvert.SerializeObject(
                new CertificateData
                {
                    AchievementDate = DateTime.Now,
                    LearnerGivenNames = "JOHN",
                    LearnerFamilyName = "SMITH",
                    StandardName = "FIRE EMERGENCY & SECURITY SYSTEMS TECHNICIAN",
                    CourseOption = "",
                    StandardLevel = 4,
                    AchievementOutcome = "acheiving a",
                    OverallGrade = "MERIT",
                    ContactName = "Mr Stephen Long",
                    ContactOrganisation = "Employer 1",
                    Department = "Human Resources",
                    ContactAddLine1 = "The Qaudrant",
                    ContactAddLine2 = "123 Scotts Road",
                    ContactAddLine3 = "Selly Oak",
                    ContactAddLine4 = "Birmingham",
                    ContactPostCode = "B34 1JK"
                }
            );

            certificates[7].CertificateData = JsonConvert.SerializeObject(
                new CertificateData
                {
                    AchievementDate = DateTime.Now,
                    LearnerGivenNames = "ANNE",
                    LearnerFamilyName = "JENKINS",
                    StandardName = "FIRE EMERGENCY & SECURITY SYSTEMS TECHNICIAN",
                    CourseOption = "",
                    StandardLevel = 2,
                    AchievementOutcome = "acheiving a",
                    OverallGrade = "MERIT",
                    ContactName = "Mary Green Ward",
                    ContactOrganisation = "Employer 1",
                    ContactAddLine1 = "The Qaudrant",
                    ContactAddLine2 = "123 Scotts Road",
                    ContactAddLine3 = "Selly Oak",
                    ContactAddLine4 = "Birmingham",
                    ContactPostCode = "B34 1JK"
                }
            );

            return certificates;
        }

        private IEnumerable<Certificate> GenerateDummyData()
        {
            var size = Convert.ToInt32(10);
            var dummyCertificates = Builder<Certificate>.CreateListOfSize(size)
                .All()
                .With(
                    q => q.CertificateData =
                        JsonConvert.SerializeObject(
                            Builder<SFA.DAS.AssessorService.Domain.JsonData.CertificateData>
                                .CreateNew()
                                .With(x => x.ContactName = "Kim Fucious")
                                .With(x => x.ContactPostCode = "B61 4FE")
                                .Build()
                        ))
                .Build();

            return dummyCertificates;
        }
    }
}
