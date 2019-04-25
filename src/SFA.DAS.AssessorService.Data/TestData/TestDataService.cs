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
            List<Organisation> organisations = new List<Organisation>();

            var existingOrganisation = context.Organisations.FirstOrDefault();
            if (existingOrganisation == null)
            {
                var firstOrganisation = new Organisation
                {
                    Id = Guid.NewGuid(),
                    EndPointAssessorName = "BCS, The Chartered Institute for IT",
                    EndPointAssessorOrganisationId = "EPA000011",
                    EndPointAssessorUkprn = 10022719,
                    Status = OrganisationStatus.New
                };

                organisations.Add(firstOrganisation);

                context.Organisations.Add(firstOrganisation);
                context.SaveChanges();

                var secondOrganisation = new Organisation
                {
                    Id = Guid.NewGuid(),
                    EndPointAssessorName = "BCS, The Chartered Institute for IT 1",
                    EndPointAssessorOrganisationId = "EPA000013",
                    EndPointAssessorUkprn = 10033672,
                    Status = OrganisationStatus.New
                };

                organisations.Add(secondOrganisation);
                context.Organisations.Add(secondOrganisation);
                context.SaveChanges();

                var thirdOrganisation = new Organisation
                {
                    Id = Guid.NewGuid(),
                    EndPointAssessorName = "BCS, The Chartered Institute for IT 2",
                    EndPointAssessorOrganisationId = "EPA000014",
                    EndPointAssessorUkprn = 1003367,
                    Status = OrganisationStatus.New
                };

                context.Organisations.Add(thirdOrganisation);
                context.SaveChanges();

                organisations.Add(thirdOrganisation);

                var firstContact = new Contact
                {
                    Id = Guid.NewGuid(),
                    Username = "fredjones",
                    Email = "blah@blah.com",
                    DisplayName = "Fred Jones",
                    EndPointAssessorOrganisationId = firstOrganisation.EndPointAssessorOrganisationId,
                    Status = ContactStatus.Live,
                    OrganisationId = firstOrganisation.Id
                };

                context.Contacts.Add(firstContact);

                var secondContact = new Contact
                {
                    Id = Guid.NewGuid(),
                    Username = "jcoxhead",
                    Email = "jcoxhead@gmail.com",
                    DisplayName = "John Coxhead",
                    EndPointAssessorOrganisationId = secondOrganisation.EndPointAssessorOrganisationId,
                    Status = ContactStatus.Live,
                    OrganisationId = secondOrganisation.Id
                };

                context.Contacts.Add(secondContact);
                context.SaveChanges();

                var thirdContact = new Contact
                {
                    Id = Guid.NewGuid(),
                    Username = "jcoxhead2",
                    Email = "jcoxhead@gmail.com",
                    DisplayName = "John Coxhead",
                    EndPointAssessorOrganisationId = thirdOrganisation.EndPointAssessorOrganisationId,
                    Status = ContactStatus.Live,
                    OrganisationId = thirdOrganisation.Id
                };

                context.Contacts.Add(thirdContact);
                context.SaveChanges();

                // create 30 certificates which will not have any duplicate standard codes
                for (int i = 0; i <=30; i++)
                {
                    var certificateData = new CertificateData
                    {
                        AchievementDate = DateTime.Now.AddDays(-1),
                        ContactName = "John Coxhead",
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

                    var certificate = new Certificate
                    {
                        Id = Guid.NewGuid(),
                        OrganisationId = firstOrganisation.Id,
                        CertificateData = JsonConvert.SerializeObject(certificateData),
                        Status = CertificateStatus.Submitted,
                        CreatedBy = "jcoxhead",
                        CertificateReference = $"104123{i:00}",
                        Uln = 1111111111,
                        ProviderUkPrn = 12345678
                    };

                    switch (i)
                    {
                        case 0:
                            certificate.StandardCode = 93;
                            break;
                        case 1:
                            certificate.StandardCode = 90;
                            break;
                        case 2:
                            certificate.StandardCode = 33;
                            break;
                        case 3:
                            certificate.StandardCode = 44;
                            break;
                        case 4:
                            certificate.StandardCode = 45;
                            break;
                        case 5:
                            certificate.StandardCode = 46;
                            break;
                        case 6:
                            certificate.StandardCode = 47;
                            break;
                        case 7:
                            certificate.StandardCode = 48;
                            break;
                        case 8:
                            certificate.StandardCode = 49;
                            break;
                        case 9:
                            certificate.StandardCode = 50;
                            break;
                        case 10:
                            certificate.StandardCode = 51;
                            break;
                        case 11:
                            certificate.StandardCode = 52;
                            break;
                        case 12:
                            certificate.StandardCode = 53;
                            break;
                        default:
                            certificate.StandardCode = i;
                            break;
                    }

                    context.Certificates.Add(certificate);

                    var certificateLog = new CertificateLog
                    {
                        Id = Guid.NewGuid(),
                        Action = "Submitted",
                        CertificateId = certificate.Id,
                        CertificateData = JsonConvert.SerializeObject(certificateData),
                        Certificate = certificate,
                        EventTime = DateTime.Now,
                        Status = CertificateStatus.Ready,
                        Username = "testuser"
                    };

                    context.CertificateLogs.Add(certificateLog);

                    context.SaveChanges();

                    var ilr = new Ilr()
                    {
                        Uln = certificate.Uln,
                        GivenNames = "Karla",
                        FamilyName = "Hawkins",
                        UkPrn = firstOrganisation.EndPointAssessorUkprn.Value,
                        StdCode = certificate.StandardCode,
                        LearnStartDate = new DateTime(2015, 8, 9),
                        EpaOrgId = firstOrganisation.EndPointAssessorOrganisationId,
                        CreatedAt = new DateTime(2018, 1, 1)
                    };

                    context.Ilrs.Add(ilr);
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
        }
    }
}