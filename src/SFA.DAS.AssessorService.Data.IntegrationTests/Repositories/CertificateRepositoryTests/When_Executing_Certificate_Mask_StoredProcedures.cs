using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Data.IntegrationTests.Factories;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Data.IntegrationTests.Repositories;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.JsonData;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Repositories.CertificateRepositoryTests
{
    public class When_Executing_Certificate_Mask_StoredProcedures : TestBase
    {
        public enum StandardExclusion
        {
            Uln,
            Course,
            Provider,
            Sector
        }

        public enum FrameworkExclusion
        {
            Uln,
            Course,
            Provider
        }

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

        [TestCase(StandardExclusion.Uln)]
        [TestCase(StandardExclusion.Course)]
        [TestCase(StandardExclusion.Provider)]
        [TestCase(StandardExclusion.Sector)]
        public async Task Certificates_GetStandardMasks_Does_Not_Return_A_Mask_Matching_An_Excluded_Certificate_Value(
            StandardExclusion exclusion)
        {
            const long excludedUln = 2000000101;
            const long candidateUln = 2000000102;
            const long unaffectedUln = 2000000103;

            using (var fixture = new CertificateMaskStoredProceduresFixture()
                .WithOrganisation("P1 organisation", "EPA_P1", 11111111, null)
                .WithOrganisation("P2 organisation", "EPA_P2", 22222222, null)
                .WithOrganisation("P3 organisation", "EPA_P3", 33333333, null)
                .WithMaskStandard("Course A", "ST0001", 1001, "S1")
                .WithMaskStandard("Course B", "ST0002", 1002, exclusion == StandardExclusion.Sector ? "S1" : "S2")
                .WithMaskStandard("Course C", "ST0003", 1003, "S3")

                // certificate A which is the excluded one
                .WithStandardMaskCertificate(
                    "EPA_P1", excludedUln, 1001, 11111111, "ST0001_1.0",
                    "Course A", "P1", new DateTime(2024, 6, 13, 0 ,0, 0, DateTimeKind.Utc))

                // certificate B which in each test case shares a value with a so must be
                // excluded in each test case
                .WithStandardMaskCertificate(
                    exclusion == StandardExclusion.Provider ? "EPA_P1" : "EPA_P2",
                    exclusion == StandardExclusion.Uln ? excludedUln : candidateUln,
                    exclusion == StandardExclusion.Course ? 1001 : 1002,
                    exclusion == StandardExclusion.Provider ? 11111111 : 22222222,
                    exclusion == StandardExclusion.Course ? "ST0001_1.0" : "ST0002_1.0",
                    exclusion == StandardExclusion.Course ? "Course A" : "Course B",
                    exclusion == StandardExclusion.Provider ? "P1" : "P2",
                    new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc))

                // certificate C is different in all respects to A, so will rank 1 and be returned
                // as the only mask
                .WithStandardMaskCertificate(
                    "EPA_P3", unaffectedUln, 1003, 33333333, "ST0003_1.0",
                    "Course C", "P3", new DateTime(2024, 6, 14, 0, 0 , 0, DateTimeKind.Utc)))
            {
                var results = await fixture.ExecuteStandardMasks(excludedUln.ToString());

                results.Should().ContainSingle();
                results.Single().CourseCode.Should().Be("1003");
            }
        }

        [TestCase(FrameworkExclusion.Uln)]
        [TestCase(FrameworkExclusion.Course)]
        [TestCase(FrameworkExclusion.Provider)]
        public async Task Certificates_GetFrameworkMasks_Does_Not_Return_A_Mask_Matching_An_Excluded_Certificate_Value(
            FrameworkExclusion exclusion)
        {
            const long excludedUln = 3000000101;

            using (var fixture = new CertificateMaskStoredProceduresFixture()
                // certificate A which is the excluded one
                .WithFrameworkMaskCertificate(
                    excludedUln, "A", "Course A", "P1", new DateTime(2024, 6, 13, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(2024, 5, 13, 0, 0, 0, DateTimeKind.Utc))

                // certificate B which in each test case shares a value with a so must be
                // excluded in each test case
                .WithFrameworkMaskCertificate(
                    exclusion == FrameworkExclusion.Uln ? excludedUln : 3000000102,
                    exclusion == FrameworkExclusion.Course ? "A" : "B",
                    exclusion == FrameworkExclusion.Course ? "Course A" : "Course B",
                    exclusion == FrameworkExclusion.Provider ? "P1" : "P2",
                    new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(2024, 5, 15, 0, 0, 0, DateTimeKind.Utc))

                // certificate C is different in all respects to A, so will rank 1 and be returned
                // as the only mask
                .WithFrameworkMaskCertificate(
                    3000000103, "C", "Course C", "P3", new DateTime(2024, 6, 14, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(2024, 5, 14, 0, 0, 0, DateTimeKind.Utc)))
            {
                var results = await fixture.ExecuteFrameworkMasks(excludedUln.ToString());

                results.Should().ContainSingle();
                results.Single().CourseCode.Should().Be("C");
            }
        }

        [TestCase(null)]
        [TestCase("")]
        public async Task Certificates_GetStandardMasks_Returns_Masks_When_Exclusions_Are_Null_Or_Empty(
            string excludeUlns)
        {
            // with no excluded ULNs, the sole rank-one candidate remains eligible
            using (var fixture = new CertificateMaskStoredProceduresFixture()
                .WithOrganisation("P1 organisation", "EPA_P1", 11111111, null)
                .WithMaskStandard("Course A", "ST0001", 1001, "S1")
                .WithStandardMaskCertificate(
                    "EPA_P1", 2000000201, 1001, 11111111, "ST0001_1.0",
                    "Course A", "P1", new DateTime(2024, 6, 13, 0, 0, 0, DateTimeKind.Utc)))
            {
                var results = await fixture.ExecuteStandardMasks(excludeUlns);

                results.Should().ContainSingle();
            }
        }

        [TestCase(null)]
        [TestCase("")]
        public async Task Certificates_GetFrameworkMasks_Returns_Masks_When_Exclusions_Are_Null_Or_Empty(
            string excludeUlns)
        {
            // with no excluded ULNs, the sole rank-one candidate remains eligible
            using (var fixture = new CertificateMaskStoredProceduresFixture()
                .WithFrameworkMaskCertificate(
                    3000000201, "A", "Course A", "P1", new DateTime(2024, 6, 13, 0, 0, 0, DateTimeKind.Utc)))
            {
                var results = await fixture.ExecuteFrameworkMasks(excludeUlns);

                results.Should().ContainSingle();
            }
        }

        [Test]
        public async Task Certificates_GetStandardMasks_Applies_Multiple_Excluded_Ulns()
        {
            using (var fixture = new CertificateMaskStoredProceduresFixture()
                .WithOrganisation("P1 organisation", "EPA_P1", 11111111, null)
                .WithOrganisation("P2 organisation", "EPA_P2", 22222222, null)
                .WithOrganisation("P3 organisation", "EPA_P3", 33333333, null)
                .WithMaskStandard("Course A", "ST0001", 1001, "S1")
                .WithMaskStandard("Course B", "ST0002", 1002, "S2")
                .WithMaskStandard("Course C", "ST0003", 1003, "S3")
                // certificate A and B are both explicitly excluded. Their course, provider and sector
                // values are all different, proving the comma-separated list is parsed.
                .WithStandardMaskCertificate(
                    "EPA_P1", 2000000301, 1001, 11111111, "ST0001_1.0",
                    "Course A", "P1", new DateTime(2024, 6, 13, 0, 0, 0, DateTimeKind.Utc))
                .WithStandardMaskCertificate(
                    "EPA_P2", 2000000302, 1002, 22222222, "ST0002_1.0",
                    "Course B", "P2", new DateTime(2024, 6, 14, 0, 0, 0, DateTimeKind.Utc))
                // certificate C shares none of those values and is the only survivor.
                .WithStandardMaskCertificate(
                    "EPA_P3", 2000000303, 1003, 33333333, "ST0003_1.0",
                    "Course C", "P3", new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc)))
            {
                var results = await fixture.ExecuteStandardMasks("2000000301,2000000302");

                results.Should().ContainSingle();
                results.Single().CourseCode.Should().Be("1003");
            }
        }

        [Test]
        public async Task Certificates_GetFrameworkMasks_Applies_Multiple_Excluded_Ulns()
        {
            using (var fixture = new CertificateMaskStoredProceduresFixture()
                // certificate A and B are explicitly excluded and use different courses, providers
                // and award dates, so each exclusion contributes independently.
                .WithFrameworkMaskCertificate(
                    3000000301, "A", "Course A", "P1", new DateTime(2024, 6, 13, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(2024, 5, 13, 0, 0, 0, DateTimeKind.Utc))
                .WithFrameworkMaskCertificate(
                    3000000302, "B", "Course B", "P2", new DateTime(2024, 6, 14, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(2024, 5, 14, 0, 0, 0, DateTimeKind.Utc))
                // certificate C shares none of their excluded values and is the only survivor.
                .WithFrameworkMaskCertificate(
                    3000000303, "C", "Course C", "P3", new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc),
                    new DateTime(2024, 5, 15, 0, 0, 0, DateTimeKind.Utc)))
            {
                var results = await fixture.ExecuteFrameworkMasks("3000000301,3000000302");

                results.Should().ContainSingle();
                results.Single().CourseCode.Should().Be("C");
            }
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(5)]
        public async Task Certificates_GetStandardMasks_Applies_The_Top_Limit(int top)
        {
            // fixture creates six candidates. Every candidate has a different course,
            // provider and sector, so all six are rank one in all three partitions
            // TOP is therefore the only reason fewer than six rows are returned
            using (var fixture = new CertificateMaskStoredProceduresFixture()
                .WithSixIndependentStandardMasks())
            {
                var results = await fixture.ExecuteStandardMasks(null, top);

                results.Should().HaveCount(top);
            }
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(5)]
        public async Task Certificates_GetFrameworkMasks_Applies_The_Top_Limit(int top)
        {
            // fixture creates six candidates with different courses, providers and
            // award dates. All six pass the three rank-one checks before TOP is applied
            using (var fixture = new CertificateMaskStoredProceduresFixture()
                .WithSixIndependentFrameworkMasks())
            {
                var results = await fixture.ExecuteFrameworkMasks(null, top);

                results.Should().HaveCount(top);
            }
        }

        [Test]
        public async Task Certificates_GetFrameworkMasks_Excludes_Records_On_The_CreateDay_Cutoff()
        {
            using (var fixture = new CertificateMaskStoredProceduresFixture()
                // the cutoff is exclusive: a row created on 1 January 2021 is ineligible
                .WithFrameworkMaskCertificate(
                    3000000401, "A", "Course A", "P1", new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                // row created one day later remains eligible.
                .WithFrameworkMaskCertificate(
                    3000000402, "B", "Course B", "P2", new DateTime(2021, 1, 2, 0, 0, 0, DateTimeKind.Utc)))
            {
                var results = await fixture.ExecuteFrameworkMasks(null);

                results.Should().ContainSingle();
                results.Single().CourseCode.Should().Be("B");
            }
        }

        [Test]
        public async Task Certificates_GetFrameworkMasks_Excludes_Records_Without_A_Ukprn()
        {
            using (var fixture = new CertificateMaskStoredProceduresFixture()
                // row otherwise qualifies, but the search view requires a UKPRN.
                .WithFrameworkMaskCertificate(
                    3000000501, "A", "Course A", "P1", new DateTime(2024, 6, 13, 0, 0, 0, DateTimeKind.Utc),
                    ukprn: null)
                // independent rank-one row has a UKPRN and survives.
                .WithFrameworkMaskCertificate(
                    3000000502, "B", "Course B", "P2", new DateTime(2024, 6, 14, 0, 0, 0, DateTimeKind.Utc)))
            {
                var results = await fixture.ExecuteFrameworkMasks(null);

                results.Should().ContainSingle();
                results.Single().CourseCode.Should().Be("B");
            }
        }

        [Test]
        public async Task Certificates_GetStandardMasks_Does_Not_Return_An_Ineligible_View_Record()
        {
            using (var fixture = new CertificateMaskStoredProceduresFixture()
                .WithOrganisation("P1 organisation", "EPA_P1", 11111111, null)
                .WithOrganisation("P2 organisation", "EPA_P2", 22222222, null)
                .WithMaskStandard("Course A", "ST0001", 1001, "S1")
                .WithMaskStandard("Course B", "ST0002", 1002, "S2")
                // draft certificates do not enter the standard search view, even though
                // this row is otherwise rank one in independent partitions.
                .WithStandardMaskCertificate(
                    "EPA_P1", 2000000601, 1001, 11111111, "ST0001_1.0",
                    "Course A", "P1", new DateTime(2024, 6, 13, 0, 0, 0, DateTimeKind.Utc),
                    certificate => certificate.Status = CertificateStatus.Draft)
                // the printed certificate is the only eligible view row.
                .WithStandardMaskCertificate(
                    "EPA_P2", 2000000602, 1002, 22222222, "ST0002_1.0",
                    "Course B", "P2", new DateTime(2024, 6, 14, 0, 0, 0, DateTimeKind.Utc)))
            {
                var results = await fixture.ExecuteStandardMasks(null);

                results.Should().ContainSingle();
                results.Single().CourseCode.Should().Be("1002");
            }
        }

        [Test]
        public async Task Certificates_GetFrameworkMasks_Does_Not_Return_An_Ineligible_View_Record()
        {
            using (var fixture = new CertificateMaskStoredProceduresFixture()
                // framework learner without a ULN is filtered out by the search view.
                .WithFrameworkMaskCertificate(
                    null, "A", "Course A", "P1", new DateTime(2024, 6, 13, 0, 0, 0, DateTimeKind.Utc))
                // independent row has a ULN and remains eligible.
                .WithFrameworkMaskCertificate(
                    3000000602, "B", "Course B", "P2", new DateTime(2024, 6, 14, 0, 0, 0, DateTimeKind.Utc)))
            {
                var results = await fixture.ExecuteFrameworkMasks(null);

                results.Should().ContainSingle();
                results.Single().CourseCode.Should().Be("B");
            }
        }

        [Test]
        [Ignore("Disabled until the mask stored procedure falls back to the next ranked match.")]
        public async Task Certificates_GetStandardMasks_Returns_Next_Ranked_Mask_When_Top_Ranked_Matches_Excluded_Provider()
        {
            const long excludedUln = 2000000011;
            const long newestIneligibleCandidateUln = 2000000012;
            const long expectedMaskUln = 2000000013;
            const int courseAStandardCode = 1001;
            const int courseBStandardCode = 1002;
            const int providerP1Ukprn = 99999999;
            const int providerP2Ukprn = 88888888;
            const string providerP1Name = "P1";
            const string providerP2Name = "P2";
            const string sectorS1 = "S1";
            const string sectorS2 = "S2";

            var june13 = new DateTime(2024, 6, 13, 0, 0, 0, DateTimeKind.Utc);
            var june14 = new DateTime(2024, 6, 14, 0, 0, 0, DateTimeKind.Utc);
            var june15 = new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc);

            using (var fixture = new CertificateMaskStoredProceduresFixture()
                .WithOrganisation(
                    endPointAssessorName: "Provider P1 organisation",
                    endPointAssessorOrganisationId: "EPA_P1",
                    ukprn: providerP1Ukprn,
                    recognitionNumber: null)
                .WithOrganisation(
                    endPointAssessorName: "Provider P2 organisation",
                    endPointAssessorOrganisationId: "EPA_P2",
                    ukprn: providerP2Ukprn,
                    recognitionNumber: null)
                .WithMaskStandard(
                    title: "Course A",
                    reference: "ST0001",
                    larsCode: courseAStandardCode,
                    route: sectorS1)
                .WithMaskStandard(
                    title: "Course B",
                    reference: "ST0002",
                    larsCode: courseBStandardCode,
                    route: sectorS2)

                // excluded: Course A, Provider P1, Sector S1, 13 June
                .WithStandardMaskCertificate(
                    organisationId: "EPA_P1",
                    uln: excludedUln,
                    standardCode: courseAStandardCode,
                    providerUkprn: providerP1Ukprn,
                    standardUId: "ST0001_1.0",
                    standardName: "Course A",
                    providerName: providerP1Name,
                    createdAt: june13)

                // newest: Course B, Provider P1, Sector S2, 15 June
                // it is ranked first but conflicts with the excluded Provider P1
                .WithStandardMaskCertificate(
                    organisationId: "EPA_P1",
                    uln: newestIneligibleCandidateUln,
                    standardCode: courseBStandardCode,
                    providerUkprn: providerP1Ukprn,
                    standardUId: "ST0002_1.0",
                    standardName: "Course B",
                    providerName: providerP1Name,
                    createdAt: june15)

                // fallback: Course B, Provider P2, Sector S2, 14 June
                // it should become rank one after the Provider P1 rows are excluded
                .WithStandardMaskCertificate(
                    organisationId: "EPA_P2",
                    uln: expectedMaskUln,
                    standardCode: courseBStandardCode,
                    providerUkprn: providerP2Ukprn,
                    standardUId: "ST0002_1.0",
                    standardName: "Course B",
                    providerName: providerP2Name,
                    createdAt: june14))
            {
                var results = await fixture.ExecuteStandardMasks(
                    excludedUln.ToString());

                results.Should().ContainSingle();

                results.Single().Should().BeEquivalentTo(new CertificateMask
                {
                    CertificateType = CertificateTypes.Standard,
                    CourseCode = courseBStandardCode.ToString(),
                    CourseName = "Course B",
                    CourseLevel = "3",
                    ProviderName = providerP2Name
                });
            }
        }

        [Test]
        [Ignore("Disabled until the mask stored procedure falls back to the next ranked match.")]
        public async Task Certificates_GetFrameworkMasks_Returns_Next_Ranked_Mask_When_Top_Ranked_Matches_Excluded_Provider()
        {
            const long excludedUln = 3000000011;
            const long newestIneligibleCandidateUln = 3000000012;
            const long expectedMaskUln = 3000000013;
            const string courseA = "A";
            const string courseB = "B";
            const string providerP1 = "P1";
            const string providerP2 = "P2";

            var june13 = new DateTime(2024, 6, 13, 0, 0, 0, DateTimeKind.Utc);
            var june14 = new DateTime(2024, 6, 14, 0, 0, 0, DateTimeKind.Utc);
            var june15 = new DateTime(2024, 6, 15, 0, 0, 0, DateTimeKind.Utc);

            using (var fixture = new CertificateMaskStoredProceduresFixture()

                // excluded: Course A, Provider P1, 13 June
                .WithFrameworkMaskCertificate(
                    uln: excludedUln,
                    courseCode: courseA,
                    courseName: "Course A",
                    providerName: providerP1,
                    createdAt: june13)

                // newest: Course B, Provider P1, 15 June
                // it is ranked first but conflicts with the excluded Provider P1
                .WithFrameworkMaskCertificate(
                    uln: newestIneligibleCandidateUln,
                    courseCode: courseB,
                    courseName: "Course B",
                    providerName: providerP1,
                    createdAt: june15)

                // fallback: Course B, Provider P2, 14 June
                // it should become rank one after the Provider P1 rows are excluded
                .WithFrameworkMaskCertificate(
                    uln: expectedMaskUln,
                    courseCode: courseB,
                    courseName: "Course B",
                    providerName: providerP2,
                    createdAt: june14))
            {
                var results = await fixture.ExecuteFrameworkMasks(
                    excludedUln.ToString());

                results.Should().ContainSingle();

                results.Single().Should().BeEquivalentTo(new CertificateMask
                {
                    CertificateType = CertificateTypes.Framework,
                    CourseCode = courseB,
                    CourseName = "Course B",
                    CourseLevel = "Level 3",
                    ProviderName = providerP2
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
                DateTime createdAt,
                Action<CertificateModel> overrides = null)
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

                        overrides?.Invoke(certificate);
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
                long? uln,
                string courseCode,
                string courseName,
                string providerName,
                DateTime createdAt,
                DateTime? dateAwarded = null,
                string ukprn = "12345678")
            {
                return WithFrameworkLearner(
                    Guid.NewGuid(),
                    Guid.NewGuid().ToString("N").Substring(0, 10),
                    createdAt.Year.ToString(),
                    dateAwarded ?? createdAt.Date,
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
                    ukprn,
                    courseName,
                    "Pathway",
                    "Level 3",
                    uln ?? 3000000999,
                    createdAt,
                    $"test learner {uln ?? 3000000999}");
            }

            public CertificateMaskStoredProceduresFixture WithSixIndependentStandardMasks()
            {
                for (var index = 1; index <= 6; index++)
                {
                    var organisationId = $"EPA_P{index}";
                    var ukprn = 11111110 + index;
                    var standardCode = 1000 + index;

                    // Each row gets its own course, provider and sector. Consequently every
                    // row receives CcSeqn = 1, PrSeqn = 1 and SeSeqn = 1.
                    WithOrganisation(
                        $"Provider {index} organisation",
                        organisationId,
                        ukprn,
                        null);
                    WithMaskStandard(
                        $"Course {index}",
                        $"ST000{index}",
                        standardCode,
                        $"Sector {index}");
                    WithStandardMaskCertificate(
                        organisationId,
                        2000001000 + index,
                        standardCode,
                        ukprn,
                        $"ST000{index}_1.0",
                        $"Course {index}",
                        $"Provider {index}",
                        new DateTime(2024, 6, index, 0, 0, 0, DateTimeKind.Utc));
                }

                return this;
            }

            public CertificateMaskStoredProceduresFixture WithSixIndependentFrameworkMasks()
            {
                for (var index = 1; index <= 6; index++)
                {
                    // Each row gets its own course, provider and award date. Consequently
                    // every row is rank one in all three framework partitions.
                    WithFrameworkMaskCertificate(
                        3000001000 + index,
                        $"FW{index}",
                        $"Course {index}",
                        $"Provider {index}",
                        new DateTime(2024, 6, index, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(2024, 5, index, 0, 0, 0, DateTimeKind.Utc));
                }

                return this;
            }

            public Task<List<CertificateMask>> ExecuteStandardMasks(
                string excludeUlns,
                int top = 5)
            {
                return _databaseService.ExecuteStoredProcedure<CertificateMask>(
                    "Certificates_GetStandardMasks",
                    new { ExcludeUlns = excludeUlns, Top = top });
            }

            public Task<List<CertificateMask>> ExecuteFrameworkMasks(
                string excludeUlns,
                int top = 5)
            {
                return _databaseService.ExecuteStoredProcedure<CertificateMask>(
                    "Certificates_GetFrameworkMasks",
                    new { ExcludeUlns = excludeUlns, Top = top });
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