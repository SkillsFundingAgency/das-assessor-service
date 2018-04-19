using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Data.TestData
{
    using System;
    using System.Linq;
    using Domain.Consts;
    using Domain.Entities;
    using Domain.JsonData;
    using Newtonsoft.Json;

    public static class TestDataService
    {
        public static void AddTestData(AssessorDbContext context)
        {
            var existingOrganisation = context.Organisations.FirstOrDefault();
            if (existingOrganisation == null)
            {
                var organisation = new Organisation
                {
                    Id = Guid.NewGuid(),
                    EndPointAssessorName = "EPAO 1",
                    EndPointAssessorOrganisationId = "1234",
                    EndPointAssessorUkprn = 10033670,
                    Status = OrganisationStatus.New
                };

                context.Organisations.Add(organisation);
                context.SaveChanges();

                var firstContact = new Contact
                {
                    Id = Guid.NewGuid(),
                    Username = "fredjones",
                    Email = "blah@blah.com",
                    DisplayName = "Fred Jones",
                    EndPointAssessorOrganisationId = organisation.EndPointAssessorOrganisationId,
                    Status = ContactStatus.Live,
                    OrganisationId = organisation.Id
                };

                context.Contacts.Add(firstContact);

                var secondContact = new Contact
                {
                    Id = Guid.NewGuid(),
                    Username = "jcoxhead",
                    Email = "jcoxhead@gmail.com",
                    DisplayName = "John Coxhead",
                    EndPointAssessorOrganisationId = organisation.EndPointAssessorOrganisationId,
                    Status = ContactStatus.Live,
                    OrganisationId = organisation.Id
                };

                context.Contacts.Add(secondContact);
                context.SaveChanges();

                var firstCertificateData = new CertificateData
                {
                    AchievementDate = DateTime.Now.AddDays(-1),
                    ContactName = "David Gouge",
                    ContactOrganisation = "1234",
                    Department = "Human Resources",
                    ContactAddLine1 = "1 Alpha Drive",
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
                };

                var firstCertificate = new Certificate
                {
                    Id = Guid.NewGuid(),
                    OrganisationId = organisation.Id,
                    CertificateData = JsonConvert.SerializeObject(firstCertificateData),
                    Status = CertificateStatus.Submitted,
                    CreatedBy = "jcoxhead",
                    CertificateReference = "ABC123",
                    Uln = 1234567890,
                    StandardCode = 93,
                    ProviderUkPrn = 12345678
                };

                context.Certificates.Add(firstCertificate);

                var secondCertificateData = new CertificateData
                {
                    AchievementDate = DateTime.Now.AddDays(-1),
                    ContactName = "John Coxhead",
                    ContactOrganisation = "1234",
                    Department = "Human Resources",
                    ContactAddLine1 = "1 Beta Drive",
                    ContactAddLine2 = "Oakhalls",
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
                };

                var secondCertificate = new Certificate
                {
                    Id = Guid.NewGuid(),
                    OrganisationId = organisation.Id,
                    CertificateData = JsonConvert.SerializeObject(secondCertificateData),
                    Status = CertificateStatus.Submitted,
                    CreatedBy = "jcoxhead",
                    CertificateReference = "DEF456",
                    Uln = 1234567890,
                    StandardCode = 94,
                    ProviderUkPrn = 12345678
                };

                context.Certificates.Add(secondCertificate);
                context.SaveChanges();

                var firstCertificateLog = new CertificateLog
                {
                    Id = Guid.NewGuid(),
                    Action = "Action",
                    CertificateId = firstCertificate.Id,

                    EventTime = DateTime.Now,
                    Status = CertificateStatus.Ready,

                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                context.CertificateLogs.Add(firstCertificateLog);

                var secondCertificateLog = new CertificateLog
                {
                    Id = Guid.NewGuid(),
                    Action = "Action",
                    CertificateId = secondCertificate.Id,

                    EventTime = DateTime.Now,
                    Status = CertificateStatus.Ready,

                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                context.CertificateLogs.Add(secondCertificateLog);
                context.SaveChanges();


                var emailTemplate = new EMailTemplate
                {
                    Id = Guid.NewGuid(),
                    Recipients = "john.coxhead@digital.education.gov.uk",
                    TemplateId = "5b171b91-d406-402a-a651-081cce820acb",
                    TemplateName = "PrintAssessorCoverLetters",
                };

                context.EMailTemplates.Add(emailTemplate);
                context.SaveChanges();

            }

            var existingIlrData = context.Ilrs.FirstOrDefault();
            if (existingIlrData == null)
            {
                var ilrs = new List<Ilr>
                {
                    new Ilr()
                    {
                        Uln = 1111111111,
                        GivenNames = "Karla",
                        FamilyName = "Hawkins",
                        DateOfBirth = new DateTime(2002, 4, 15),
                        Sex = "Female",
                        UkPrn = 10009931,
                        StdCode = "90",
                        LearnStartDate = new DateTime(2015, 8, 9),
                        EpaOrgId = "EPA000011"
                    },
                    new Ilr()
                    {
                        Uln = 1111111111,
                        GivenNames = "Karla",
                        FamilyName = "Hawkins",
                        DateOfBirth = new DateTime(2002, 4, 15),
                        Sex = "Female",
                        UkPrn = 10009931,
                        StdCode = "93",
                        LearnStartDate = new DateTime(2015, 3, 2),
                        EpaOrgId = "EPA000011"
                    },
                    new Ilr()
                    {
                        Uln = 2222222222,
                        GivenNames = "Karla",
                        FamilyName = "Hawkins",
                        DateOfBirth = new DateTime(2002, 4, 15),
                        Sex = "Male",
                        UkPrn = 10009931,
                        StdCode = "33",
                        LearnStartDate = new DateTime(2015, 8, 9),
                        EpaOrgId = "EPA000011"
                    }

                };
                context.Ilrs.AddRange(ilrs);
                context.SaveChanges();
            }
        }
    }
}