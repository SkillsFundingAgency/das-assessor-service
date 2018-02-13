using System;
using System.Linq;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NLog.Web;
using SFA.DAS.AssessorService.Data;
using SFA.DAS.AssessorService.Data.Entitites;

namespace SFA.DAS.AssessorService.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

            try
            {
                logger.Info("Starting up host");

                var host = BuildWebHost(args);
                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var context = services.GetService<AssessorDbContext>();
                    AddTestData(context);
                }

                host.Run();

            }
            catch (Exception ex)
            {
                //NLog: catch setup errors
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseNLog()
                .Build();


        private static void AddTestData(AssessorDbContext context)
        {
            var existingOrganisation = context.Organisations.FirstOrDefault();
            if (existingOrganisation == null)
            {
                var organisation = new Organisation
                {
                    Id = Guid.NewGuid(),
                    EndPointAssessorName = "David Gouge",
                    EndPointAssessorOrganisationId = "1234",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Status = "Active"
                };

                context.Organisations.Add(organisation);
                context.SaveChanges();

                var firstContact = new Contact
                {
                    Id = Guid.NewGuid(),
                    ContactEmail = "dgouge@gmail.com",
                    ContactName = "David Gouge",
                    EndPointAssessorContactId = 1,
                    EndPointAssessorOrganisationId = "1234",
                    EndPointAssessorUKPRN = 9999,
                    Status = "Active",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    OrganisationId = organisation.Id
                };

                context.Contacts.Add(firstContact);

                var secondContact = new Contact
                {
                    Id = Guid.NewGuid(),
                    ContactEmail = "jcoxhead@gmail.com",
                    ContactName = "John Coxhead",
                    EndPointAssessorContactId = 1,
                    EndPointAssessorOrganisationId = "1234",
                    EndPointAssessorUKPRN = 9999,
                    Status = "Active",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    OrganisationId = organisation.Id
                };

                context.Contacts.Add(secondContact);
                context.SaveChanges();

                var firstCertificate = new Certificate
                {
                    Id = Guid.NewGuid(),
                    AchievementDate = DateTime.Now.AddDays(-1),
                    AchievementOutcome = "Succesfull",
                    ContactName = "David Gouge",
                    ContactOrganisation = "1234",
                    ContactAddLine1 = "1 Alpha Drive",
                    ContactAddLine2 = "Oakhalls",
                    ContactAddLine3 = "Malvern",
                    ContactAddLine4 = "Worcs",
                    ContactPostCode = "B60 2TY",
                    CourseOption = "French",
                    EndPointAssessorCertificateId = 2222222,
                    EndPointAssessorContactId = firstContact.EndPointAssessorContactId,
                    EndPointAssessorOrganisationId = organisation.EndPointAssessorOrganisationId,
                    LearnerDateofBirth = DateTime.Now.AddYears(-22),
                    LearnerFamilyName = "Gouge",
                    LearnerSex = "Male",
                    LearnerGivenNames = "David",
                    OrganisationId = organisation.Id,
                    OverallGrade = "PASS",
                    ProviderUKPRN = 999999,
                    Registration = "Registered",
                    LearningStartDate = DateTime.Now.AddDays(10),
                    StandardCode = 100,
                    StandardLevel = 1,
                    StandardName = "Test",
                    StandardPublicationDate = DateTime.Now,
                    Status = "Active",
                    ULN = 123456,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedBy = firstContact.Id,
                    UpdatedBY = firstContact.Id

                };

                context.Certificates.Add(firstCertificate);
                var secondCertificate = new Certificate
                {
                    Id = Guid.NewGuid(),
                    AchievementDate = DateTime.Now.AddDays(-1),
                    AchievementOutcome = "Succesfull",
                    ContactName = "John Coxhead",
                    ContactOrganisation = "1234",
                    ContactAddLine1 = "1 Beta Drive",
                    ContactAddLine2 = "Oakhalls",
                    ContactAddLine3 = "Malvern",
                    ContactAddLine4 = "Worcs",
                    ContactPostCode = "B60 2TY",
                    CourseOption = "French",
                    EndPointAssessorCertificateId = 2222222,
                    EndPointAssessorContactId = firstContact.EndPointAssessorContactId,
                    EndPointAssessorOrganisationId = organisation.EndPointAssessorOrganisationId,
                    LearnerDateofBirth = DateTime.Now.AddYears(-22),
                    LearnerFamilyName = "Coxhead",
                    LearnerSex = "Male",
                    LearnerGivenNames = "David",
                    OrganisationId = organisation.Id,
                    OverallGrade = "PASS",
                    ProviderUKPRN = 999999,
                    Registration = "Registered",
                    LearningStartDate = DateTime.Now.AddDays(10),
                    StandardCode = 100,
                    StandardLevel = 1,
                    StandardName = "Test",
                    StandardPublicationDate = DateTime.Now,
                    Status = "Active",
                    ULN = 123456,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    CreatedBy = firstContact.Id,
                    UpdatedBY = firstContact.Id
                };
               
                context.Certificates.Add(secondCertificate);
                context.SaveChanges();

                var firstCertificateLog = new CertificateLog
                {
                    Id = Guid.NewGuid(),
                    Action = "Action",
                    CertificateId = firstCertificate.Id,
                    EndPointAssessorCertificateId = 2222222,
                    EventTime = DateTime.Now,
                    Status = "Active",
                    
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                context.CertificateLogs.Add(firstCertificateLog);

                var secondCertificateLog = new CertificateLog
                {
                    Id = Guid.NewGuid(),
                    Action = "Action",
                    CertificateId = secondCertificate.Id,
                    EndPointAssessorCertificateId = 2222222,
                    EventTime = DateTime.Now,
                    Status = "Active",

                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                context.CertificateLogs.Add(secondCertificateLog);
                context.SaveChanges();
            }
        }
    }
}
