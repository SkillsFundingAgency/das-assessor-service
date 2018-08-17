﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using SFA.DAS.AssessorService.Domain.Consts;

namespace SFA.DAS.AssessorService.Data.IntegrationTests
{
    public class EpaOrganisationPostTests: TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        private  RegisterRepository _repository;
        private RegisterQueryRepository _queryRepository;
        private string _organisationIdCreated;
        private int _ukprnCreated;
        private EpaOrganisation _organisation;
        private int _organisationTypeId;
        private Guid _id;

        [OneTimeSetUp]
        public void SetUpOrganisationTests()
        {
            _repository = new RegisterRepository(_databaseService.WebConfiguration);
            _queryRepository = new RegisterQueryRepository(_databaseService.WebConfiguration);
            _organisationIdCreated = "EPA987";
            _ukprnCreated = 123321;
            _organisationTypeId = 5;
            OrganisationTypeHandler.InsertRecord(new OrganisationTypeModel { Id = _organisationTypeId, Status = "new", Type = "organisation type 1" });
            _id = Guid.NewGuid();

            _organisation = new EpaOrganisation
            {
                Id = _id,
                CreatedAt = DateTime.Now,
                Name = "name 1",
                OrganisationId = _organisationIdCreated,
                Ukprn = _ukprnCreated,
                PrimaryContact = null,
                Status = OrganisationStatus.New,
                OrganisationTypeId = _organisationTypeId,
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
        }

        [Test]
        public void CreateOrganisationThatDoesntExistAndCheckItIsThere()
        {
            var isOrgByOrgIdPresentBeforeInsert = _queryRepository.EpaOrganisationExistsWithOrganisationId(_organisationIdCreated).Result;
            var isOrgByUkprnPresentBeforeInsert = _queryRepository.EpaOrganisationExistsWithUkprn(_ukprnCreated).Result;
            var returnedOrganisationId = _repository.CreateEpaOrganisation(_organisation).Result;
            var isOrgByOrgIdPresentAfterInsert = _queryRepository.EpaOrganisationExistsWithOrganisationId(_organisationIdCreated).Result;
            var isOrgByUkprnPresentAfterInsert = _queryRepository.EpaOrganisationExistsWithUkprn(_ukprnCreated).Result;
            var returnedOrganisationByGetById = _queryRepository.GetEpaOrganisationById(_id).Result;
            var returnedOrganisationByGetByOrganisationId =
                _queryRepository.GetEpaOrganisationByOrganisationId(_organisationIdCreated).Result;

            Assert.IsFalse(isOrgByOrgIdPresentBeforeInsert);
            Assert.IsFalse(isOrgByUkprnPresentBeforeInsert);

            Assert.AreEqual(_organisation.Ukprn, returnedOrganisationByGetById.Ukprn);
            Assert.IsTrue(isOrgByOrgIdPresentAfterInsert);

            Assert.IsTrue(isOrgByUkprnPresentAfterInsert);
            Assert.AreEqual(_ukprnCreated, returnedOrganisationByGetById.Ukprn);
            Assert.AreEqual(_ukprnCreated, returnedOrganisationByGetByOrganisationId.Ukprn);
        }



        [OneTimeTearDown]
        public void TearDownOrganisationTests()
        {
            OrganisationHandler.DeleteRecordByOrganisationId(_organisationIdCreated);
            OrganisationTypeHandler.DeleteRecord(_organisationTypeId);
        }
    }
}
