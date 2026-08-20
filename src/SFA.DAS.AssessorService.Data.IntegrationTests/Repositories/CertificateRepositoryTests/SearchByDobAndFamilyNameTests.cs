using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Data.IntegrationTests.Factories;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Repositories.CertificateRepositoryTests
{
    public class SearchByDobAndFamilyNameTests : TestBase
    {
        private static readonly DateTime DateOfBirth = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        [TestCase(false)]
        [TestCase(true)]
        public async Task SearchByDobAndFamilyName_Returns_Standard_And_Framework_Matches_When_No_Ulns_Are_Excluded(
            bool useNullExclusions)
        {
            const long standardUln = 2000000001;
            const long frameworkUln = 3000000001;

            using (var fixture = new SearchByDobAndFamilyNameFixture()
                .WithDefaultOrganisationAndStandard()
                .WithSearchableStandardCertificate(
                    standardUln,
                    "Smith",
                    DateOfBirth,
                    new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc))
                .WithSearchableFrameworkCertificate(
                    frameworkUln,
                    "Smith",
                    DateOfBirth,
                    new DateTime(2023, 2, 3, 0, 0, 0, DateTimeKind.Utc)))
            {
                var exclusions = useNullExclusions ? null : Array.Empty<long>();

                var results = await fixture.Search(DateOfBirth, "Smith", exclusions);

                results.Select(x => x.Uln).Should().Equal(standardUln, frameworkUln);
            }
        }

        [Test]
        public async Task SearchByDobAndFamilyName_Excludes_Multiple_Ulns_Across_Standard_And_Framework_Matches()
        {
            const long excludedStandardUln = 2000000001;
            const long expectedStandardUln = 2000000002;
            const long excludedFrameworkUln = 3000000001;
            const long expectedFrameworkUln = 3000000002;

            using (var fixture = new SearchByDobAndFamilyNameFixture()
                .WithDefaultOrganisationAndStandard()
                .WithSearchableStandardCertificate(excludedStandardUln, "Smith", DateOfBirth, new DateTime(2024, 1, 4, 0, 0, 0, DateTimeKind.Utc))
                .WithSearchableStandardCertificate(expectedStandardUln, "Smith", DateOfBirth, new DateTime(2024, 1, 3, 0, 0, 0, DateTimeKind.Utc))
                .WithSearchableFrameworkCertificate(excludedFrameworkUln, "Smith", DateOfBirth, new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc))
                .WithSearchableFrameworkCertificate(expectedFrameworkUln, "Smith", DateOfBirth, new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)))
            {
                var results = await fixture.Search(
                    DateOfBirth,
                    "Smith",
                    new[] { excludedStandardUln, excludedFrameworkUln });

                results.Select(x => x.Uln).Should().Equal(expectedStandardUln, expectedFrameworkUln);
            }
        }

        [Test]
        public async Task SearchByDobAndFamilyName_Matches_A_Cleansed_Family_Name_Ignoring_Case()
        {
            const long standardUln = 2000000001;
            const long frameworkUln = 3000000001;

            using (var fixture = new SearchByDobAndFamilyNameFixture()
                .WithDefaultOrganisationAndStandard()
                .WithSearchableStandardCertificate(standardUln, "Smith", DateOfBirth, new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc))
                .WithSearchableFrameworkCertificate(frameworkUln, "Smith", DateOfBirth, new DateTime(2023, 2, 3, 0, 0, 0, DateTimeKind.Utc)))
            {
                var results = await fixture.Search(DateOfBirth, "  sMiTh  ", null);

                results.Select(x => x.Uln).Should().Equal(standardUln, frameworkUln);
            }
        }

        [Test]
        public async Task SearchByDobAndFamilyName_Returns_Only_Certificates_Matching_The_Date_Of_Birth_And_Family_Name()
        {
            const long expectedStandardUln = 2000000001;
            const long wrongDateOfBirthUln = 2000000002;
            const long expectedFrameworkUln = 3000000001;
            const long wrongFamilyNameUln = 3000000002;

            using (var fixture = new SearchByDobAndFamilyNameFixture()
                .WithDefaultOrganisationAndStandard()
                .WithSearchableStandardCertificate(expectedStandardUln, "Smith", DateOfBirth, new DateTime(2024, 1, 4, 0, 0, 0, DateTimeKind.Utc))
                .WithSearchableStandardCertificate(wrongDateOfBirthUln, "Smith", DateOfBirth.AddDays(1), new DateTime(2024, 1, 3, 0, 0, 0, DateTimeKind.Utc))
                .WithSearchableFrameworkCertificate(expectedFrameworkUln, "Smith", DateOfBirth, new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc))
                .WithSearchableFrameworkCertificate(wrongFamilyNameUln, "Jones", DateOfBirth, new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)))
            {
                var results = await fixture.Search(DateOfBirth, "Smith", null);

                results.Select(x => x.Uln).Should().Equal(expectedStandardUln, expectedFrameworkUln);
            }
        }

        [Test]
        public async Task SearchByDobAndFamilyName_Orders_Standard_And_Framework_Matches_By_Date_Awarded_Descending()
        {
            const long oldestStandardUln = 2000000001;
            const long newestStandardUln = 2000000002;
            const long middleFrameworkUln = 3000000001;

            using (var fixture = new SearchByDobAndFamilyNameFixture()
                .WithDefaultOrganisationAndStandard()
                .WithSearchableStandardCertificate(oldestStandardUln, "Smith", DateOfBirth, new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .WithSearchableStandardCertificate(newestStandardUln, "Smith", DateOfBirth, new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                .WithSearchableFrameworkCertificate(middleFrameworkUln, "Smith", DateOfBirth, new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc)))
            {
                var results = await fixture.Search(DateOfBirth, "Smith", null);

                results.Select(x => x.Uln).Should().Equal(
                    newestStandardUln,
                    middleFrameworkUln,
                    oldestStandardUln);
            }
        }

        private class SearchByDobAndFamilyNameFixture
            : FixtureBase<SearchByDobAndFamilyNameFixture>, IDisposable
        {
            private const string OrganisationId = "EPA0001";
            private const int StandardCode = 1001;
            private const string StandardUId = "ST0001_1.0";
            private readonly CertificateRepository _sut;
            private int _certificateReferenceId = 10001;

            public SearchByDobAndFamilyNameFixture()
            {
                var databaseService = new DatabaseService();
                _sut = new CertificateRepository(new AssessorUnitOfWork(databaseService.TestContext));
            }

            public SearchByDobAndFamilyNameFixture WithDefaultOrganisationAndStandard()
            {
                WithOrganisation("Test organisation", OrganisationId, 12345678, null);

                var standard = StandardFactory.Create("Test standard", "ST0001", StandardCode, "1.0");
                standard.Route = "Test route";
                return WithStandard(standard);
            }

            public SearchByDobAndFamilyNameFixture WithSearchableStandardCertificate(
                long uln,
                string familyName,
                DateTime dateOfBirth,
                DateTime dateAwarded)
            {
                return WithCertificate(
                    OrganisationId,
                    certificate =>
                    {
                        certificate.Uln = uln;
                        certificate.StandardCode = StandardCode;
                        certificate.ProviderUkPrn = 87654321;
                        certificate.StandardUId = StandardUId;
                        certificate.Status = CertificateStatus.Printed;
                        certificate.CertificateReferenceId = _certificateReferenceId++;
                        certificate.DateOfBirth = dateOfBirth;
                    },
                    certificateData =>
                    {
                        certificateData.LearnerGivenNames = "Test";
                        certificateData.LearnerFamilyName = familyName;
                        certificateData.StandardName = "Test standard";
                        certificateData.StandardLevel = 3;
                        certificateData.ProviderName = "Test standard provider";
                        certificateData.AchievementDate = dateAwarded;
                        certificateData.EpaDetails = new EpaDetails
                        {
                            LatestEpaOutcome = EpaOutcome.Pass
                        };
                    });
            }

            public SearchByDobAndFamilyNameFixture WithSearchableFrameworkCertificate(
                long uln,
                string familyName,
                DateTime dateOfBirth,
                DateTime dateAwarded)
            {
                return WithFrameworkLearner(
                    Guid.NewGuid(),
                    Guid.NewGuid().ToString("N").Substring(0, 10),
                    dateAwarded.Year.ToString(),
                    dateAwarded,
                    $"Test {familyName}",
                    familyName,
                    "Test",
                    dateOfBirth,
                    uln,
                    "FW100",
                    "Test framework",
                    "Test pathway",
                    3,
                    "Test framework provider",
                    "12345678",
                    "Test framework",
                    "Test pathway",
                    "Level 3",
                    uln,
                    dateAwarded,
                    $"test learner {uln}");
            }

            public Task<List<SearchCertificatesResponse>> Search(
                DateTime dateOfBirth,
                string familyName,
                IEnumerable<long> excludeUlns)
            {
                return _sut.SearchByDobAndFamilyName(dateOfBirth, familyName, excludeUlns);
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