using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.IntegrationTests.Factories;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Repositories.StandardRepositoryTests
{
    [TestFixture]
    [NonParallelizable]
    public class WhenImportingStandardsAndOptions : TestBase
    {
        [TestCase(false)]
        [TestCase(true)]
        public async Task Then_imports_standard_fields_and_options_through_bulk_copy(
            bool useNullValues)
        {
            // Arrange
            var expected = StandardFactory.CreateFull();
            expected.EPAChanged = true;
            expected.CoronationEmblem = true;
            expected.EpaoMustBeApprovedByRegulatorBody = true;

            if (useNullValues)
            {
                expected.Version = null;
                expected.LastDateStarts = null;
                expected.EffectiveFrom = null;
                expected.EffectiveTo = null;
                expected.VersionEarliestStartDate = null;
                expected.VersionLatestStartDate = null;
                expected.VersionLatestEndDate = null;
                expected.VersionApprovedForDelivery = null;
                expected.StandardPageUrl = null;
                expected.TrailblazerContact = null;
                expected.Route = null;
                expected.IntegratedDegree = null;
                expected.EqaProviderName = null;
                expected.EqaProviderContactName = null;
                expected.EqaProviderContactEmail = null;
                expected.OverviewOfRole = null;
            }

            var standard = ToStandard(expected);

            var options = new[]
            {
                new StandardOption
                {
                    StandardUId = standard.StandardUId,
                    OptionName = "Option A"
                },
                new StandardOption
                {
                    StandardUId = standard.StandardUId,
                    OptionName = "Option B"
                }
            };

            using (var fixture = new MergeStandardsFromStagingTestsFixture())
            {
                // Act
                await fixture.ImportStandardsAndOptions(
                    new[] { standard },
                    options);

                // Assert
                fixture.GetStagingStandards().Should().BeEquivalentTo(new[] { expected });
                fixture.GetStagingStandardOptions().Should().BeEquivalentTo(options);
                fixture.GetStandards().Should().HaveCount(1);

                var actual = fixture.GetStandard(standard.StandardUId);
                actual.Should().BeEquivalentTo(expected, comparison => comparison.Excluding(s => s.UpdatedAt));
                actual.UpdatedAt.Should().NotBeNull();
                actual.UpdatedAt.Value.Should().BeOnOrAfter(fixture.MergeStartedAt);
                actual.UpdatedAt.Value.Should().BeOnOrBefore(fixture.MergeFinishedAt);

                fixture.GetStandardOptions().Should().BeEquivalentTo(options);
            }
        }

        [Test]
        public async Task Then_imports_a_standard_when_there_are_no_options()
        {
            // Arrange
            var expected = StandardFactory.CreateFull();
            var standard = ToStandard(expected);

            using (var fixture = new MergeStandardsFromStagingTestsFixture())
            {
                // Act
                await fixture.ImportStandardsAndOptions(
                    new[] { standard },
                    Array.Empty<StandardOption>());

                // Assert
                fixture.GetStandard(standard.StandardUId)
                    .Should().BeEquivalentTo(expected, comparison => comparison.Excluding(s => s.UpdatedAt));

                fixture.GetStandard(standard.StandardUId).UpdatedAt.Should().NotBeNull();

                fixture.GetStagingStandardOptions().Should().BeEmpty();
                fixture.GetStandardOptions().Should().BeEmpty();
            }
        }

        [Test]
        public async Task Then_rolls_back_bulk_copied_and_live_data_when_merging_fails()
        {
            // Arrange
            var standard = ToStandard(StandardFactory.CreateFull());

            var options = new[]
            {
                new StandardOption
                {
                    StandardUId = standard.StandardUId,
                    OptionName = "Duplicate option"
                },
                new StandardOption
                {
                    StandardUId = standard.StandardUId,
                    OptionName = "Duplicate option"
                }
            };

            using (var fixture = new MergeStandardsFromStagingTestsFixture())
            {
                // Act
                Func<Task> act = () => fixture.ImportStandardsAndOptions(
                    new[] { standard },
                    options);

                // Assert
                var result = await act.Should().ThrowAsync<SqlException>();

                result.Which.Number.Should().BeOneOf(2601, 2627);

                fixture.GetStagingStandards().Should().BeEmpty();
                fixture.GetStagingStandardOptions().Should().BeEmpty();
                fixture.GetStandards().Should().BeEmpty();
                fixture.GetStandardOptions().Should().BeEmpty();
            }
        }

        private static Standard ToStandard(StandardModel standard)
        {
            return new Standard
            {
                StandardUId = standard.StandardUId,
                IfateReferenceNumber = standard.IFateReferenceNumber,
                LarsCode = standard.LarsCode.Value,
                Title = standard.Title,
                Version = standard.Version,
                Level = standard.Level,
                Status = standard.Status,
                TypicalDuration = standard.TypicalDuration,
                MaxFunding = standard.MaxFunding,
                IsActive = standard.IsActive == 1,
                LastDateStarts = standard.LastDateStarts,
                EffectiveFrom = standard.EffectiveFrom,
                EffectiveTo = standard.EffectiveTo,
                VersionEarliestStartDate = standard.VersionEarliestStartDate,
                VersionLatestStartDate = standard.VersionLatestStartDate,
                VersionLatestEndDate = standard.VersionLatestEndDate,
                VersionApprovedForDelivery = standard.VersionApprovedForDelivery,
                ProposedTypicalDuration = standard.ProposedTypicalDuration,
                ProposedMaxFunding = standard.ProposedMaxFunding,
                EPAChanged = standard.EPAChanged,
                StandardPageUrl = standard.StandardPageUrl,
                TrailBlazerContact = standard.TrailblazerContact,
                Route = standard.Route,
                VersionMajor = standard.VersionMajor,
                VersionMinor = standard.VersionMinor,
                IntegratedDegree = standard.IntegratedDegree,
                EqaProviderName = standard.EqaProviderName,
                EqaProviderContactName = standard.EqaProviderContactName,
                EqaProviderContactEmail = standard.EqaProviderContactEmail,
                OverviewOfRole = standard.OverviewOfRole,
                CoronationEmblem = standard.CoronationEmblem,
                EpaoMustBeApprovedByRegulatorBody =
                    standard.EpaoMustBeApprovedByRegulatorBody
            };
        }
    }
}