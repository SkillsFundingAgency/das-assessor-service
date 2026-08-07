using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Data.IntegrationTests.Factories;
using SFA.DAS.AssessorService.Data.IntegrationTests.Repositories;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Repositories.CertificateRepositoryTests
{
    public class When_Executing_Certificate_Mask_StoredProcedures : TestBase
    {
        [Test]
        public async Task Certificates_GetStandardMasks_Excludes_Masks_Matching_The_Excluded_Certificate()
        {
            const long excludedUln = 2000000001;
            var createdAt = DateTime.UtcNow;

            using (var fixture = new CertificateMaskStoredProceduresFixture()
                .WithOrganisation("Excluded organisation", "EPA_EXCLUDED", 99999999, null)
                .WithOrganisation("Mask organisation", "EPA_MASK", 88888888, null)
                .WithMaskStandard("Excluded standard", "ST0001", 1001, "Route 1")
                .WithMaskStandard("Mask standard", "ST0002", 1002, "Route 2")
                .WithStandardMaskCertificate(
                    "EPA_EXCLUDED", excludedUln, 1001, 99999999, "ST0001_1.0",
                    "Excluded standard", "Excluded provider", createdAt)
                .WithStandardMaskCertificate(
                    "EPA_MASK", 2000000002, 1002, 88888888, "ST0002_1.0",
                    "Mask standard", "Mask provider", createdAt.AddMinutes(1)))
            {
                var results = await fixture.ExecuteStandardMasks(excludedUln.ToString());

                results.Should().ContainSingle();
                results.Single().Should().BeEquivalentTo(new CertificateMask
                {
                    CertificateType = CertificateTypes.Standard,
                    CourseCode = "1002",
                    CourseName = "Mask standard",
                    CourseLevel = "3",
                    ProviderName = "Mask provider"
                });
            }
        }

        [Test]
        public async Task Certificates_GetFrameworkMasks_Excludes_Masks_Matching_The_Excluded_Certificate()
        {
            const long excludedUln = 3000000001;
            var createdAt = DateTime.UtcNow;

            using (var fixture = new CertificateMaskStoredProceduresFixture()
                .WithFrameworkMaskCertificate(
                    excludedUln, "FW100", "Excluded framework", "Excluded provider", createdAt)
                .WithFrameworkMaskCertificate(
                    3000000002, "FW200", "Mask framework", "Mask provider", createdAt.AddMinutes(1)))
            {
                var results = await fixture.ExecuteFrameworkMasks(excludedUln.ToString());

                results.Should().ContainSingle();
                results.Single().Should().BeEquivalentTo(new CertificateMask
                {
                    CertificateType = CertificateTypes.Framework,
                    CourseCode = "FW200",
                    CourseName = "Mask framework",
                    CourseLevel = "Level 3",
                    ProviderName = "Mask provider"
                });
            }
        }

        private class CertificateMaskStoredProceduresFixture
            : FixtureBase<CertificateMaskStoredProceduresFixture>, IDisposable
        {
            private readonly DatabaseService _databaseService = new DatabaseService();
            private int _certificateReferenceId = 10001;

            public CertificateMaskStoredProceduresFixture WithMaskStandard(
                string title,
                string reference,
                int larsCode,
                string route)
            {
                var standard = StandardFactory.Create(title, reference, larsCode, "1.0");
                standard.Route = route;
                return WithStandard(standard);
            }

            public CertificateMaskStoredProceduresFixture WithStandardMaskCertificate(
                string organisationId,
                long uln,
                int standardCode,
                int providerUkprn,
                string standardUId,
                string standardName,
                string providerName,
                DateTime createdAt)
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
                        certificate.CreatedAt = createdAt;
                        certificate.CertificateReferenceId = _certificateReferenceId++;
                        certificate.DateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    },
                    certificateData =>
                    {
                        certificateData.LearnerGivenNames = "Test";
                        certificateData.LearnerFamilyName = "Learner";
                        certificateData.StandardName = standardName;
                        certificateData.StandardLevel = 3;
                        certificateData.ProviderName = providerName;
                        certificateData.AchievementDate = createdAt.Date;
                        certificateData.EpaDetails = new EpaDetails
                        {
                            LatestEpaOutcome = EpaOutcome.Pass
                        };
                    });
            }

            public CertificateMaskStoredProceduresFixture WithFrameworkMaskCertificate(
                long uln,
                string courseCode,
                string courseName,
                string providerName,
                DateTime createdAt)
            {
                return WithFrameworkLearner(
                    Guid.NewGuid(),
                    Guid.NewGuid().ToString("N").Substring(0, 10),
                    createdAt.Year.ToString(),
                    createdAt.Date,
                    "Test Learner",
                    "Learner",
                    "Test",
                    new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    uln,
                    courseCode,
                    courseName,
                    "Pathway",
                    3,
                    providerName,
                    "12345678",
                    courseName,
                    "Pathway",
                    "Level 3",
                    uln,
                    createdAt,
                    "test learner");
            }

            public Task<List<CertificateMask>> ExecuteStandardMasks(string excludeUlns)
            {
                return _databaseService.ExecuteStoredProcedure<CertificateMask>(
                    "Certificates_GetStandardMasks",
                    new { ExcludeUlns = excludeUlns, Top = 5 });
            }

            public Task<List<CertificateMask>> ExecuteFrameworkMasks(string excludeUlns)
            {
                return _databaseService.ExecuteStoredProcedure<CertificateMask>(
                    "Certificates_GetFrameworkMasks",
                    new { ExcludeUlns = excludeUlns, Top = 5 });
            }
        }
    }
}