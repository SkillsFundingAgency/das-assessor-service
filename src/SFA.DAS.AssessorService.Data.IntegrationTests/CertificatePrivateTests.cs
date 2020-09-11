using FluentAssertions;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.IntegrationTests
{
    public class CertificatePrivateTests : TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        
        private AssessorDbContext _context;
        private SqlConnection _databaseConnection;
        
        private CertificateRepository _repository;
        
        private static int _organisationTypeId = 1;
        private static Guid _organisationId = Guid.NewGuid();
        private OrganisationModel _organisation;

        private Certificate _createdCertificate;

        [OneTimeSetUp]
        public async Task SetupCertificateTests()
        {
            var option = new DbContextOptionsBuilder<AssessorDbContext>();

            _databaseConnection = new SqlConnection(_databaseService.WebConfiguration.SqlConnectionString)
            {
                AccessToken = (new AzureServiceTokenProvider()).GetAccessTokenAsync("https://database.windows.net/").Result
            };

            option.UseSqlServer(_databaseConnection, options => options.EnableRetryOnFailure(3));    
            
            _context = new AssessorDbContext(option.Options);

            _repository = new CertificateRepository(_context, _databaseConnection);

            OrganisationTypeHandler.InsertRecord(
                new OrganisationTypeModel 
                { 
                    Id = _organisationTypeId, 
                    Status = "Live",
                    Type = "Organisation Type A" 
                });

            _organisation = new OrganisationModel
            {
                Id = _organisationId,
                CreatedAt = DateTime.Now,
                EndPointAssessorName = "Epao Name 1",
                EndPointAssessorOrganisationId = "EPA0200",
                EndPointAssessorUkprn = 1234567890,
                PrimaryContact = null,
                OrganisationTypeId = _organisationTypeId,
                OrganisationData = null,
                Status = OrganisationStatus.New
            };

            OrganisationHandler.InsertRecords(
                new List<OrganisationModel>()
                {
                    _organisation
                });

            var certData = new CertificateData()
            {
                LearnerGivenNames = "Test",
                LearnerFamilyName = "Person",
                LearningStartDate = DateTime.Now,
                StandardReference = "ST0100",
                StandardName = "Software Tester",
                StandardLevel = 1,
                StandardPublicationDate = DateTime.Now.AddDays(-356),
                FullName = $"Test Person",
                ProviderName = _organisation.EndPointAssessorName,
                EpaDetails = new EpaDetails { Epas = new List<EpaRecord>() }
            };

            var certificate = new Certificate()
            {
                Uln = 123456789,
                StandardCode = 1,
                ProviderUkPrn = _organisation.EndPointAssessorUkprn.Value,
                OrganisationId = _organisation.Id,
                CreatedBy = "Tester",
                CertificateData = JsonConvert.SerializeObject(certData),
                Status = CertificateStatus.Draft,
                CertificateReference = string.Empty,
                LearnRefNumber = "1234567890",
                CreateDay = DateTime.UtcNow.Date,
                IsPrivatelyFunded = true
            };

            _createdCertificate = await _repository.NewPrivate(certificate, "EPA0200");
        }

        [Test]
        public void Then_the_reference_number_is_padded_to_8_characters_with_zeroes()
        {
            _createdCertificate.
                CertificateReference.
                Should().
                Be(_createdCertificate.CertificateReferenceId.ToString().PadLeft(8, '0'));
        }

        [Test]
        public void Then_the_EpaReference_is_updated_with_CertificateReference()
        {
            var returnedCertificateData = JsonConvert.DeserializeObject<CertificateData>(_createdCertificate.CertificateData);
            returnedCertificateData.
                EpaDetails.
                EpaReference.
                Should().
                Be(_createdCertificate.CertificateReferenceId.ToString().PadLeft(8, '0'));
        }

        [OneTimeTearDown]
        public void TearDownCertificateTests()
        {
            CertificateHandler.DeleteRecord(_createdCertificate.Id);
            OrganisationHandler.DeleteRecordByEndPointAssessorOrganisationId("EPA0200");
            OrganisationTypeHandler.DeleteRecord(_organisationTypeId);
        }
    }
}
