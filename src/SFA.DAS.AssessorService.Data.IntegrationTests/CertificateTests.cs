﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Data.IntegrationTests
{
    public class CertificateTests : TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        
        private CertificateRepository _repository;
        
        private static int _organisationTypeId = 20;
        private static Guid _organisationId = Guid.NewGuid();

        private OrganisationModel _organisation;

        private Certificate _createdCertificate;

        [OneTimeSetUp]
        public async Task SetupCertificateTests()
        {
            _repository = new CertificateRepository(new AssessorUnitOfWork(_databaseService.TestContext));

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
                CertificateData = certData,
                Status = CertificateStatus.Draft,
                CertificateReference = string.Empty,
                LearnRefNumber = "1234567890",
                CreateDay = DateTime.UtcNow.Date
            };

            _createdCertificate = await _repository.NewStandardCertificate(certificate);
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
            _createdCertificate.CertificateData.
                EpaDetails.
                EpaReference.
                Should().
                Be(_createdCertificate.CertificateReferenceId.ToString().PadLeft(8, '0'));
        }

        [OneTimeTearDown]
        public void TearDownCertificateTests()
        {
            CertificateLogsHandler.DeleteRecord(_createdCertificate.Id);
            CertificateHandler.DeleteRecord(_createdCertificate.Id);
            OrganisationHandler.DeleteRecordByEndPointAssessorOrganisationId("EPA0200");
            OrganisationTypeHandler.DeleteRecord(_organisationTypeId);
        }
    }
}
