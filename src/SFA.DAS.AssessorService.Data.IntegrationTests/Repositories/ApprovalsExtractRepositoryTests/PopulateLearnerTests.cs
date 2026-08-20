using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.IntegrationTests.Factories;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.Roatp;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Repositories.ApprovalsExtractRepositoryTests
{
    public class PopulateLearnerTests : TestBase
    {
        [TestCase(-45, "1.0")]
        [TestCase(-40, "1.0")]
        [TestCase(-39, "1.1")]
        [TestCase(-35, "1.1")]
        [TestCase(-30, "1.1")]
        [TestCase(-29, "1.2")]
        [TestCase(-25, "1.2")]
        [TestCase(-20, "1.2")]
        [TestCase(-19, "1.3")]
        [TestCase(-11, "1.4")] // overlapping VersionEarliestStartDate with previous VersionLastestStartDate
        [TestCase(10, "1.4")]
        [TestCase(100, "1.4")]
        public async Task GetVersionFromLarsCode_GetsHighestVersionWithinVersionDateRangeForLearnStartDate(int learnStartDateOffset, string expectedVersion)
        {
            var now = DateTime.Now;
            var learnStartDateTime = now.AddDays(learnStartDateOffset);

            using (var fixture = new PopulateLearnerTestsFixture()
                .WithStandard("Standard 1", "ST0001", 123, "1.0", now.AddYears(-1).Date, null, null, now.AddDays(-40).Date, now.AddDays(10).Date)
                .WithStandard("Standard 1", "ST0001", 123, "1.1", now.AddYears(-1).Date, null, now.AddDays(-39).Date, now.AddDays(-30).Date, now.AddDays(10).Date)
                .WithStandard("Standard 1", "ST0001", 123, "1.2", now.AddYears(-1).Date, null, now.AddDays(-29).Date, now.AddDays(-20).Date, now.AddDays(10).Date)
                .WithStandard("Standard 1", "ST0001", 123, "1.3", now.AddYears(-1).Date, null, now.AddDays(-19).Date, now.AddDays(-10).Date, now.AddDays(10).Date)
                .WithStandard("Standard 1", "ST0001", 123, "1.4", now.AddYears(-1).Date, null, now.Date.AddDays(-12), null, null))
            {
                var results = await fixture.GetVersionFromLarsCode(learnStartDateTime, 123);
                results.VerifyVersionFromLarsCode(expectedVersion);
            }
        }

        [TestCase(-25, "1.0")]
        [TestCase(-20, "1.0")]
        [TestCase(-19, "1.1")]
        [TestCase(-10, "1.1")]
        [TestCase(-5, "1.1")] /* in range of a standard which is not approved for delivery */
        public async Task GetVersionFromLarsCode_WhenStandardNotApprovedForDelivery_GetsHighestApprovedVersionWithinVersionDateRangeForLearnStartDate(int learnStartDateOffset, string expectedVersion)
        {
            var now = DateTime.Now;
            var learnStartDateTime = now.AddDays(learnStartDateOffset);

            using (var fixture = new PopulateLearnerTestsFixture()
                .WithStandard("Standard 1", "ST0001", 123, "1.0", now.AddYears(-1).Date, null, null, now.AddDays(-20).Date, now.AddDays(10).Date)
                .WithStandard("Standard 1", "ST0001", 123, "1.1", now.AddYears(-1).Date, null, now.AddDays(-19).Date, now.AddDays(-10).Date, now.AddDays(10).Date)
                .WithStandard("Standard 1", "ST0001", 123, "1.2", now.AddYears(-1).Date, null, now.AddDays(-9).Date, null, null, null /* standard not approved for delivery */))
            {
                var results = await fixture.GetVersionFromLarsCode(learnStartDateTime, 123);
                results.VerifyVersionFromLarsCode(expectedVersion);
            }
        }

        [TestCase(-25, "1.0")]
        [TestCase(-20, "1.0")]
        [TestCase(-19, "1.1")]
        [TestCase(-10, "1.1")]
        [TestCase(-5, "1.2")]
        [TestCase(5, "1.2")] /* after the standard latest start date when there are no later versions */
        public async Task GetVersionFromLarsCode_WhenStandardVersionHasEnded_GetsHighestApprovedVersionWithinVersionDateRangeForLearnStartDate(int learnStartDateOffset, string expectedVersion)
        {
            var now = DateTime.Now;
            var learnStartDateTime = now.AddDays(learnStartDateOffset);

            using (var fixture = new PopulateLearnerTestsFixture()
                .WithStandard("Standard 1", "ST0001", 123, "1.0", now.AddYears(-1).Date, null, null, now.AddDays(-20).Date, now.AddDays(10).Date)
                .WithStandard("Standard 1", "ST0001", 123, "1.1", now.AddYears(-1).Date, null, now.AddDays(-19).Date, now.AddDays(-10).Date, now.AddDays(10).Date)
                .WithStandard("Standard 1", "ST0001", 123, "1.2", now.AddYears(-1).Date, null, now.AddDays(-9).Date, now.AddDays(0), now.AddDays(10).Date))
            {
                var results = await fixture.GetVersionFromLarsCode(learnStartDateTime, 123);
                results.VerifyVersionFromLarsCode(expectedVersion);
            }
        }

        [TestCase(-45, "ST0001_1.0")]
        [TestCase(-40, "ST0001_1.0")]
        [TestCase(-39, "ST0001_1.1")]
        [TestCase(-35, "ST0001_1.1")]
        [TestCase(-30, "ST0001_1.1")]
        [TestCase(-29, "ST0001_1.2")]
        [TestCase(-25, "ST0001_1.2")]
        [TestCase(-20, "ST0001_1.2")]
        [TestCase(-19, "ST0001_1.3")]
        [TestCase(-11, "ST0001_1.4")] // overlapping VersionEarliestStartDate with previous VersionLastestStartDate
        [TestCase(10, "ST0001_1.4")]
        [TestCase(100, "ST0001_1.4")]
        public async Task GetStandardUidFromLarsCode_GetsHighestStandardUiWithinVersionDateRangeForLearnStartDate(int learnStartDateOffset, string expectedStandardUid)
        {
            var now = DateTime.Now;
            var learnStartDateTime = now.AddDays(learnStartDateOffset);

            using (var fixture = new PopulateLearnerTestsFixture()
                .WithStandard("Standard 1", "ST0001", 123, "1.0", now.AddYears(-1).Date, null, null, now.AddDays(-40).Date, null)
                .WithStandard("Standard 1", "ST0001", 123, "1.1", now.AddYears(-1).Date, null, now.AddDays(-39).Date, now.AddDays(-30).Date, null)
                .WithStandard("Standard 1", "ST0001", 123, "1.2", now.AddYears(-1).Date, null, now.AddDays(-29).Date, now.AddDays(-20).Date, now.AddDays(10).Date)
                .WithStandard("Standard 1", "ST0001", 123, "1.3", now.AddYears(-1).Date, null, now.AddDays(-19).Date, now.AddDays(-10).Date, now.AddDays(10).Date)
                .WithStandard("Standard 1", "ST0001", 123, "1.4", now.AddYears(-1).Date, null, now.Date.AddDays(-12), null, null))
            {
                var results = await fixture.GetStandardUidFromLarsCode(learnStartDateTime, 123);
                results.VerifyStandardUidFromLarsCode(expectedStandardUid);
            }
        }

        [TestCase(-25, "ST0001_1.0")]
        [TestCase(-20, "ST0001_1.0")]
        [TestCase(-19, "ST0001_1.1")]
        [TestCase(-10, "ST0001_1.1")]
        [TestCase(-5, "ST0001_1.1")] /* in range of a standard which is not approved for delivery */
        public async Task GetStandardUidFromLarsCode_WhenStandardNotApprovedForDelivery_GetsHighestApprovedVersionWithinVersionDateRangeForLearnStartDate(int learnStartDateOffset, string expectedStandardUid)
        {
            var now = DateTime.Now;
            var learnStartDateTime = now.AddDays(learnStartDateOffset);

            using (var fixture = new PopulateLearnerTestsFixture()
                .WithStandard("Standard 1", "ST0001", 123, "1.0", now.AddYears(-1).Date, null, null, now.AddDays(-20).Date, now.AddDays(10).Date)
                .WithStandard("Standard 1", "ST0001", 123, "1.1", now.AddYears(-1).Date, null, now.AddDays(-19).Date, now.AddDays(-10).Date, now.AddDays(10).Date)
                .WithStandard("Standard 1", "ST0001", 123, "1.2", now.AddYears(-1).Date, null, now.AddDays(-9).Date, null, null, null /* standard not approved for delivery */))
            {
                var results = await fixture.GetStandardUidFromLarsCode(learnStartDateTime, 123);
                results.VerifyStandardUidFromLarsCode(expectedStandardUid);
            }
        }

        [TestCase(-25, "ST0001_1.0")]
        [TestCase(-20, "ST0001_1.0")]
        [TestCase(-19, "ST0001_1.1")]
        [TestCase(-10, "ST0001_1.1")]
        [TestCase(-5, "ST0001_1.2")]
        [TestCase(5, "ST0001_1.2")] /* after the standard latest start date when there are no later versions */
        public async Task GetStandardUidFromLarsCode_WhenStandardVersionHasEnded_GetsHighestApprovedVersionWithinVersionDateRangeForLearnStartDate(int learnStartDateOffset, string expectedStandardUid)
        {
            var now = DateTime.Now;
            var learnStartDateTime = now.AddDays(learnStartDateOffset);

            using (var fixture = new PopulateLearnerTestsFixture()
                .WithStandard("Standard 1", "ST0001", 123, "1.0", now.AddYears(-1).Date, null, null, now.AddDays(-20).Date, now.AddDays(10).Date)
                .WithStandard("Standard 1", "ST0001", 123, "1.1", now.AddYears(-1).Date, null, now.AddDays(-19).Date, now.AddDays(-10).Date, now.AddDays(10).Date)
                .WithStandard("Standard 1", "ST0001", 123, "1.2", now.AddYears(-1).Date, null, now.AddDays(-9).Date, now.AddDays(0), now.AddDays(10).Date))
            {
                var results = await fixture.GetStandardUidFromLarsCode(learnStartDateTime, 123);
                results.VerifyStandardUidFromLarsCode(expectedStandardUid);
            }
        }

        [TestCase(-50, "1.0", "ST0002_1.0")] // even when started before earliest start date
        [TestCase(0, "1.0", "ST0002_1.0")]
        [TestCase(50, "1.0", "ST0002_1.0")] // even when started after latest start date
        public async Task PopulatedLearner_WhenNoMatchingApprovalsExtract_AndOnlyStandardVersion1_0_ThenVersionAndStandardUidAreFromIlr_AndVersionConfirmed(
            int learnStartDateOffset,
            string expectedVersion,
            string expectedStandardUid)
        {
            var now = DateTime.Now;
            var plannedEndDateTime = now.AddMonths(12);
            var learnStartDateTime = now.AddDays(learnStartDateOffset);

            var standard123V1_0 = StandardFactory.Create(
                title: "Standard 1",
                referenceNumber: "ST0001",
                larsCode: 123,
                version: "1.0")
                .WithEffectiveFrom(now.AddYears(-1).Date)
                .WithVersionApprovedForDelivery(now.AddMonths(-1).Date);

            var standard456V1_0 = StandardFactory.Create(
                title: "Standard 2",
                referenceNumber: "ST0002",
                larsCode: 456,
                version: "1.0")
                .WithEffectiveFrom(now.AddYears(-1).Date)
                .WithVersionApprovedForDelivery(now.AddMonths(-1).Date)
                .WithVersionEarliestStartDate(now.AddDays(-25).Date)
                .WithVersionLatestStartDate(now.AddDays(25).Date);

            var ilr = IlrFactory.ForStandard(standard456V1_0, now)
                .WithLearnStartDate(learnStartDateTime)
                .WithPlannedEndDate(plannedEndDateTime);

            var approvalsExtract = ApprovalsExtractFactory.ForIlrAndStandard(ilr, standard123V1_0, now);

            using (var fixture = new PopulateLearnerTestsFixture()
                .WithStandard(standard123V1_0)
                .WithStandard(standard456V1_0)
                .WithIlr(ilr)
                .WithApprovalsExtract(approvalsExtract))
            {
                var results = await fixture.PopulateLearner();

                var expected = LearnerFactory.From(ilr, standard456V1_0)
                    .WithVersion(expectedVersion)
                    .WithVersionConfirmed(1) // version is confirmed when only 1.0 exists
                    .WithStandardUId(expectedStandardUid)
                    .WithLastUpdated(now.Date)
                    .WithLatestApprovals(null);

                results.VerifyUpdated(1);
                await results.VerifyLearnerRowCount(1);
                await results.VerifyLearnerExists(expected);
            }
        }

        [TestCase(-50, "1.0", "ST0002_1.0")]
        [TestCase(0, "1.1", "ST0002_1.1")]
        [TestCase(50, "1.1", "ST0002_1.1")]
        public async Task PopulatedLearner_WhenNoMatchingApprovalsExtract_AndMultipleStandardVersions_ThenVersionAndStandardUidAreFromIlr_AndVersionNotConfirmed(
            int learnStartDateOffset,
            string expectedVersion,
            string expectedStandardUid)
        {
            var now = DateTime.Now;
            var plannedEndDateTime = now.AddMonths(12);
            var learnStartDateTime = now.AddDays(learnStartDateOffset);

            var standard123V1_0 = StandardFactory.Create(
                title: "Standard 1",
                referenceNumber: "ST0001",
                larsCode: 123,
                version: "1.0")
                .WithEffectiveFrom(now.AddYears(-1).Date)
                .WithVersionApprovedForDelivery(now.AddMonths(-1).Date);

            var standard456V1_0 = StandardFactory.Create(
                title: "Standard 2",
                referenceNumber: "ST0002",
                larsCode: 456,
                version: "1.0")
                .WithEffectiveFrom(now.AddYears(-1).Date)
                .WithVersionApprovedForDelivery(now.AddMonths(-1).Date)
                .WithVersionLatestStartDate(now.Date);

            var standard456V1_1 = StandardFactory.Create(
                title: "Standard 2",
                referenceNumber: "ST0002",
                larsCode: 456,
                version: "1.1")
                .WithEffectiveFrom(now.AddYears(-1).Date)
                .WithVersionApprovedForDelivery(now.AddMonths(-1).Date)
                .WithVersionEarliestStartDate(now.Date);

            var ilr = IlrFactory.ForStandard(standard456V1_0, now)
                .WithLearnStartDate(learnStartDateTime)
                .WithPlannedEndDate(plannedEndDateTime);

            var approvalsExtract = ApprovalsExtractFactory.ForIlrAndStandard(ilr, standard123V1_0, now);

            using (var fixture = new PopulateLearnerTestsFixture()
                .WithStandard(standard123V1_0)
                .WithStandard(standard456V1_0)
                .WithStandard(standard456V1_1)
                .WithIlr(ilr)
                .WithApprovalsExtract(approvalsExtract))
            {
                var results = await fixture.PopulateLearner();

                var expected = LearnerFactory.From(ilr, standard456V1_0)
                    .WithVersion(expectedVersion)
                    .WithVersionConfirmed(0)
                    .WithStandardUId(expectedStandardUid)
                    .WithLastUpdated(now.Date)
                    .WithLatestApprovals(null);

                results.VerifyUpdated(1);
                await results.VerifyLearnerRowCount(1);
                await results.VerifyLearnerExists(expected);
            }
        }

        [Test]
        public async Task PopulateLearner_WhenMatchingApprovalsExtract_AndIlrHasDateOfBirth_ThenLearnerDateOfBirthIsPopulatedFromIlr()
        {
            var now = DateTime.Now;
            var dateOfBirthFromIlr = new DateTime(2000, 3, 7, 0, 0, 0, DateTimeKind.Utc);
            
            var standard = StandardFactory.Create(
                title: "Standard",
                referenceNumber: "ST0001",
                larsCode: 123,
                version: "1.0")
                .WithEffectiveFrom(now.AddYears(-1).Date)
                .WithVersionApprovedForDelivery(now.AddMonths(-1).Date)
                .WithVersionEarliestStartDate(now.Date);

            var ilr = IlrFactory.ForStandard(standard, now)
                .WithDateOfBirth(dateOfBirthFromIlr);

            var approvalsExtract = ApprovalsExtractFactory.ForIlrAndStandard(ilr, standard, now);

            using (var fixture = new PopulateLearnerTestsFixture()
                .WithStandard(standard)
                .WithIlr(ilr)
                .WithApprovalsExtract(approvalsExtract))
            {
                var results = await fixture.PopulateLearner();

                var expected = LearnerFactory.From(ilr, approvalsExtract, standard)
                    .WithDateOfBirth(dateOfBirthFromIlr);

                results.VerifyUpdated(1);
                await results.VerifyLearnerRowCount(1);
                await results.VerifyLearnerExists(expected);
            }
        }

        [Test]
        public async Task PopulateLearner_WhenNoMatchingApprovalsExtract_AndIlrHasDateOfBirth_ThenLearnerDateOfBirthIsPopulatedFromIlr()
        {
            var now = DateTime.Now;
            var dateOfBirthFromIlr = new DateTime(2000, 3, 7, 0, 0, 0, DateTimeKind.Utc);

            var standard = StandardFactory.Create(
               title: "Standard",
               referenceNumber: "ST0001",
               larsCode: 123,
               version: "1.0")
               .WithEffectiveFrom(now.AddYears(-1).Date)
               .WithVersionApprovedForDelivery(now.AddMonths(-1).Date)
               .WithVersionEarliestStartDate(now.Date);

            var ilr = IlrFactory.ForStandard(standard, now)
                .WithDateOfBirth(dateOfBirthFromIlr);

            using (var fixture = new PopulateLearnerTestsFixture()
                .WithStandard(standard)
                .WithIlr(ilr))
            {
                var results = await fixture.PopulateLearner();

                var expected = LearnerFactory.From(ilr, standard)
                    .WithDateOfBirth(dateOfBirthFromIlr);

                results.VerifyUpdated(1);
                await results.VerifyLearnerRowCount(1);
                await results.VerifyLearnerExists(expected);
            }
        }

        private class PopulateLearnerTestsFixture : FixtureBase<PopulateLearnerTestsFixture>, IDisposable
        {
            private readonly DatabaseService _databaseService = new DatabaseService();
            private readonly SqlConnection _sqlConnection;

            private readonly ApprovalsExtractRepository _repository;
            private readonly Mock<IRoatpApiClient> _roatpApiClient;
            private readonly Mock<ILogger<ApprovalsExtractRepository>> _logger;
            
            private string _versionFromLarsCode;
            private string _standardUidFromLarsCode;
            private int _updated;

            public PopulateLearnerTestsFixture()
            {
                _sqlConnection = new SqlConnection(_databaseService.SqlConnectionStringTest);
                _roatpApiClient = new Mock<IRoatpApiClient>();
                _logger = new Mock<ILogger<ApprovalsExtractRepository>>();
                _repository = new ApprovalsExtractRepository(new UnitOfWork(_sqlConnection), _roatpApiClient.Object, _logger.Object);
            }

            public async Task<PopulateLearnerTestsFixture> PopulateLearner()
            {
                _updated = await _repository.PopulateLearner();
                return this;
            }

            public async Task<PopulateLearnerTestsFixture> GetVersionFromLarsCode(DateTime startDate, int stdCode)
            {
                _versionFromLarsCode = await _databaseService.QueryFirstOrDefaultAsync<string>("SELECT dbo.GetVersionFromLarsCode(@StartDate, @StdCode)", new {StartDate = startDate, StdCode = stdCode});
                return this;
            }

            public async Task<PopulateLearnerTestsFixture> GetStandardUidFromLarsCode(DateTime startDate, int stdCode)
            {
                _standardUidFromLarsCode = await _databaseService.QueryFirstOrDefaultAsync<string>("SELECT dbo.GetStandardUidFromLarsCode(@StartDate, @StdCode)", new { StartDate = startDate, StdCode = stdCode });
                return this;
            }

            public PopulateLearnerTestsFixture VerifyVersionFromLarsCode(string version)
            {
                _versionFromLarsCode.Should().Be(version);
                return this;
            }

            public PopulateLearnerTestsFixture VerifyStandardUidFromLarsCode(string standardUid)
            {
                _standardUidFromLarsCode.Should().Be(standardUid);
                return this;
            }

            public PopulateLearnerTestsFixture VerifyUpdated(int updated)
            {
                _updated.Should().Be(updated);
                return this;
            }

            protected override void Dispose(bool disposing)
            {
            }

            public void Dispose()
            {
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }
    }
}
