using System;
using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;

namespace SFA.DAS.AssessorService.Data.IntegrationTests
{
    public class RegisterRepositoryEpaOrganisationAlreadyUsingUkprnTests : TestBase
    {
        private readonly DatabaseService _databaseService = new DatabaseService();
        private RegisterQueryRepository _repository;
        private OrganisationModel _organisation2;
        private OrganisationModel _organisation1;
        private string _organisationId1;
        private int _ukprn1;
        private int _ukprn2;

        [OneTimeSetUp]
        public void SetUpOrganisationTests()
        {
            _repository = new RegisterQueryRepository(_databaseService.WebConfiguration);
            _organisationId1 = "EPA0088";
            _ukprn1 = 876533;
            _ukprn2 = 9888;

            _organisation1 = new OrganisationModel
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.Now.AddYears(-1).Date,
                DeletedAt = null,
                EndPointAssessorName = "Name 1",
                EndPointAssessorOrganisationId = _organisationId1,
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
                EndPointAssessorOrganisationId = "EPA0002",
                EndPointAssessorUkprn = _ukprn2,
                PrimaryContact = null,
                Status = "new",
                UpdatedAt = null,
                OrganisationTypeId = null,
                OrganisationData = null
            };

            OrganisationHandler.InsertRecords(new List<OrganisationModel> { _organisation1, _organisation2 });
        }

        [Test]
        public void CheckEpaOrganisationIsntAlreadyUsingUkprnWhereUsingOrganisationsCurrentUkprn()
        {
            var exists = _repository.EpaOrganisationAlreadyUsingUkprn(_ukprn1, _organisationId1).Result;
            Assert.IsFalse(exists);
        }

        [Test]
        public void CheckEpaOrganisationIsntAlreadyUsingUkprnWhenCheckingUnusedUkprn()
        {
            var exists = _repository.EpaOrganisationAlreadyUsingUkprn(323454, _organisationId1).Result;
            Assert.IsFalse(exists);
        }

        [Test]
        public void CheckEpaOrganisationIsAlreadyUsingUkprnWhenCheckingUkprnUsedByAnotherOrganisation()
        {
            var exists = _repository.EpaOrganisationAlreadyUsingUkprn(_ukprn2, _organisationId1).Result;
            Assert.IsTrue(exists);
        }

        [OneTimeTearDown]
        public void TearDownOrganisationTests()
        {
            OrganisationHandler.DeleteRecords(new List<Guid> { _organisation1.Id, _organisation2.Id });
        }
    }
}
