using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Data.IntegrationTests.Repositories;
using SFA.DAS.AssessorService.Data.IntegrationTests.Services;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Certificates
{
    public class When_Executing_Certificate_StoredProcedures : TestBase
    {
        [Test]
        public async Task Certificates_GetStandardMasks_Returns_Results()
        {
            var currentDateTime = DateTime.UtcNow;
            const string standardName = "Test Standard";
            const string certificateData = "{\"StandardName\":\"Test Standard\"}";

            using (var fixture = new CertificateStoredProceduresFixture()
                .WithOrganisation("Test Org", "EPA_TEST", 99999999, null)
                .WithStandard(standardName, "IFATE_REF", 1234, "1.0", currentDateTime.AddYears(-1), null)
                .WithCertificateForStoredProcedure(
                    Guid.NewGuid(), certificateData, currentDateTime,
                    20000000, 1234, 99999999, "EPA_TEST", "Submitted", "IFATE_REF_1.0")
                .WithCertificateForStoredProcedure(
                    Guid.NewGuid(), certificateData, currentDateTime,
                    20000001, 1235, 99999999, "EPA_TEST", "Submitted", "IFATE_REF_1.0"))
            {

                var results = await fixture.ExecGetStandardMasks(excludeUlns: "20000000");

                results.Should().NotBeNull();
                results.Count.Should().BeGreaterThan(0);

                var mask = results.FirstOrDefault(r => r.CourseCode == "1235");
                mask.Should().NotBeNull();
                mask.CourseName.Should().Be(standardName);
            }
        }

        [Test]
        public async Task Certificates_GetFrameworkMasks_Returns_Results()
        {
            var currentDateTime = DateTime.UtcNow;

            using (var fixture = new CertificateStoredProceduresFixture()
                .WithFrameworkLearner(
                    Guid.NewGuid(), "FCN1", "2024", currentDateTime.Date, "Jane Doe", "Doe", "Jane",
                    currentDateTime.Date.AddYears(-20), 30000000, "FW101", "Test Framework", "Pathway",
                    2, "ProvF", "12345678", "Test Framework", "Pathway", "Level 2", 1, currentDateTime, "jane doe")
                .WithFrameworkLearner(
                    Guid.NewGuid(), "FCN2", "2024", currentDateTime.Date, "John Smith", "Smith", "John",
                    currentDateTime.Date.AddYears(-22), 40000000, "FW100", "Test Framework", "Pathway",
                    2, "ProvF", "12345678", "Test Framework", "Pathway", "Level 2", 2, currentDateTime, "john smith"))
            {
                var results = await fixture.ExecGetFrameworkMasks(excludeUlns: "30000000");

                results.Should().NotBeNull();
                results.Count.Should().BeGreaterThan(0);

                var mask = results.FirstOrDefault(r => r.CourseCode == "FW100");
                mask.Should().NotBeNull();
                mask.CourseName.Should().Be("Test Framework");
            }
        }

        private class CertificateStoredProceduresFixture : FixtureBase<CertificateStoredProceduresFixture>, IDisposable
        {
            private readonly DatabaseService _databaseService = new DatabaseService();

            public async Task<List<CertificateMask>> ExecGetStandardMasks(string excludeUlns = null)
            {
                return await _databaseService.ExecuteStoredProcedure<CertificateMask>(
                    "Certificates_GetStandardMasks",
                    new { ExcludeUlns = excludeUlns, Top = 5 });
            }

            public async Task<List<CertificateMask>> ExecGetFrameworkMasks(string excludeUlns = null)
            {
                return await _databaseService.ExecuteStoredProcedure<CertificateMask>(
                    "Certificates_GetFrameworkMasks",
                    new { ExcludeUlns = excludeUlns, Top = 5 });
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
