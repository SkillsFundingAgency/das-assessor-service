using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using System;
using System.Data.SqlClient;
using System.Linq;
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

        [TestCase(12345678, 123, "ST0001")]
        [TestCase(23456781, 231, "ST0002")]
        public async Task LoadOfsStandards_AddStandardToOrganisationStandards_WhenIlrContainsDataForOfsStandard(
            int ukprn, int stdCode, string ifateReferenceNumber)
        {
            var ofsOrganisationId = Guid.NewGuid();
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfsStandardsTestsFixture()
                .WithStandard("BrickLayer", ifateReferenceNumber, stdCode, "1.0", null, true, "Office for Students", false)
                .WithOrganisation("OrganisationOne", "EPA0001", ukprn, string.Empty)
                .WithOfsOrganisation(ofsOrganisationId, ukprn, currentDateTime.AddDays(-1))
                .WithProvider(ukprn)
                .WithContact("DisplayName", "displayname@organisationone.com", "EPA0001", "username")
                .WithIlr(Guid.NewGuid(), 1234567890, ukprn, stdCode, 1, IlrHandler.GetAcademicYear(currentDateTime)))
            {
                var results = await fixture.LoadOfsStandards(currentDateTime);

                var expected = await OrganisationStandardHandler.Create(null, "EPA0001", stdCode, currentDateTime.Date, null,
                    currentDateTime.Date, "Added from OFS matching ILR data", "displayname@organisationone.com", "Live", ifateReferenceNumber);

                results.VerifyUpdated(1);
                await results.VerifyOrganisationStandardRowCount(1);
                await results.VerifyOrganisationStandardExists(expected);
            }
        }

        [TestCase(12345678, 123, "ST0001", 231, "ST0002")]
        public async Task LoadOfsStandards_AddStandardToOrganisationStandards_WhenIlrContainsMultipleDataForOfsStandard(
            int ukprn, int stdCodeFirst, string ifateReferenceNumberFirst, int stdCodeSecond, string ifateReferenceNumberSecond)
        {
            var ofsOrganisationIdFirst = Guid.NewGuid();
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfsStandardsTestsFixture()
                .WithStandard("BrickLayer", ifateReferenceNumberFirst, stdCodeFirst, "1.0", null, true, "Office for Students", false)
                .WithStandard("Plaster", ifateReferenceNumberSecond, stdCodeSecond, "1.0", null, true, "Office for Students", false)
                .WithOrganisation("OrganisationOne", "EPA0001", ukprn, string.Empty)
                .WithOfsOrganisation(ofsOrganisationIdFirst, ukprn, currentDateTime.AddDays(-1))
                .WithProvider(ukprn)
                .WithContact("DisplayName", "displayname@organisationone.com", "EPA0001", "username")
                .WithIlr(Guid.NewGuid(), 1234567890, ukprn, stdCodeFirst, 1, IlrHandler.GetAcademicYear(currentDateTime))
                .WithIlr(Guid.NewGuid(), 1234567890, ukprn, stdCodeSecond, 1, IlrHandler.GetAcademicYear(currentDateTime)))
            {
                var results = await fixture.LoadOfsStandards(currentDateTime);

                var expectedFirst = await OrganisationStandardHandler.Create(null, "EPA0001", stdCodeFirst, currentDateTime.Date, null,
                    currentDateTime.Date, "Added from OFS matching ILR data", "displayname@organisationone.com", "Live", ifateReferenceNumberFirst);

                var expectedSecond = await OrganisationStandardHandler.Create(null, "EPA0001", stdCodeSecond, currentDateTime.Date, null,
                    currentDateTime.Date, "Added from OFS matching ILR data", "displayname@organisationone.com", "Live", ifateReferenceNumberSecond);

                results.VerifyUpdated(2);
                await results.VerifyOrganisationStandardRowCount(2);
                await results.VerifyOrganisationStandardExists(expectedFirst);
                await results.VerifyOrganisationStandardExists(expectedSecond);
            }
        }

        [TestCase(12345678, 23456781, 123, "ST0001")]
        public async Task LoadOfsStandards_AddStandardToOrganisationStandards_WhenIlrContainsMultipleDataForOfsStandardAndOrganisation(
            int ukprnFirst, int ukprnSecond, int stdCode, string ifateReferenceNumber)
        {
            var ofsOrganisationIdFirst = Guid.NewGuid();
            var ofsOrganisationIdSecond = Guid.NewGuid();
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfsStandardsTestsFixture()
                .WithStandard("BrickLayer", ifateReferenceNumber, stdCode, "1.0", null, true, "Office for Students", false)
                .WithOrganisation("OrganisationOne", "EPA0001", ukprnFirst, string.Empty)
                .WithOrganisation("OrganisationTwo", "EPA0002", ukprnSecond, string.Empty)
                .WithOfsOrganisation(ofsOrganisationIdFirst, ukprnFirst, currentDateTime.AddDays(-1))
                .WithOfsOrganisation(ofsOrganisationIdSecond, ukprnSecond, currentDateTime.AddDays(-1))
                .WithProvider(ukprnFirst)
                .WithProvider(ukprnSecond)
                .WithContact("DisplayName", "displayname@organisationone.com", "EPA0001", "usernameone")
                .WithContact("DisplayName", "displayname@organisationtwo.com", "EPA0002", "usernametwo")
                .WithIlr(Guid.NewGuid(), 1234567890, ukprnFirst, stdCode, 1, IlrHandler.GetAcademicYear(currentDateTime))
                .WithIlr(Guid.NewGuid(), 2345678901, ukprnSecond, stdCode, 1, IlrHandler.GetAcademicYear(currentDateTime)))
            {
                var results = await fixture.LoadOfsStandards(currentDateTime);

                var expectedFirst = await OrganisationStandardHandler.Create(null, "EPA0001", stdCode, currentDateTime.Date, null,
                    currentDateTime.Date, "Added from OFS matching ILR data", "displayname@organisationone.com", "Live", ifateReferenceNumber);

                var expectedSecond = await OrganisationStandardHandler.Create(null, "EPA0002", stdCode, currentDateTime.Date, null,
                    currentDateTime.Date, "Added from OFS matching ILR data", "displayname@organisationtwo.com", "Live", ifateReferenceNumber);

                results.VerifyUpdated(2);
                await results.VerifyOrganisationStandardRowCount(2);
                await results.VerifyOrganisationStandardExists(expectedFirst);
                await results.VerifyOrganisationStandardExists(expectedSecond);
            }
        }

        [TestCase(12345678, 123, "ST0001")]
        [TestCase(23456781, 231, "ST0002")]
        public async Task LoadOfsStandards_DoNotAddStandardToOrganisationStandards_WhenIlrContainsDataForOfsStandardForPreviousAcademicYear(
            int ukprn, int stdCode, string ifateReferenceNumber)
        {
            var ofsOrganisationId = Guid.NewGuid();
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfsStandardsTestsFixture()
                .WithStandard("BrickLayer", ifateReferenceNumber, stdCode, "1.0", null, true, "Office for Students", false)
                .WithOrganisation("OrganisationOne", "EPA0001", ukprn, string.Empty)
                .WithOfsOrganisation(ofsOrganisationId, ukprn, currentDateTime.AddDays(-1))
                .WithProvider(ukprn)
                .WithContact("DisplayName", "displayname@organisationone.com", "EPA0001", "username")
                .WithIlr(Guid.NewGuid(), 1234567890, ukprn, stdCode, 1, IlrHandler.GetAcademicYear(currentDateTime.AddYears(-1))))
            {
                var results = await fixture.LoadOfsStandards(currentDateTime);

                results.VerifyUpdated(0);
                await results.VerifyOrganisationStandardRowCount(0);
            }
        }

        [TestCase(12345678, 123, "ST0001")]
        [TestCase(23456781, 231, "ST0002")]
        public async Task LoadOfsStandards_DoNotAddStandardToOrganisationStandards_WhenIlrContainsDataForOfsStandardForWithdrawnLearner(
            int ukprn, int stdCode, string ifateReferenceNumber)
        {
            var ofsOrganisationId = Guid.NewGuid();
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfsStandardsTestsFixture()
                .WithStandard("BrickLayer", ifateReferenceNumber, stdCode, "1.0", null, true, "Office for Students", false)
                .WithOrganisation("OrganisationOne", "EPA0001", ukprn, string.Empty)
                .WithOfsOrganisation(ofsOrganisationId, ukprn, currentDateTime.AddDays(-1))
                .WithProvider(ukprn)
                .WithContact("DisplayName", "displayname@organisationone.com", "EPA0001", "username")
                .WithIlr(Guid.NewGuid(), 1234567890, ukprn, stdCode, 3, IlrHandler.GetAcademicYear(currentDateTime)))
            {
                var results = await fixture.LoadOfsStandards(currentDateTime);

                results.VerifyUpdated(0);
                await results.VerifyOrganisationStandardRowCount(0);
            }
        }

        [TestCase(12345678, 123, "ST0001")]
        [TestCase(23456781, 231, "ST0002")]
        public async Task LoadOfsStandards_DoNotAddStandardToOrganisationStandards_WhenIlrContainsDataForOfsStandardWhichRequiresApproval(
            int ukprn, int stdCode, string ifateReferenceNumber)
        {
            var ofsOrganisationId = Guid.NewGuid();
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfsStandardsTestsFixture()
                .WithStandard("BrickLayer", ifateReferenceNumber, stdCode, "1.0", null, true, "Office for Students", true)
                .WithOrganisation("OrganisationOne", "EPA0001", ukprn, string.Empty)
                .WithOfsOrganisation(ofsOrganisationId, ukprn, currentDateTime.AddDays(-1))
                .WithProvider(ukprn)
                .WithContact("DisplayName", "displayname@organisationone.com", "EPA0001", "username")
                .WithIlr(Guid.NewGuid(), 1234567890, ukprn, stdCode, 1, IlrHandler.GetAcademicYear(currentDateTime)))
            {
                var results = await fixture.LoadOfsStandards(currentDateTime);

                results.VerifyUpdated(0);
                await results.VerifyOrganisationStandardRowCount(0);
            }
        }

        [TestCase(12345678, 123, "ST0001")]
        [TestCase(23456781, 231, "ST0002")]
        public async Task LoadOfsStandards_DoNotAddStandardToOrganisationStandards_WhenIlrContainsDataForExistingOrganisationStandard(
            int ukprn, int stdCode, string ifateReferenceNumber)
        {
            var ofsOrganisationId = Guid.NewGuid();
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfsStandardsTestsFixture()
                .WithStandard("BrickLayer", ifateReferenceNumber, stdCode, "1.0", null, true, "Office for Students", false)
                .WithOrganisation("OrganisationOne", "EPA0001", ukprn, string.Empty)
                .WithOrganisationStandard(1, "EPA0001", stdCode, ifateReferenceNumber, currentDateTime.Date, null, currentDateTime.Date)
                .WithOfsOrganisation(ofsOrganisationId, ukprn, currentDateTime.AddDays(-1))
                .WithProvider(ukprn)
                .WithContact("DisplayName", "displayname@organisationone.com", "EPA0001", "username")
                .WithIlr(Guid.NewGuid(), 1234567890, ukprn, stdCode, 1, IlrHandler.GetAcademicYear(currentDateTime)))
            {
                var results = await fixture.LoadOfsStandards(currentDateTime);

                results.VerifyUpdated(0);
                await results.VerifyOrganisationStandardRowCount(1);
            }
        }

        [TestCase(12345678, 123, "ST0001")]
        [TestCase(23456781, 231, "ST0002")]
        public async Task LoadOfsStandards_DoNotAddStandardToOrganisationStandards_WhenIlrContainsDataForNonOfsStandard(
            int ukprn, int stdCode, string ifateReferenceNumber)
        {
            var ofsOrganisationId = Guid.NewGuid();
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfsStandardsTestsFixture()
                .WithStandard("BrickLayer", ifateReferenceNumber, stdCode, "1.0", null, true, "Ofqual")
                .WithOrganisation("OrganisationOne", "EPA0001", ukprn, string.Empty)
                .WithOfsOrganisation(ofsOrganisationId, ukprn, currentDateTime.AddDays(-1))
                .WithProvider(12345678)
                .WithContact("DisplayName", "displayname@organisationone.com", "EPA0001", "username")
                .WithIlr(Guid.NewGuid(), 1234567890, ukprn, stdCode, 1, IlrHandler.GetAcademicYear(currentDateTime)))
            {
                var results = await fixture.LoadOfsStandards(currentDateTime);

                results.VerifyUpdated(0);
                await results.VerifyOrganisationStandardRowCount(0);
            }
        }

        [TestCase(12345678, 101, "ST0001", new[] { "1.0", "1.1", "1.2" })]
        [TestCase(12345678, 102, "ST0002", new[] { "1.0", "1.1", "1.2" })]
        [TestCase(12345678, 103, "ST0003", new[] { "1.1", "1.2" })]
        [TestCase(12345678, 104, "ST0004", new[] { "1.2" })]
        [TestCase(12345678, 105, "ST0005", new[] { "1.2" })]
        [TestCase(12345678, 106, "ST0006", new[] { "1.2" })]
        [TestCase(12345678, 107, "ST0007", new[] { "1.1", "1.2" })]
        [TestCase(12345678, 108, "ST0008", new[] { "1.2" })]
        public async Task LoadOfsStandards_AddOrganisationStandardVersionsForLatestEpaPlan_WhenIlrContainsDataForOfsStandard(
            int ukprn, int stdCode, string ifateReferenceNumber, string[] versionsWithLatestEpaPlan)
        {
            var ofsOrganisationId = Guid.NewGuid();
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfsStandardsTestsFixture()
                .WithStandard("One", "ST0001", 101, "1.0", null, false, "Office for Students", false)
                .WithStandard("One", "ST0001", 101, "1.1", null, false, "Office for Students", false)
                .WithStandard("One", "ST0001", 101, "1.2", null, false, "Office for Students", false)
                .WithStandard("Two", "ST0002", 102, "1.0", null, true, "Office for Students", false)
                .WithStandard("Two", "ST0002", 102, "1.1", null, false, "Office for Students", false)
                .WithStandard("Two", "ST0002", 102, "1.2", null, false, "Office for Students", false)
                .WithStandard("Three", "ST0003", 103, "1.0", null, false, "Office for Students", false)
                .WithStandard("Three", "ST0003", 103, "1.1", null, true, "Office for Students", false)
                .WithStandard("Three", "ST0003", 103, "1.2", null, false, "Office for Students", false)
                .WithStandard("Four", "ST0004", 104, "1.0", null, false, "Office for Students", false)
                .WithStandard("Four", "ST0004", 104, "1.1", null, false, "Office for Students", false)
                .WithStandard("Four", "ST0004", 104, "1.2", null, true, "Office for Students", false)
                .WithStandard("Five", "ST0005", 105, "1.0", null, false, "Office for Students", false)
                .WithStandard("Five", "ST0005", 105, "1.1", null, true, "Office for Students", false)
                .WithStandard("Five", "ST0005", 105, "1.2", null, true, "Office for Students", false)
                .WithStandard("Six", "ST0006", 106, "1.0", null, true, "Office for Students", false)
                .WithStandard("Six", "ST0006", 106, "1.1", null, false, "Office for Students", false)
                .WithStandard("Six", "ST0006", 106, "1.2", null, true, "Office for Students", false)
                .WithStandard("Seven", "ST0007", 107, "1.0", null, true, "Office for Students", false)
                .WithStandard("Seven", "ST0007", 107, "1.1", null, true, "Office for Students", false)
                .WithStandard("Seven", "ST0007", 107, "1.2", null, false, "Office for Students", false)
                .WithStandard("Eight", "ST0008", 108, "1.0", null, true, "Office for Students", false)
                .WithStandard("Eight", "ST0008", 108, "1.1", null, true, "Office for Students", false)
                .WithStandard("Eight", "ST0008", 108, "1.2", null, true, "Office for Students", false)
                .WithOrganisation("OrganisationOne", "EPA0001", ukprn, string.Empty)
                .WithOfsOrganisation(ofsOrganisationId, ukprn, currentDateTime.AddDays(-1))
                .WithProvider(ukprn)
                .WithContact("DisplayName", "displayname@organisationone.com", "EPA0001", "username")
                .WithIlr(Guid.NewGuid(), 1234567890, ukprn, stdCode, 1, IlrHandler.GetAcademicYear(currentDateTime)))
            {
                var results = await fixture.LoadOfsStandards(currentDateTime);

                results.VerifyUpdated(1);
                await results.VerifyOrganisationStandardRowCount(1);
                await results.VerifyOrganisationStandardVersionRowCount(versionsWithLatestEpaPlan.Count());

                foreach (var version in versionsWithLatestEpaPlan)
                {
                    var expectedResult = OrganisationStandardVersionHandler.Create($"{ifateReferenceNumber}_{version}", version, null, currentDateTime.Date,
                        null, currentDateTime.Date, "Added from OFS matching ILR data", "Live");

                    await results.VerifyOrganisationStandardVersionExists(expectedResult);
                }
            }
        }

        [TestCase(12345678, 123, "ST0001")]
        public async Task LoadOfsStandards_AddOrganisationStandardDeliveryArea__WhenIlrContainsDataForOfsStandard(
            int ukprn, int stdCode, string ifateReferenceNumber)
        {
            var ofsOrganisationId = Guid.NewGuid();
            var currentDateTime = DateTime.Now;

            using (var fixture = new LoadOfsStandardsTestsFixture()
                .WithStandard("BrickLayer", ifateReferenceNumber, stdCode, "1.0", null, true, "Office for Students", false)
                .WithOrganisation("OrganisationOne", "EPA0001", ukprn, string.Empty)
                .WithOfsOrganisation(ofsOrganisationId, ukprn, currentDateTime.AddDays(-1))
                .WithProvider(ukprn)
                .WithContact("DisplayName", "displayname@organisationone.com", "EPA0001", "username")
                .WithIlr(Guid.NewGuid(), 1234567890, ukprn, stdCode, 1, IlrHandler.GetAcademicYear(currentDateTime)))
            {
                var results = await fixture.LoadOfsStandards(currentDateTime);

                results.VerifyUpdated(1);
                await results.VerifyOrganisationStandardRowCount(1);
                await results.VerifyOrganisationStandardDeliveryAreaRowCount(9);
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
