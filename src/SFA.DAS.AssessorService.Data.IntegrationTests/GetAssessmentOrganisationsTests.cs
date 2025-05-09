﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;

namespace SFA.DAS.AssessorService.Data.IntegrationTests
{
    public class GetAssessmentOrganisationsTests : TestBase
    {
        private OrganisationModel _organisation1;
        private OrganisationModel _organisation2;
        private readonly DatabaseService _databaseService = new DatabaseService();
        private RegisterQueryRepository _repository;
        private string _organisationId1;
        private string _organisationId2;

        [OneTimeSetUp]
        public void SetupOrganisationTests()
        {
            var databaseConnection = new SqlConnection(_databaseService.SqlConnectionStringTest);
            var unitOfWork = new UnitOfWork(databaseConnection);

            _repository = new RegisterQueryRepository(unitOfWork);

            _organisationId1 = "EPA0001";
            _organisationId2 = "EPA005";
            _organisation1 = new OrganisationModel
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now.AddYears(-1).Date,
                DeletedAt = null,
                EndPointAssessorName = "Name 1",
                EndPointAssessorOrganisationId = _organisationId1,
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
                EndPointAssessorOrganisationId = _organisationId2,
                EndPointAssessorUkprn = 9888,
                PrimaryContact = null,
                Status = "new",
                UpdatedAt = null,
                OrganisationTypeId = null,
                OrganisationData = null
            };

            OrganisationHandler.InsertRecords(new List<OrganisationModel> { _organisation1, _organisation2 });
        }

        [Test]
        public void RunGetAllOrganisationsAndCheckAllOrganisationsExpectedAreReturned()
        {
            var organisationsReturned = _repository.GetAssessmentOrganisations().Result.ToList();
            organisationsReturned.Count.Should().Be(2, $@"Expected 2 organisations back but got {organisationsReturned.Count()}");
            organisationsReturned.Count(x => x.Id == _organisation1.EndPointAssessorOrganisationId).Should().Be(1, "Organisation 1 Id was not found");
            organisationsReturned.Count(x => x.Id == _organisation2.EndPointAssessorOrganisationId).Should().Be(1, "Organisation 2 Id was not found");
        }

        [Test]
        public void GetOrganisationByIdAndCheckTheOrganisationIsReturned()
        {
            var organisation = OrganisationHandler.GetOrganisationSummaryByOrgId(_organisationId2);
            organisation.Name.Should().Be(_organisation2.EndPointAssessorName, "The organisation names do not match");
            organisation.Id.Should().Be(_organisation2.EndPointAssessorOrganisationId, "The organisation Ids do not match");
        }

        [OneTimeTearDown]
        public void TearDownOrganisationTests()
        {
            OrganisationHandler.DeleteRecords(new List<Guid> { _organisation1.Id, _organisation2.Id });
        }
    }
}
