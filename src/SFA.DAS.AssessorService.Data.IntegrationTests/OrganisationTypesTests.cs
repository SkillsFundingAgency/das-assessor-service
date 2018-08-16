using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;

namespace SFA.DAS.AssessorService.Data.IntegrationTests
{
    public class OrganisationTypesTests:TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        private RegisterQueryRepository _repository;
        private OrganisationTypeModel _organisationType1;
        private OrganisationTypeModel _organisationType2;

        [OneTimeSetUp]
        public void SetupOrganisationTypesTests()
        {
            _repository = new RegisterQueryRepository(_databaseService.WebConfiguration);
            _organisationType1 = new OrganisationTypeModel {Id = 10, Status = "Live", Type = "Award Organisation"};
            _organisationType2 = new OrganisationTypeModel {Id = 20, Status = "New", Type = "Some Other"};
            var organisationTypes = new List<OrganisationTypeModel> {_organisationType1, _organisationType2};

            OrganisationTypeHandler.InsertRecords(organisationTypes);
        }

        [Test]
        public void RunGetAllOrganisationTypesAndCheckAllOrganisationsExpectedAreReturned()
        {

            var orgTypesReturned = _repository.GetOrganisationTypes().Result.ToArray();

            Assert.AreEqual(2, orgTypesReturned.Count(), $@"Expected 2 organisation types back but got {orgTypesReturned.Count()}");
            Assert.AreEqual(1, orgTypesReturned.Count(x => x.Id == _organisationType1.Id), "Organisation Type 1 Id was not found");
            Assert.AreEqual(1, orgTypesReturned.Count(x => x.Id == _organisationType2.Id), "Organisation Type 2 Id was not found");
        }

        [OneTimeTearDown]
        public void TearDownOrganisationTypesTests()
        {
            OrganisationTypeHandler.DeleteRecords(new List<int>{ _organisationType1.Id, _organisationType2.Id });
        }
    }
}
