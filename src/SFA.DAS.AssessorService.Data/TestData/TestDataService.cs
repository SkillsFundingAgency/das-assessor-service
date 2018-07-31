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
                    EndPointAssessorUkprn = 10033671,
                    Status = OrganisationStatus.New
                };

                context.Organisations.Add(organisation);
                context.SaveChanges();

                var secondOrganisation = new Organisation
                {
                    Id = Guid.NewGuid(),
                    EndPointAssessorName = "EPAO 2",
                    EndPointAssessorOrganisationId = "1235",
                    EndPointAssessorUkprn = 10033672,
                    Status = OrganisationStatus.New
                };

                context.Organisations.Add(secondOrganisation);
                context.SaveChanges();

                var thirdOrganisation = new Organisation
                {
                    Id = Guid.NewGuid(),
                    EndPointAssessorName = "EPAO 3",
                    EndPointAssessorOrganisationId = "1236",
                    EndPointAssessorUkprn = 10033672,
                    Status = OrganisationStatus.New
                };

                context.Organisations.Add(thirdOrganisation);
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

                var thirdContact = new Contact
                {
                    Id = Guid.NewGuid(),
                    Username = "jcoxhead2",
                    Email = "jcoxhead@gmail.com",
                    DisplayName = "John Coxhead",
                    EndPointAssessorOrganisationId = organisation.EndPointAssessorOrganisationId,
                    Status = ContactStatus.Live,
                    OrganisationId = secondOrganisation.Id
                };

                context.Contacts.Add(thirdContact);
                context.SaveChanges();

                for (int i = 0; i <=40; i++)
                {
                    var firstCertificateData = new CertificateData
                    {
                        AchievementDate = DateTime.Now.AddDays(-1),
                        ContactName = "David Gouge",
                        ContactOrganisation = "1234" + i,
                        Department = "Human Resources",
                        ContactAddLine1 = "1 Alpha Drive",
                        ContactAddLine2 = "Oakhalls",
                        ContactAddLine3 = "Malvern",
                        ContactAddLine4 = "Worcs",
                        ContactPostCode = "B60 2TY",
                        CourseOption = "French",
                        LearnerFamilyName = "Gouge",
                        LearnerGivenNames = "David",
                        ProviderName = "Test Provider",
                        OverallGrade = "PASS",

                        Registration = "Registered",
                        LearningStartDate = DateTime.Now.AddDays(10),

                        StandardLevel = 1,
                        StandardName = "Test",
                        StandardPublicationDate = DateTime.Now,
                        FullName = "David Gouge"
                    };

                    var firstCertificate = new Certificate
                    {
                        Id = Guid.NewGuid(),
                        OrganisationId = organisation.Id,
                        CertificateData = JsonConvert.SerializeObject(firstCertificateData),
                        Status = CertificateStatus.Submitted,
                        CreatedBy = "jcoxhead",
                        CertificateReference = "ABC123" + i,
                        Uln = 1234567890 + i,
                        StandardCode = 93,
                        ProviderUkPrn = 12345678
                    };

                    context.Certificates.Add(firstCertificate);

                    var firstCertificateLog = new CertificateLog
                    {
                        Id = Guid.NewGuid(),
                        Action = "Submitted",
                        CertificateId = firstCertificate.Id,
                        CertificateData = JsonConvert.SerializeObject(firstCertificateData),
                        Certificate = firstCertificate,
                        EventTime = DateTime.Now,
                        Status = CertificateStatus.Ready,      
                        Username = "testuser"
                    };

                    context.CertificateLogs.Add(firstCertificateLog);

                    context.SaveChanges();
                }

                var emailTemplate = new EMailTemplate
                {
                    Id = Guid.NewGuid(),
                    Recipients = "john.coxhead@digital.education.gov.uk",
                    TemplateId = "5b171b91-d406-402a-a651-081cce820acb",
                    TemplateName = "PrintAssessorCoverLetters",
                };

                context.EMailTemplates.Add(emailTemplate);
                context.SaveChanges();

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
                        UkPrn = 10022712,
                        StdCode = 90,
                        LearnStartDate = new DateTime(2015, 8, 9),
                        EpaOrgId = "EPA0001",
                        CreatedAt = new DateTime(2018,1,1)
                    },
                    new Ilr()
                    {
                        Uln = 1111111111,
                        GivenNames = "Karla",
                        FamilyName = "Hawkins",
                        UkPrn = 10022712,
                        StdCode = 93,
                        LearnStartDate = new DateTime(2015, 3, 2),
                        EpaOrgId = "EPA0001",
                        CreatedAt = new DateTime(2018,1,1)
                    },
                    new Ilr()
                    {
                        Uln = 2222222222,
                        GivenNames = "Karla",
                        FamilyName = "Hawkins",
                        UkPrn = 10022712,
                        StdCode = 33,
                        LearnStartDate = new DateTime(2015, 8, 9),
                        EpaOrgId = "EPA0001",
                        CreatedAt = new DateTime(2018,1,1)
                    }

                };
                context.Ilrs.AddRange(ilrs);
                context.SaveChanges();
            }
        }
    }
}