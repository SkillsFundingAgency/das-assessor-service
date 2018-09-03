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
    public class GetOrgaGetAssessmentOrganisationsByOrganisationIdTestsnisationsByUkprnTests : TestBase
    {
        private OrganisationModel _organisation1;
        private OrganisationModel _organisation2;
        private readonly DatabaseService _databaseService = new DatabaseService();
        private RegisterQueryRepository _repository;
        private string _orgId1;
        private string _orgId2;
        private int _ukprn1;

        [OneTimeSetUp]
        public void SetupOrganisationTests()
        {
            _repository = new RegisterQueryRepository(_databaseService.WebConfiguration);
            _orgId1 = "EPA0001";
            _orgId2 = "EPA005";
            _ukprn1 = 876544;
            _organisation1 = new OrganisationModel
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now.AddYears(-1).Date,
                DeletedAt = null,
                EndPointAssessorName = "Name 1",
                EndPointAssessorOrganisationId = _orgId1,
                EndPointAssessorUkprn = _ukprn1,
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
                EndPointAssessorOrganisationId = _orgId2,
                EndPointAssessorUkprn = 9888,
                PrimaryContact = null,
                Status = "new",
                UpdatedAt = null,
                OrganisationTypeId = null,
                OrganisationData = null
            };

            OrganisationHandler.InsertRecords(new List<OrganisationModel> { _organisation1, _organisation2 });
        }

        [TestCase("EPA0001", 1)]
        [TestCase("EPA9999", 0)]
        [TestCase("test", 0)]
        public void RunGetAllOrganisationsAndCheckAllOrganisationsExpectedAreReturned(string orgId, int expectedCount)
        {
            var organisationsReturned = _repository.GetAssessmentOrganisationsByOrganisationId(orgId).Result.ToList();
            Assert.AreEqual(expectedCount, organisationsReturned.Count(), $@"Expected {expectedCount} organisations back but got {organisationsReturned.Count()}");
        }



        [OneTimeTearDown]
        public void TearDownOrganisationTests()
        {
            OrganisationHandler.DeleteRecords(new List<Guid> { _organisation1.Id, _organisation2.Id });
        }
    }
}
