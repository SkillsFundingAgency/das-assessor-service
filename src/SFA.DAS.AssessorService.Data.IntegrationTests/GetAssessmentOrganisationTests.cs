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
    public class GetAssessmentOrganisationTests : TestBase
    {
        private OrganisationModel _organisation1;
        private readonly DatabaseService _databaseService = new DatabaseService();
        private RegisterQueryRepository _repository;
        private string _orgId1;

        [OneTimeSetUp]
        public void SetupOrganisationTests()
        {
            _repository = new RegisterQueryRepository(_databaseService.WebConfiguration);
            _orgId1 = "EPA0111";

            _organisation1 = new OrganisationModel
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now.AddYears(-1).Date,
                DeletedAt = null,
                EndPointAssessorName = "Name 1",
                EndPointAssessorOrganisationId = _orgId1,
                EndPointAssessorUkprn = 876544,
                PrimaryContact = null,
                Status = "new",
                UpdatedAt = null,
                OrganisationTypeId = null,
                OrganisationData = null
            };

            OrganisationHandler.InsertRecord(_organisation1);
        }

        [Test]
        public void RunGetOrganisationAndCheckOrganisationExpectedIsReturned()
        {
            var organisationReturned = _repository.GetEpaOrganisationByOrganisationId(_orgId1).Result;
            Assert.AreEqual(_organisation1.EndPointAssessorName, organisationReturned.Name, "The organisation names do not match");
            Assert.AreEqual(_organisation1.EndPointAssessorOrganisationId, organisationReturned.OrganisationId, "The organisation Ids do not match");
        }

        [Test]
        public void RunGetOrganisationThatDoesntExistAndCheckNoDetailsAreReturned()
        {
            var organisationReturned = _repository.GetEpaOrganisationByOrganisationId("ABC").Result;
            Assert.IsNull(organisationReturned);
        }


        [OneTimeTearDown]
        public void TearDownOrganisationTests()
        {
            OrganisationHandler.DeleteRecord(_organisation1.Id);
        }
    }
}
