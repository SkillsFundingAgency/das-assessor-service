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
using System.Reflection.Metadata;
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
        public async Task PopulatedLearnerTest(int learnStartDateOffset, string expectedVersion)
        {
            var currentDateTime = DateTime.Now;
            var plannedEndDateTime = currentDateTime.AddMonths(12);
            var learnStartDateTime = currentDateTime.AddDays(learnStartDateOffset);

            using (var fixture = new PopulateLearnerTestsFixture()
                .WithStandard("Standard 1", "ST0001", 123, "1.0", currentDateTime.AddYears(-1).Date, null, null, currentDateTime.AddDays(-40).Date, null)
                .WithStandard("Standard 1", "ST0001", 123, "1.1", currentDateTime.AddYears(-1).Date, null, currentDateTime.AddDays(-39).Date, currentDateTime.AddDays(-30).Date, null)
                .WithStandard("Standard 1", "ST0001", 123, "1.2", currentDateTime.AddYears(-1).Date, null, currentDateTime.AddDays(-29).Date, currentDateTime.AddDays(-20).Date, currentDateTime.AddDays(10).Date)
                .WithStandard("Standard 1", "ST0001", 123, "1.3", currentDateTime.AddYears(-1).Date, null, currentDateTime.Date.AddDays(-19), null, null)
                .WithIlr(Guid.NewGuid(), 123456789, "Chris", "Woodcock", 12345678, 123, learnStartDateTime, null, currentDateTime, 2, plannedEndDateTime)
                .WithApprovalsExtract(12345, "Chris", "Woodcock", "123456789", 123, "1.0", false, null, "ST0001_1.0", currentDateTime, null, currentDateTime, currentDateTime, null, null, null, 12345678, "LEARN123", 1, 12345, "Bob"))
            {
                var results = await fixture.PopulateLearner();

                var expected = LearnerHandler.Create(null, 123456789, "Chris", "Woodcock", 12345678, 123, learnStartDateTime,
                    null, 36, 12345, "2324+App", null, 2, plannedEndDateTime, null, null, null, null, null, null, expectedVersion, 0, null,
                    "ST0001_1.0", "ST0001", "Standard 1", currentDateTime.Date, plannedEndDateTime.Date.GetEndOfMonth(), 
                    null, null, null, 1, currentDateTime, currentDateTime, 12345, "Bob", 0, null);

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
            public int _updated;

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

            public PopulateLearnerTestsFixture VerifyUpdated(int updated)
            {
                _updated.Should().Be(updated);
                return this;
            }
        }
    }
}
