using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.DTOs.Certificate;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Repositories.CertificateRepositoryTests
{
    public class GetAssessmentsTests : TestBase
    {
        [TestCase(-1, -1)]
        [TestCase(-2, -2)]
        [TestCase(1, 0)]
        [TestCase(2, 0)]
        public async Task GetAssessments_ReturnsEarliestAssessment_WhenCalled(int achievementDateMonthsOffset, int expectedEarliestAssessmentOffset)
        {
            var currentDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 15, 0, 0, 0, DateTimeKind.Utc);

            using (var fixture = new GetAssessmentsTestsFixture()
                .WithOrganisation("Organisation1", "EPA0001", 123456, null)
                .WithCertificate(Guid.NewGuid(), currentDateTime, 11111, 101, "EPA0001", CertificateStatus.Printed, 10001000, "ST0010", currentDateTime)
                .WithCertificate(Guid.NewGuid(), currentDateTime, 22222, 101, "EPA0001", CertificateStatus.Printed, 10001000, "ST0010", currentDateTime.AddMonths(achievementDateMonthsOffset)))
            {
                await fixture
                    .GetAssessments(10001000, "ST0010");
                    
                fixture.VerifyResult(currentDateTime.AddMonths(expectedEarliestAssessmentOffset), 2);
            }
        }

        [Test]
        public async Task GetAssessments_DoesNotIncludeDeleted_WhenCalled()
        {
            var currentDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 15, 0, 0, 0, DateTimeKind.Utc);

            using (var fixture = new GetAssessmentsTestsFixture()
                .WithOrganisation("Organisation1", "EPA0001", 123456, null)
                .WithCertificate(Guid.NewGuid(), currentDateTime, 11111, 101, "EPA0001", CertificateStatus.Deleted, 10001000, "ST0010", currentDateTime))
            {
                await fixture
                    .GetAssessments(10001000, "ST0010");

                fixture.VerifyResult(null, 0);
            }
        }

        [Test]
        public async Task GetAssessments_DoesNotIncludeDraft_WhenCalled()
        {
            var currentDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 15, 0, 0, 0, DateTimeKind.Utc);

            using (var fixture = new GetAssessmentsTestsFixture()
                .WithOrganisation("Organisation1", "EPA0001", 123456, null)
                .WithCertificate(Guid.NewGuid(), currentDateTime, 11111, 101, "EPA0001", CertificateStatus.Draft, 10001000, "ST0010", currentDateTime))
            {
                await fixture
                    .GetAssessments(10001000, "ST0010");

                fixture.VerifyResult(null, 0);
            }
        }

        [TestCase("ST0010", 0, 1)]
        [TestCase("ST0011", null, 0)]
        public async Task GetAssessments_DoesFilterByStandardReference_WhenCalled(string standardReference, int? expectedEarliestAssessmentOffset, int expectedEndpointAssessmentCount)
        {
            var currentDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 15, 0, 0, 0, DateTimeKind.Utc);

            using (var fixture = new GetAssessmentsTestsFixture()
                .WithOrganisation("Organisation1", "EPA0001", 123456, null)
                .WithCertificate(Guid.NewGuid(), currentDateTime, 11111, 101, "EPA0001", CertificateStatus.Printed, 10001000, "ST0010", currentDateTime))
            {
                await fixture
                    .GetAssessments(10001000, standardReference);

                fixture.VerifyResult(
                    expectedEarliestAssessmentOffset != null ? currentDateTime.AddMonths(expectedEarliestAssessmentOffset.Value) : null, 
                    expectedEndpointAssessmentCount);
            }
        }

        [TestCase(10001000, 0, 1)]
        [TestCase(20002000, null, 0)]
        public async Task GetAssessments_DoesFilterByProviderUkprn_WhenCalled(long providerUkprn, int? expectedEarliestAssessmentOffset, int expectedEndpointAssessmentCount)
        {
            var currentDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 15, 0, 0, 0, DateTimeKind.Utc);

            using (var fixture = new GetAssessmentsTestsFixture()
                .WithOrganisation("Organisation1", "EPA0001", 123456, null)
                .WithCertificate(Guid.NewGuid(), currentDateTime, 11111, 101, "EPA0001", CertificateStatus.Printed, 10001000, "ST0010", currentDateTime))
            {
                await fixture
                    .GetAssessments(providerUkprn, "ST0010");

                fixture.VerifyResult(
                    expectedEarliestAssessmentOffset != null ? currentDateTime.AddMonths(expectedEarliestAssessmentOffset.Value) : null,
                    expectedEndpointAssessmentCount);
            }
        }

        [TestCase(false, 0, 1)]
        [TestCase(true, null, 0)]
        public async Task GetAssessments_DoesFilterByIsPrivatelyFunded_WhenCalled(bool isPrivatelyFunded, int? expectedEarliestAssessmentOffset, int expectedEndpointAssessmentCount)
        {
            var currentDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 15, 0, 0, 0, DateTimeKind.Utc);

            using (var fixture = new GetAssessmentsTestsFixture()
                .WithOrganisation("Organisation1", "EPA0001", 123456, null)
                .WithCertificate(Guid.NewGuid(), currentDateTime, 11111, 101, "EPA0001", CertificateStatus.Printed, 10001000, "ST0010", currentDateTime, isPrivatelyFunded))
            {
                await fixture
                    .GetAssessments(10001000, "ST0010");

                fixture.VerifyResult(
                    expectedEarliestAssessmentOffset != null ? currentDateTime.AddMonths(expectedEarliestAssessmentOffset.Value) : null,
                    expectedEndpointAssessmentCount);
            }
        }

        private class GetAssessmentsTestsFixture : FixtureBase<GetAssessmentsTestsFixture>, IDisposable
        {
            private readonly DatabaseService _databaseService = new DatabaseService();
            private readonly SqlConnection _sqlConnection;
            private readonly CertificateRepository _repository;

            private AssessmentsResult _result;

            public GetAssessmentsTestsFixture()
            {
                _sqlConnection = new SqlConnection(_databaseService.SqlConnectionStringTest);
                _repository = new CertificateRepository(
                    new UnitOfWork(_sqlConnection), 
                    new AssessorDbContext(_sqlConnection, new DbContextOptionsBuilder<AssessorDbContext>().Options));
            }

            public async Task<GetAssessmentsTestsFixture> GetAssessments(long ukprn, string standardReference)
            {
                _result = await _repository.GetAssessments(ukprn, standardReference);
                return this;
            }

            public void VerifyResult(DateTime? earliestAssessment, int endpointAssessmentCount)
            {
                _result.EarliestAssessment.Should().Be(earliestAssessment);
                _result.EndpointAssessmentCount.Should().Be(endpointAssessmentCount);
            }

            public override void Dispose()
            {
                Dispose(true);
            }

            protected virtual void Dispose(bool disposing)
            {
            }
        }
    }
}
