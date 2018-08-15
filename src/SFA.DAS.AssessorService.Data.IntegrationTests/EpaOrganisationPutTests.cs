using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Data.IntegrationTests
{
    public class EpaOrganisationPutTests : TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        private RegisterRepository _repository;
        private string _organisationIdCreated;
        private int _ukprnBefore;
        private EpaOrganisation _initialOrganisationDetails;
        private int _organisationTypeIdBefore;
        private int _ukprnAfter;
        private int _organisationTypeIdAfter;
        private EpaOrganisation _expectedOrganisationDetails;
        private DateTime _updatedAt;
        private string _nameUpdated;
        private OrganisationData _updatedOrgData;
        private DateTime _createdAt;

        [OneTimeSetUp]
        public void SetUpOrganisationTests()
        {
            _repository = new RegisterRepository(_databaseService.WebConfiguration);
            _organisationIdCreated = "EPA987";
            _ukprnBefore = 123321;
            _ukprnAfter = 124421;
            _organisationTypeIdBefore = 5;
            _organisationTypeIdAfter = 6;
            _updatedAt = DateTime.Today.Date.AddHours(9);
            OrganisationTypeHandler.InsertRecord(new OrganisationTypeModel { Id = _organisationTypeIdBefore, Status = "new", Type = "organisation type 1" });
            OrganisationTypeHandler.InsertRecord(new OrganisationTypeModel { Id = _organisationTypeIdAfter, Status = "new", Type = "organisation type 2" });
            _nameUpdated = "name 2";
            _createdAt = DateTime.Today.Date.AddHours(8);

            _initialOrganisationDetails = new EpaOrganisation
            {
                Id = Guid.NewGuid(),
                CreatedAt = _createdAt,
                Name = "name 1",
                OrganisationId = _organisationIdCreated,
                Ukprn = _ukprnBefore,
                PrimaryContact = null,
                Status = OrganisationStatus.New,
                OrganisationTypeId = _organisationTypeIdBefore,
                OrganisationData = new OrganisationData
                {
                    LegalName = " legal name",
                    Address1 = "address 1",
                    Address2 = "address 2",
                    Address3 = "address 3",
                    Address4 = "address 4",
                    Postcode = "postcode"
                }
            };

            _updatedOrgData = new OrganisationData
            {
                LegalName = " legal name 2",
                Address1 = "address 1b",
                Address2 = "address 2b",
                Address3 = "address 3b",
                Address4 = "address 4b",
                Postcode = "postcodeb"
            };

            _expectedOrganisationDetails = new EpaOrganisation
            {
                Id = _initialOrganisationDetails.Id,
                CreatedAt = _createdAt,
                Name = _nameUpdated,
                OrganisationId = _organisationIdCreated,
                Ukprn = _ukprnAfter,
                PrimaryContact = null,
                Status = OrganisationStatus.New,
                UpdatedAt = _updatedAt,
                OrganisationTypeId = _organisationTypeIdAfter,
                OrganisationData = _updatedOrgData
            };
        }

        [Test]
        public void CreateOrganisationThatDoesntExistAndCheckItIsThere()
        {
            var initialOrganisationDetails = _repository.CreateEpaOrganisation(_initialOrganisationDetails).Result;
            initialOrganisationDetails.Name = _nameUpdated;
            initialOrganisationDetails.Ukprn = _ukprnAfter;
            initialOrganisationDetails.UpdatedAt = _updatedAt;
            initialOrganisationDetails.OrganisationTypeId = _organisationTypeIdAfter;
            initialOrganisationDetails.OrganisationData = _updatedOrgData;
            var updatedOrganisationDetails = _repository.UpdateEpaOrganisation(initialOrganisationDetails).Result;
            _expectedOrganisationDetails.Should().BeEquivalentTo(updatedOrganisationDetails);
        }



        [OneTimeTearDown]
        public void TearDownOrganisationTests()
        {
            OrganisationHandler.DeleteRecordByOrganisationId(_organisationIdCreated);
            OrganisationTypeHandler.DeleteRecord(_organisationTypeIdBefore);
            OrganisationTypeHandler.DeleteRecord(_organisationTypeIdAfter);
        }
    }
}
