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
                    Status = OrganisationStatus.New,
                    OrganisationTypeId = 1
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
                    Status = OrganisationStatus.New,
                    OrganisationTypeId = 1
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
                    Status = OrganisationStatus.New,
                    OrganisationTypeId = 1
                };

                context.Organisations.Add(thirdOrganisation);
                context.SaveChanges();

                organisations.Add(thirdOrganisation);

                var firstContact = new Contact
                {
                    Id = Guid.NewGuid(),
                    Username = "user1",
                    Email = "davegouge+atuser1@gmail.com",
                    DisplayName = "User One",
                    EndPointAssessorOrganisationId = firstOrganisation.EndPointAssessorOrganisationId,
                    Status = ContactStatus.Live,
                    OrganisationId = firstOrganisation.Id,
                    Title = "Mr",
                    FamilyName = "One",
                    GivenNames = "User",
                    SignInType = "ASLogin",
                    SignInId = Guid.Parse("089b2f10-5280-4a46-b23e-fa940c06d35d")
                };

                context.Contacts.Add(firstContact);

                var secondContact = new Contact
                {
                    Id = Guid.NewGuid(),
                    Username = "user2",
                    Email = "davegouge+atuser2@gmail.com",
                    DisplayName = "User Two",
                    EndPointAssessorOrganisationId = secondOrganisation.EndPointAssessorOrganisationId,
                    Status = ContactStatus.Live,
                    OrganisationId = secondOrganisation.Id,
                    Title = "Mrs",
                    FamilyName = "Two",
                    GivenNames = "User",
                    SignInType = "ASLogin",
                    SignInId = Guid.Parse("d4360834-d84b-4181-bd7a-a03cc581d02c")
                };

                context.Contacts.Add(secondContact);

                var thirdContact = new Contact
                {
                    Id = Guid.NewGuid(),
                    Username = "user3",
                    Email = "davegouge+atuser3@gmail.com",
                    DisplayName = "User Three",
                    EndPointAssessorOrganisationId = thirdOrganisation.EndPointAssessorOrganisationId,
                    Status = ContactStatus.Live,
                    OrganisationId = thirdOrganisation.Id,
                    Title = "Miss",
                    FamilyName = "Three",
                    GivenNames = "User",
                    SignInType = "ASLogin",
                    SignInId = Guid.Parse("f205e897-0e0b-40fb-9175-8a9a44cd4ff6")
                };

                context.Contacts.Add(thirdContact);
                context.SaveChanges();

                
                context.ContactRoles.AddRange(
                    new ContactRole(){ContactId = firstContact.Id, Id = Guid.NewGuid(), RoleName = "SuperUser"}, 
                    new ContactRole(){ContactId = secondContact.Id, Id = Guid.NewGuid(), RoleName = "SuperUser"},  
                    new ContactRole(){ContactId = thirdContact.Id, Id = Guid.NewGuid(), RoleName = "SuperUser"});
                
                
                // create 30 certificates which will not have any duplicate standard codes
                for (int i = 0; i <=30; i++)
                {
                    var certificateData = new CertificateData
                    {
                        AchievementDate = DateTime.Now.AddDays(-1),
                        ContactName = "Contact One",
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
                        CreatedBy = "user2",
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
                    Recipients = "alan.burns@digital.education.gov.uk",
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