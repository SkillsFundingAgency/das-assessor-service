using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;

namespace SFA.DAS.AssessorService.Data.IntegrationTests
{
    public class OrganisationsTests: TestBase
    {
        private OrganisationModel _organisation1;
        private OrganisationModel _organisation2;
        private readonly DatabaseService _databaseService = new DatabaseService();
        private OrganisationQueryRepository _repository;

        [SetUp]
        public void SetupOrganisationTests()
        {
            _repository = new OrganisationQueryRepository(_databaseService.TestContext);
            _organisation1 = new OrganisationModel
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now.AddYears(-1).Date,
                DeletedAt = null,
                EndPointAssessorName = "Name 1",
                EndPointAssessorOrganisationId = "EPA0001",
                EndPointAssessorUkprn = 876544,
                PrimaryContact = null,
                Status = "new",
                UpdatedAt = null,
                OrganisationTypeId = null,
                OrganisationData = null
            };

            _organisation2 = new OrganisationModel
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now.AddMonths(-1).Date,
                DeletedAt = null,
                EndPointAssessorName = "Name 2",
                EndPointAssessorOrganisationId = "EPA0002",
                EndPointAssessorUkprn = 9888,
                PrimaryContact = null,
                Status = "new",
                UpdatedAt = null,
                OrganisationTypeId = null,
                OrganisationData = null
            };

            OrganisationHandler.InsertRecords(new List<OrganisationModel> {_organisation1, _organisation2});
        }

        [Test]
        public void RunGetAllOrganisationsAndCheckAllOrganisationsExpectedAreReturned()
        {          
           
            var organisationsReturned = _repository.GetAllOrganisations().Result.ToList();

            Assert.AreEqual(2, organisationsReturned.Count(), $@"Expected 2 organisations back but got {organisationsReturned.Count()}");
            Assert.AreEqual(1, organisationsReturned.Count(x => x.Id == _organisation1.Id),"Organisation 1 Id was not found");
            Assert.AreEqual(1, organisationsReturned.Count(x => x.Id == _organisation2.Id), "Organisation 2 Id was not found");
        }

        [TestCase("EPA0001")]
        [TestCase("EPA0002")]
        public void RunCheckOrganisationExists(string organisationId)
        {
            var alreadyExists = _repository.CheckIfAlreadyExists(organisationId).Result;
            Assert.IsTrue(alreadyExists);     
        }

        [Test]
        public void RunCheckOrganisationHasContactsIsFalseWhenOrganisationHasNoContacts()
        {
            var hasContacts = _repository.CheckIfOrganisationHasContacts(_organisation1.EndPointAssessorOrganisationId).Result;
            Assert.IsFalse(hasContacts);
        }

        [Test]
        public void GetOrganisationByIdAndCheckTheOrganisationIsReturned()
        {
            var organisation = _repository.Get(_organisation2.Id).Result;
            Assert.AreEqual(_organisation2.Id,organisation.Id);
            Assert.AreEqual(_organisation2.EndPointAssessorName, organisation.EndPointAssessorName, "The organisation names do not match");
            Assert.AreEqual(_organisation2.EndPointAssessorOrganisationId, organisation.EndPointAssessorOrganisationId, "The organisation Ids do not match");
        }

        [TearDown]
        public void TearDownOrganisationTests()
        {
            OrganisationHandler.DeleteRecords(new List<Guid>{_organisation1.Id, _organisation2.Id});
        }
    }
}
