using System;
using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.DocumentConversion.Prototype.Data
{
    public class CertificatesRepository
    {
        private readonly IConfiguration _configuration;

        public CertificatesRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<Certificate> GetData()
        {
            var certificates = CreateDefaultData();

            // Optionally create Dummy Data for Performance Testing
            var generateDummyData = _configuration["GenerateDummyData:Generate"];
            return generateDummyData == "true" ? certificates.Union(GenerateDummyData()) : certificates;
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


            certificates[3].CertificateData = JsonConvert.SerializeObject(
                new CertificateData
                {
                    AchievementDate = DateTime.Now,
                    LearnerGivenNames = "ZACHARY",
                    LearnerFamilyName = "QUIRK",
                    StandardName = "FIRE EMERGENCY & SECURITY SYSTEMS TECHNICIAN",
                    CourseOption = "",
                    StandardLevel = 2,
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

            certificates[4].CertificateData = JsonConvert.SerializeObject(
                new CertificateData
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
                    OverallGrade = "DISTINCTION",
                    ContactName = "Alan Bold",
                    ContactOrganisation = "Employer 1",
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
                    OverallGrade = "MERIT",
                    ContactName = "Mr Stephen Long",
                    ContactOrganisation = "Employer 1",
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
            var size = Convert.ToInt32(_configuration["GenerateDummyData:Size"]);
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
