using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Repositories.StandardRepositoryTests
{
    public class LoadOfsStandardsTests : TestBase
    {
        [TestCase(23456781, "Registered", null, 2)]
        [TestCase(23456781, "Registered", "", 2)]
        [TestCase(23456781, "Registered", "Bachelor's", 2)]
        [TestCase(23456781, "Registered", "Bachelor's - New DAPs", 2)]
        [TestCase(23456781, "Registered", "Foundation", 2)]
        [TestCase(23456781, "Registered", "Not applicable", 1)]
        [TestCase(23456781, "Registered", "Research", 2)]
        [TestCase(23456781, "Registered", "Taught", 2)]
        [TestCase(23456781, "Registered", "Taught - New DAPs", 2)]
        [TestCase(23456781, "No longer registered", null, 1)]
        [TestCase(23456781, "No longer registered", "", 1)]
        [TestCase(23456781, "No longer registered", "Bachelor's", 1)]
        [TestCase(23456781, "No longer registered", "Bachelor's - New DAPs", 1)]
        [TestCase(23456781, "No longer registered", "Foundation", 1)]
        [TestCase(23456781, "No longer registered", "Not applicable", 1)]
        [TestCase(23456781, "No longer registered", "Research", 1)]
        [TestCase(23456781, "No longer registered", "Taught", 1)]
        [TestCase(23456781, "No longer registered", "Taught - New DAPs", 1)]
        public async Task LoadOfsStandards_UpdateOfsOrganiation_WhenStagingContainsUpdatedData(
            int ukprn, string registrationStatus, string highestLevelOfDegreeAwardingPowers, int ofsOrganisationsRowCount)
        {
            var ofsOrganisationId = Guid.NewGuid();
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfsStandardsTestsFixture()
                .WithOfsOrganisation(ofsOrganisationId, 12345678, currentDateTime.AddDays(-1))
                .WithStagingOfsOrganisation(12345678, "Registered", "Taught")
                .WithStagingOfsOrganisation(ukprn, registrationStatus, highestLevelOfDegreeAwardingPowers))
            {
                var results = await fixture.LoadOfsStandards(currentDateTime);

                await results.VerifyOfsOrganisationExists(OfsOrganisationHandler.Create(ofsOrganisationId, 12345678, currentDateTime.AddDays(-1)));
                await results.VerifyOfsOrganisationRowCount(ofsOrganisationsRowCount);

                if(ofsOrganisationsRowCount == 2)
                {
                    await results.VerifyOfsOrganisationExists(OfsOrganisationHandler.Create(null, ukprn, currentDateTime));
                }
            }
        }

        [TestCase(12345678)]
        public async Task LoadOfsStandards_DoesNotRemoveOfsOrganiation_WhenStagingContainsRemovedData(int ukprn)
        {
            var ofsOrganisationId = Guid.NewGuid();
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfsStandardsTestsFixture()
                .WithOfsOrganisation(ofsOrganisationId, ukprn, currentDateTime.AddDays(-1)))
            {
                var results = await fixture.LoadOfsStandards(currentDateTime);

                await results.VerifyOfsOrganisationExists(OfsOrganisationHandler.Create(ofsOrganisationId, ukprn, currentDateTime.AddDays(-1)));
            }
        }

        private class LoadOfsStandardsTestsFixture : FixtureBase<LoadOfsStandardsTestsFixture>, IDisposable
        {
            private readonly DatabaseService _databaseService = new DatabaseService();
            private readonly SqlConnection _sqlConnection;

            private StandardRepository _repository;
            public int _updated;

            public LoadOfsStandardsTestsFixture()
            {
                _sqlConnection = new SqlConnection(_databaseService.WebConfiguration.SqlConnectionString);
                _repository = new StandardRepository(new UnitOfWork(_sqlConnection));
            }

            public async Task<LoadOfsStandardsTestsFixture> LoadOfsStandards(DateTime dateTimeUtc)
            {
                _updated = await _repository.LoadOfsStandards(dateTimeUtc);
                return this;
            }

            public LoadOfsStandardsTestsFixture VerifyUpdated(int updated)
            {
                _updated.Should().Be(updated);
                return this;
            }
        }
    }
}
