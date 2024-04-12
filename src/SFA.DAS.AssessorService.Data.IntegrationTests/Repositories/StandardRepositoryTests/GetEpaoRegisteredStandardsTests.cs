using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Repositories.StandardRepositoryTests
{
    public class GetEpaoRegisteredStandardsTests : TestBase
    {
        private static DateTime DefaultEffectiveFrom = DateTime.Today.AddDays(-50);

        [Test]
        public async Task GetEpaoRegisteredStandards_ReturnsOrganisationStandardsWithVersions_WhenRequireAtLeastOneVersionIsDefault()
        {
            using (var fixture = new GetEpaoRegisteredStandardsTestsFixture()
                .WithStandard("BrickLayer", "ST0001", 101, "1.0", DefaultEffectiveFrom, null)
                .WithStandard("Bricklayer", "ST0001", 101, "1.1", DefaultEffectiveFrom, null)
                .WithOrganisation("Brick & Co", "EPA0001", 123456, null)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", null, null, null)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", null, null)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.1", null, null)
                .WithStandard("Roofer", "ST0002", 102, "1.0", DefaultEffectiveFrom, null)
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", null, null, null)) // no versions are opted in
            {
                (await fixture.GetEpaoRegisteredStandards("EPA0001", true)).Verify(1);
            }
        }

        [Test]
        public async Task GetEpaoRegisteredStandards_ReturnsOrganisationStandardsWithVersions_WhenRequireAtLeastOneVersionIsTrue()
        {
            using (var fixture = new GetEpaoRegisteredStandardsTestsFixture()
                .WithStandard("BrickLayer", "ST0001", 101, "1.0", DefaultEffectiveFrom, null)
                .WithStandard("Bricklayer", "ST0001", 101, "1.1", DefaultEffectiveFrom, null)
                .WithOrganisation("Brick & Co", "EPA0001", 123456, null)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", null, null, null)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", null, null)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.1", null, null)
                .WithStandard("Roofer", "ST0002", 102, "1.0", DefaultEffectiveFrom, null)
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", null, null, null)) // no versions are opted in
            {
                (await fixture.GetEpaoRegisteredStandards("EPA0001", true)).Verify(1);
            }
        }

        [Test]
        public async Task GetEpaoRegisteredStandards_ReturnsOrganisationStandardsWithOrWithoutVersions_WhenRequireAtLeastOneVersionIsFalse()
        {
            using (var fixture = new GetEpaoRegisteredStandardsTestsFixture()
                .WithStandard("BrickLayer", "ST0001", 101, "1.0", DefaultEffectiveFrom, null)
                .WithStandard("Bricklayer", "ST0001", 101, "1.1", DefaultEffectiveFrom, null)
                .WithOrganisation("Brick & Co", "EPA0001", 123456, null)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", null, null, null)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", null, null)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.1", null, null)
                .WithStandard("Roofer", "ST0002", 102, "1.0", DefaultEffectiveFrom, null)
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", null, null, null)) // no versions are opted in
            {
                (await fixture.GetEpaoRegisteredStandards("EPA0001", false)).Verify(2);
            }
        }

        [Test]
        public async Task GetEpaoRegisteredStandards_ReturnsOrganisationStandardsNotRemoved_WhenRequireAtLeastOneVersionIsDefault()
        {
            using (var fixture = new GetEpaoRegisteredStandardsTestsFixture()
                .WithStandard("BrickLayer", "ST0001", 101, "1.0", DefaultEffectiveFrom, null)
                .WithStandard("Bricklayer", "ST0001", 101, "1.1", DefaultEffectiveFrom, null)
                .WithOrganisation("Brick & Co", "EPA0001", 123456, null)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", null, DateTime.Today, null) // standard has been removed
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", null, null)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.1", null, null)
                .WithStandard("Roofer", "ST0002", 102, "1.0", DefaultEffectiveFrom, null)
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", null, null, null)) // no versions are opted in
            {
                (await fixture.GetEpaoRegisteredStandards("EPA0001")).Verify(0);
            }
        }

        [Test]
        public async Task GetEpaoRegisteredStandards_ReturnsOrganisationStandardsNotRemoved_WhenRequireAtLeastOneVersionIsTrue()
        {
            using (var fixture = new GetEpaoRegisteredStandardsTestsFixture()
                .WithStandard("BrickLayer", "ST0001", 101, "1.0", DefaultEffectiveFrom, null)
                .WithStandard("Bricklayer", "ST0001", 101, "1.1", DefaultEffectiveFrom, null)
                .WithOrganisation("Brick & Co", "EPA0001", 123456, null)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", null, DateTime.Today, null) // standard has been removed
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", null, null)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.1", null, null)
                .WithStandard("Roofer", "ST0002", 102, "1.0", DefaultEffectiveFrom, null)
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", null, null, null)) // no versions are opted in
            {
                (await fixture.GetEpaoRegisteredStandards("EPA0001", true)).Verify(0);
            }
        }

        [Test]
        public async Task GetEpaoRegisteredStandards_ReturnsOrganisationStandardsNotRemoved_WhenRequireAtLeastOneVersionIsFalse()
        {
            using (var fixture = new GetEpaoRegisteredStandardsTestsFixture()
                .WithStandard("BrickLayer", "ST0001", 101, "1.0", DefaultEffectiveFrom, null)
                .WithStandard("Bricklayer", "ST0001", 101, "1.1", DefaultEffectiveFrom, null)
                .WithOrganisation("Brick & Co", "EPA0001", 123456, null)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", null, DateTime.Today, null) // standard has been removed
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", null, null)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.1", null, null)
                .WithStandard("Roofer", "ST0002", 102, "1.0", DefaultEffectiveFrom, null)
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", null, null, null)) // no versions are opted in
            {
                (await fixture.GetEpaoRegisteredStandards("EPA0001", false)).Verify(1);
            }
        }

        [Test]
        public async Task GetEpaoRegisteredStandards_ReturnsOrganisationStandardsWithVersionsWithVersionsOptedIn_WhenRequireAtLeastOneVersionIsDefault()
        {
            using (var fixture = new GetEpaoRegisteredStandardsTestsFixture()
                .WithStandard("BrickLayer", "ST0001", 101, "1.0", DefaultEffectiveFrom, null)
                .WithStandard("Bricklayer", "ST0001", 101, "1.1", DefaultEffectiveFrom, null)
                .WithOrganisation("Brick & Co", "EPA0001", 123456, null)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", null, null, null)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", null, null)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.1", null, null)
                .WithStandard("Roofer", "ST0002", 102, "1.0", DefaultEffectiveFrom, null)
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", null, null, null)
                .WithOrganisationStandardVersion("EPA0001", 102, "ST0002", "1.0", null, DateTime.Today)) // version is opted out
            {
                (await fixture.GetEpaoRegisteredStandards("EPA0001")).Verify(1);
            }
        }

        [Test]
        public async Task GetEpaoRegisteredStandards_ReturnsOrganisationStandardsWithVersionsWithVersionsOptedIn_WhenRequireAtLeastOneVersionIsTrue()
        {
            using (var fixture = new GetEpaoRegisteredStandardsTestsFixture()
                .WithStandard("BrickLayer", "ST0001", 101, "1.0", DefaultEffectiveFrom, null)
                .WithStandard("Bricklayer", "ST0001", 101, "1.1", DefaultEffectiveFrom, null)
                .WithOrganisation("Brick & Co", "EPA0001", 123456, null)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", null, null, null)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", null, null)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.1", null, null)
                .WithStandard("Roofer", "ST0002", 102, "1.0", DefaultEffectiveFrom, null)
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", null, null, null)
                .WithOrganisationStandardVersion("EPA0001", 102, "ST0002", "1.0", null, DateTime.Today)) // version is opted out
            {
                (await fixture.GetEpaoRegisteredStandards("EPA0001", true)).Verify(1);
            }
        }

        [Test]
        public async Task GetEpaoRegisteredStandards_ReturnsOrganisationStandardsWithVersionsOptedInOrOut_WhenRequireAtLeastOneVersionIsFalse()
        {
            using (var fixture = new GetEpaoRegisteredStandardsTestsFixture()
                .WithStandard("BrickLayer", "ST0001", 101, "1.0", DefaultEffectiveFrom, null)
                .WithStandard("Bricklayer", "ST0001", 101, "1.1", DefaultEffectiveFrom, null)
                .WithOrganisation("Brick & Co", "EPA0001", 123456, null)
                .WithOrganisationStandard(1, "EPA0001", 101, "ST0001", null, null, null)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.0", null, null)
                .WithOrganisationStandardVersion("EPA0001", 101, "ST0001", "1.1", null, null)
                .WithStandard("Roofer", "ST0002", 102, "1.0", DefaultEffectiveFrom, null)
                .WithOrganisationStandard(2, "EPA0001", 102, "ST0002", null, null, null)
                .WithOrganisationStandardVersion("EPA0001", 102, "ST0002", "1.0", null, DateTime.Today)) // version is opted out
            {
                (await fixture.GetEpaoRegisteredStandards("EPA0001", false)).Verify(2);
            }
        }

        private class GetEpaoRegisteredStandardsTestsFixture : FixtureBase<GetEpaoRegisteredStandardsTestsFixture>, IDisposable
        {
            private readonly DatabaseService _databaseService = new DatabaseService();
            private readonly SqlConnection _sqlConnection;

            private StandardRepository _repository;
            private EpoRegisteredStandardsResult _result;

            public GetEpaoRegisteredStandardsTestsFixture()
            {
                _sqlConnection = new SqlConnection(_databaseService.SqlConnectionStringTest);
                _repository = new StandardRepository(new UnitOfWork(_sqlConnection));

                // this is to workaround the other tests which are not clearing up after themselves properly
                DeleteAllRecords();
            }

            public async Task<GetEpaoRegisteredStandardsTestsFixture> GetEpaoRegisteredStandards(string endpointAssessmentOrganisationId)
            {
                _result = await _repository.GetEpaoRegisteredStandards(endpointAssessmentOrganisationId, 10, 1);
                return this;
            }

            public async Task<GetEpaoRegisteredStandardsTestsFixture> GetEpaoRegisteredStandards(string endpointAssessmentOrganisationId, bool requireAtLeastOneVersion)
            {
                _result = await _repository.GetEpaoRegisteredStandards(endpointAssessmentOrganisationId, requireAtLeastOneVersion, 10, 1);
                return this;
            }

            public void Verify(int numberOfResults)
            {
                Assert.That(_result.PageOfResults.Count, Is.EqualTo(numberOfResults));
            }
        }
    }
}
