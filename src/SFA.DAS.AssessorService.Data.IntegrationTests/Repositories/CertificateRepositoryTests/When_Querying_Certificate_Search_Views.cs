using System;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.CompaniesHouse;
using SFA.DAS.AssessorService.Data.IntegrationTests.Factories;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Repositories.CertificateRepositoryTests
{
    public class When_Querying_Certificate_Search_Views : TestBase
    {
        private static readonly DateTime ApprenticeDateOfBirth =
            new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        [Test]
        public void StandardCertificateSearchView_Returns_The_Searchable_Certificate()
        {
            const long uln = 2000000001;
            var dateAwarded = new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc);

            using (var fixture = new CertificateSearchViewsFixture()
                .WithOrganisation(
                    "Test organisation",
                    "EPA0001",
                    12345678,
                    null)
                .WithSearchStandard(
                    "Test standard",
                    "ST0001",
                    1001,
                    "Test route")
                .WithSearchableStandardCertificate(
                    "EPA0001",
                    uln,
                    1001,
                    87654321,
                    "ST0001_1.0",
                    "Smith",
                    "Test standard",
                    3,
                    "Test provider",
                    dateAwarded))
            {
                var result = fixture.GetStandardCertificate(uln);

                result.Should().BeEquivalentTo(
                    new CertificateSearchViewResult
                    {
                        CertificateFamilyName = "Smith",
                        DateOfBirth = ApprenticeDateOfBirth,
                        Uln = uln,
                        CourseCode = "1001",
                        CourseName = "Test standard",
                        CourseLevel = "3",
                        DateAwarded = dateAwarded,
                        ProviderName = "Test provider",
                        Ukprn = "87654321"
                    });
            }
        }

        [TestCase(CertificateStatus.Draft)]
        [TestCase(CertificateStatus.Deleted)]
        public void StandardCertificateSearchView_Does_Not_Return_An_Ineligible_Certificate(
            string status)
        {
            const long uln = 2000000001;

            using (var fixture = new CertificateSearchViewsFixture()
                .WithOrganisation(
                    "Test organisation",
                    "EPA0001",
                    12345678,
                    null)
                .WithSearchStandard(
                    "Test standard",
                    "ST0001",
                    1001,
                    "Test route")
                .WithSearchableStandardCertificate(
                    "EPA0001",
                    uln,
                    1001,
                    87654321,
                    "ST0001_1.0",
                    "Smith",
                    "Test standard",
                    3,
                    "Test provider",
                    new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                    certificate => certificate.Status = status))
            {
                fixture.GetStandardCertificate(uln)
                    .Should()
                    .BeNull();
            }
        }

        [Test]
        public void StandardCertificateSearchView_Does_Not_Return_A_Non_Standard_Certificate()
        {
            const long uln = 2000000001;

            using (var fixture = new CertificateSearchViewsFixture()
                .WithOrganisation(
                    "Test organisation",
                    "EPA0001",
                    12345678,
                    null)
                .WithSearchStandard(
                    "Test standard",
                    "ST0001",
                    1001,
                    "Test route")
                .WithSearchableFrameworkCertificate(
                    "EPA0001",
                    uln,
                    "Smith",
                    "FW100",
                    "Test framework",
                    "Test pathway",
                    "Level 3",
                    "Test provider",
                    "12345678",
                    new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc)))
            {
                fixture.GetStandardCertificate(uln)
                    .Should()
                    .BeNull();
            }
        }

        [Test]
        public void StandardCertificateSearchView_Does_Not_Return_A_Certificate_Without_A_Pass_Outcome()
        {
            const long uln = 2000000001;

            using (var fixture = new CertificateSearchViewsFixture()
                .WithOrganisation(
                    "Test organisation",
                    "EPA0001",
                    12345678,
                    null)
                .WithSearchStandard(
                    "Test standard",
                    "ST0001",
                    1001,
                    "Test route")
                .WithSearchableStandardCertificate(
                    "EPA0001",
                    uln,
                    1001,
                    87654321,
                    "ST0001_1.0",
                    "Smith",
                    "Test standard",
                    3,
                    "Test provider",
                    new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                    certificateDataOverrides: certificateData =>
                        certificateData.EpaDetails.LatestEpaOutcome =
                            EpaOutcome.Fail))
            {
                fixture.GetStandardCertificate(uln)
                    .Should()
                    .BeNull();
            }
        }

        [TestCase(1000000000)]
        [TestCase(9999999999)]
        [TestCase(999999999)]
        [TestCase(10000000000)]
        public void StandardCertificateSearchView_Does_Not_Return_A_Certificate_With_An_Out_Of_Range_Uln(
            long uln)
        {
            using (var fixture = new CertificateSearchViewsFixture()
                .WithOrganisation(
                    "Test organisation",
                    "EPA0001",
                    12345678,
                    null)
                .WithSearchStandard(
                    "Test standard",
                    "ST0001",
                    1001,
                    "Test route")
                .WithSearchableStandardCertificate(
                    "EPA0001",
                    uln,
                    1001,
                    87654321,
                    "ST0001_1.0",
                    "Smith",
                    "Test standard",
                    3,
                    "Test provider",
                    new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc)))
            {
                fixture.GetStandardCertificate(uln)
                    .Should()
                    .BeNull();
            }
        }

        [Test]
        public void StandardCertificateSearchView_Does_Not_Return_A_Certificate_Without_A_Date_Of_Birth()
        {
            const long uln = 2000000001;

            using (var fixture = new CertificateSearchViewsFixture()
                .WithOrganisation(
                    "Test organisation",
                    "EPA0001",
                    12345678,
                    null)
                .WithSearchStandard(
                    "Test standard",
                    "ST0001",
                    1001,
                    "Test route")
                .WithSearchableStandardCertificate(
                    "EPA0001",
                    uln,
                    1001,
                    87654321,
                    "ST0001_1.0",
                    "Smith",
                    "Test standard",
                    3,
                    "Test provider",
                    new DateTime(2024, 1, 2, 0, 0, 0, DateTimeKind.Utc),
                    certificate => certificate.DateOfBirth = null))
            {
                fixture.GetStandardCertificate(uln)
                    .Should()
                    .BeNull();
            }
        }

        [Test]
        public void FrameworkCertificateSearchView_Returns_The_Framework_Learner()
        {
            const long uln = 3000000001;
            var dateAwarded = new DateTime(2023, 2, 3, 0, 0, 0, DateTimeKind.Utc);

            using (var fixture = new CertificateSearchViewsFixture()
                .WithSearchableFrameworkLearner(
                    uln,
                    "Smith",
                    "FW100",
                    "Test framework",
                    "Test pathway",
                    "Level 3",
                    "Test provider",
                    "12345678",
                    dateAwarded))
            {
                var result = fixture.GetFrameworkCertificate(uln);

                result.Should().BeEquivalentTo(
                    new CertificateSearchViewResult
                    {
                        CertificateFamilyName = "Smith",
                        DateOfBirth = ApprenticeDateOfBirth,
                        Uln = uln,
                        CourseCode = "FW100",
                        CourseName = "Test framework",
                        CourseLevel = "Level 3",
                        DateAwarded = dateAwarded,
                        ProviderName = "Test provider",
                        Ukprn = "12345678"
                    });
            }
        }

        [Test]
        public void FrameworkCertificateSearchView_Does_Not_Return_A_Framework_Learner_Without_A_Uln()
        {
            using (var fixture = new CertificateSearchViewsFixture()
                .WithOrganisation(
                    "Test organisation",
                    "EPA0001",
                    12345678,
                    null)
                .WithSearchableFrameworkCertificate(
                    "EPA0001",
                    null,
                    "Smith",
                    "FW100",
                    "Test framework",
                    "Test pathway",
                    "Level 3",
                    "Test provider",
                    "12345678",
                    new DateTime(2024, 1, 2)))
            {
                fixture.GetFrameworkCertificateWithoutUln()
                    .Should()
                    .BeNull();
            }
        }

        private class CertificateSearchViewsFixture
            : FixtureBase<CertificateSearchViewsFixture>, IDisposable
        {
            private readonly DatabaseService _databaseService =
                new DatabaseService();

            private int _certificateReferenceId = 10001;

            public CertificateSearchViewsFixture WithSearchStandard(
                string title,
                string reference,
                int larsCode,
                string route)
            {
                var standard = StandardFactory.Create(
                    title,
                    reference,
                    larsCode,
                    "1.0");

                standard.Route = route;

                return WithStandard(standard);
            }

            public CertificateSearchViewsFixture WithSearchableStandardCertificate(
                string organisationId,
                long uln,
                int standardCode,
                int providerUkprn,
                string standardUId,
                string familyName,
                string standardName,
                int standardLevel,
                string providerName,
                DateTime dateAwarded,
                Action<CertificateModel> overrides = null,
                Action<CertificateData> certificateDataOverrides = null)
            {
                return WithCertificate(
                    organisationId,
                    certificate =>
                    {
                        certificate.Uln = uln;
                        certificate.StandardCode = standardCode;
                        certificate.ProviderUkPrn = providerUkprn;
                        certificate.StandardUId = standardUId;
                        certificate.Status = CertificateStatus.Printed;
                        certificate.CertificateReferenceId =
                            _certificateReferenceId++;
                        certificate.DateOfBirth = ApprenticeDateOfBirth;

                        overrides?.Invoke(certificate);
                    },
                    certificateData =>
                    {
                        certificateData.LearnerGivenNames = "Test";
                        certificateData.LearnerFamilyName = familyName;
                        certificateData.StandardName = standardName;
                        certificateData.StandardLevel = standardLevel;
                        certificateData.ProviderName = providerName;
                        certificateData.AchievementDate = dateAwarded;
                        certificateData.EpaDetails = new EpaDetails
                        {
                            LatestEpaOutcome = EpaOutcome.Pass
                        };

                        certificateDataOverrides?.Invoke(certificateData);
                    });
            }

            public CertificateSearchViewsFixture WithSearchableFrameworkCertificate(
                string organisationId,
                long? uln,
                string familyName,
                string courseCode,
                string courseName,
                string pathwayName,
                string courseLevel,
                string providerName,
                string ukprn,
                DateTime dateAwarded,
                Action<CertificateModel> overrides = null,
                Action<CertificateData> certificateDataOverrides = null)
            {
                var frameworkLearnerId = Guid.NewGuid();
                var frameworkCertificateNumber =
                    Guid.NewGuid().ToString("N").Substring(0, 10);

                WithFrameworkLearner(
                    frameworkLearnerId,
                    frameworkCertificateNumber,
                    dateAwarded.Year.ToString(),
                    dateAwarded,
                    $"Test {familyName}",
                    familyName,
                    "Test",
                    ApprenticeDateOfBirth,
                    uln,
                    courseCode,
                    courseName,
                    pathwayName,
                    3,
                    providerName,
                    ukprn,
                    courseName,
                    pathwayName,
                    courseLevel,
                    uln ?? 3000000001,
                    DateTime.UtcNow,
                    $"test {familyName.ToLowerInvariant()} {uln ?? 3000000001}");

                return WithCertificate(
                    organisationId,
                    certificate =>
                    {
                        certificate.Type = CertificateTypes.Framework;
                        certificate.FrameworkLearnerId = frameworkLearnerId;
                        certificate.Uln = null;
                        certificate.StandardCode = null;
                        certificate.ProviderUkPrn = null;
                        certificate.StandardUId = null;
                        certificate.Status = CertificateStatus.Printed;
                        certificate.CertificateReferenceId =
                            _certificateReferenceId++;
                        certificate.DateOfBirth = ApprenticeDateOfBirth;

                        overrides?.Invoke(certificate);
                    },
                    certificateData =>
                    {
                        certificateData.LearnerGivenNames = "Test";
                        certificateData.LearnerFamilyName = familyName;
                        certificateData.ProviderName = providerName;
                        certificateData.AchievementDate = dateAwarded;

                        certificateDataOverrides?.Invoke(certificateData);
                    });
            }

            public CertificateSearchViewsFixture WithSearchableFrameworkLearner(
                long uln,
                string familyName,
                string courseCode,
                string courseName,
                string pathwayName,
                string courseLevel,
                string providerName,
                string ukprn,
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
                    ApprenticeDateOfBirth,
                    uln,
                    courseCode,
                    courseName,
                    pathwayName,
                    3,
                    providerName,
                    ukprn,
                    courseName,
                    pathwayName,
                    courseLevel,
                    uln,
                    DateTime.UtcNow,
                    $"test {familyName.ToLowerInvariant()}");
            }

            public CertificateSearchViewResult GetStandardCertificate(long uln)
            {
                return _databaseService.Get<CertificateSearchViewResult>(
                    $"SELECT * FROM [dbo].[StandardCertificateSearchView] " +
                    $"WHERE [Uln] = {uln}");
            }

            public CertificateSearchViewResult GetFrameworkCertificate(long uln)
            {
                return _databaseService.Get<CertificateSearchViewResult>(
                    $"SELECT * FROM [dbo].[FrameworkCertificateSearchView] " +
                    $"WHERE [Uln] = {uln}");
            }

            public CertificateSearchViewResult GetFrameworkCertificateWithoutUln()
            {
                return _databaseService.Get<CertificateSearchViewResult>(
                    "SELECT * FROM [dbo].[FrameworkCertificateSearchView] " +
                    "WHERE [Uln] IS NULL");
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

        private class CertificateSearchViewResult
        {
            public string CertificateFamilyName { get; set; }

            public DateTime DateOfBirth { get; set; }

            public long Uln { get; set; }

            public string CourseCode { get; set; }

            public string CourseName { get; set; }

            public string CourseLevel { get; set; }

            public DateTime? DateAwarded { get; set; }

            public string ProviderName { get; set; }

            public string Ukprn { get; set; }
        }
    }
}