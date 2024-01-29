using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.IntegrationTests.Handlers;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.Roatp;
using SFA.DAS.AssessorService.TestHelper;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

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
        [TestCase(10, "1.3")]
        [TestCase(100, "1.3")]
        public async Task GetVersionFromLarsCode_WhenCalled_GetsCorrectVersionForLearnStartDate(int learnStartDateOffset, string expectedVersion)
        {
            var currentDateTime = DateTime.Now;
            var learnStartDateTime = currentDateTime.AddDays(learnStartDateOffset);

            using (var fixture = new PopulateLearnerTestsFixture()
                .WithStandard("Standard 1", "ST0001", 123, "1.0", currentDateTime.AddYears(-1).Date, null, null, currentDateTime.AddDays(-40).Date, null)
                .WithStandard("Standard 1", "ST0001", 123, "1.1", currentDateTime.AddYears(-1).Date, null, currentDateTime.AddDays(-39).Date, currentDateTime.AddDays(-30).Date, null)
                .WithStandard("Standard 1", "ST0001", 123, "1.2", currentDateTime.AddYears(-1).Date, null, currentDateTime.AddDays(-29).Date, currentDateTime.AddDays(-20).Date, currentDateTime.AddDays(10).Date)
                .WithStandard("Standard 1", "ST0001", 123, "1.3", currentDateTime.AddYears(-1).Date, null, currentDateTime.Date.AddDays(-19), null, null))
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
        [TestCase(10, "ST0001_1.3")]
        [TestCase(100, "ST0001_1.3")]
        public async Task GetStandardUidFromLarsCode_WhenCalled_GetsCorrectStandardUidForLearnStartDate(int learnStartDateOffset, string expectedStandardUid)
        {
            var currentDateTime = DateTime.Now;
            var learnStartDateTime = currentDateTime.AddDays(learnStartDateOffset);

            using (var fixture = new PopulateLearnerTestsFixture()
                .WithStandard("Standard 1", "ST0001", 123, "1.0", currentDateTime.AddYears(-1).Date, null, null, currentDateTime.AddDays(-40).Date, null)
                .WithStandard("Standard 1", "ST0001", 123, "1.1", currentDateTime.AddYears(-1).Date, null, currentDateTime.AddDays(-39).Date, currentDateTime.AddDays(-30).Date, null)
                .WithStandard("Standard 1", "ST0001", 123, "1.2", currentDateTime.AddYears(-1).Date, null, currentDateTime.AddDays(-29).Date, currentDateTime.AddDays(-20).Date, currentDateTime.AddDays(10).Date)
                .WithStandard("Standard 1", "ST0001", 123, "1.3", currentDateTime.AddYears(-1).Date, null, currentDateTime.Date.AddDays(-19), null, null))
            {
                var results = await fixture.GetStandardUidFromLarsCode(learnStartDateTime, 123);
                results.VerifyStandardUidFromLarsCode(expectedStandardUid);
            }
        }

        [TestCase(-45, "1.0", "ST0002_1.0")]
        [TestCase(-40, "1.0", "ST0002_1.0")]
        [TestCase(-39, "1.1", "ST0002_1.1")]
        [TestCase(-35, "1.1", "ST0002_1.1")]
        [TestCase(-30, "1.1", "ST0002_1.1")]
        [TestCase(-29, "1.2", "ST0002_1.2")]
        [TestCase(-25, "1.2", "ST0002_1.2")]
        [TestCase(-20, "1.2", "ST0002_1.2")]
        [TestCase(-19, "1.3", "ST0002_1.3")]
        [TestCase(10, "1.3", "ST0002_1.3")]
        [TestCase(100, "1.3", "ST0002_1.3")]
        public async Task PopulatedLearner_WhenCalled_AndIlrStandardDoesNotMatchApprovalsStandard_ThenVersionAndStandardUidAreFromIlr(int learnStartDateOffset, string expectedVersion, string expectedStandardUid)
        {
            var currentDateTime = DateTime.Now;
            var plannedEndDateTime = currentDateTime.AddMonths(12);
            var learnStartDateTime = currentDateTime.AddDays(learnStartDateOffset);

            using (var fixture = new PopulateLearnerTestsFixture()
                .WithStandard("Standard 1", "ST0001", 123, "1.0", currentDateTime.AddYears(-1).Date, null, null, null, null)
                .WithStandard("Standard 2", "ST0002", 456, "1.0", currentDateTime.AddYears(-1).Date, null, null, currentDateTime.AddDays(-40).Date, null)
                .WithStandard("Standard 2", "ST0002", 456, "1.1", currentDateTime.AddYears(-1).Date, null, currentDateTime.AddDays(-39).Date, currentDateTime.AddDays(-30).Date, null)
                .WithStandard("Standard 2", "ST0002", 456, "1.2", currentDateTime.AddYears(-1).Date, null, currentDateTime.AddDays(-29).Date, currentDateTime.AddDays(-20).Date, currentDateTime.AddDays(10).Date)
                .WithStandard("Standard 2", "ST0002", 456, "1.3", currentDateTime.AddYears(-1).Date, null, currentDateTime.Date.AddDays(-19), null, null)
                .WithIlr(Guid.NewGuid(), 123456789, "Chris", "Woodcock", 12345678, 456, learnStartDateTime, null, currentDateTime, 2, plannedEndDateTime)
                .WithApprovalsExtract(12345, "Chris", "Woodcock", "123456789", 123, "1.0", false, null, "ST0001_1.0", currentDateTime, null, currentDateTime, currentDateTime, null, null, null, 12345678, "LEARN123", 1, 12345, "Bob"))
            {
                var results = await fixture.PopulateLearner();

                var expected = LearnerHandler.Create(null, 123456789, "Chris", "Woodcock", 12345678, 456, learnStartDateTime,
                    null, 36, 12345, "2324", null, 2, plannedEndDateTime, null, null, null, null, null, null, expectedVersion, 0, null,
                    expectedStandardUid, "ST0002", "Standard 2", currentDateTime.Date, plannedEndDateTime.Date.GetEndOfMonth(),
                    null, null, null, null, currentDateTime, null, null, null, 0, null);

                results.VerifyUpdated(1);
                await results.VerifyLearnerRowCount(1);
                await results.VerifyLearnerExists(expected);
            }
        }

        private class PopulateLearnerTestsFixture : FixtureBase<PopulateLearnerTestsFixture>, IDisposable
        {
            private readonly DatabaseService _databaseService = new DatabaseService();
            private readonly SqlConnection _sqlConnection;

            private ApprovalsExtractRepository _repository;
            private Mock<IRoatpApiClient> _roatpApiClient;
            private Mock<ILogger<ApprovalsExtractRepository>> _logger;
            
            private string _versionFromLarsCode;
            private string _standardUidFromLarsCode;
            private int _updated;

            public PopulateLearnerTestsFixture()
            {
                _sqlConnection = new SqlConnection(_databaseService.WebConfiguration.SqlConnectionString);
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
        }
    }
}
