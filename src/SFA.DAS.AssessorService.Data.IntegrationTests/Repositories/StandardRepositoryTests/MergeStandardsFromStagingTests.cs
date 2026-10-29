using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.IntegrationTests.Factories;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Repositories.StandardRepositoryTests
{
    [TestFixture]
    [NonParallelizable]
    public class MergeStandardsFromStagingTests : TestBase
    {
        [Test]
        public async Task MergeStandardsFromStaging_InsertsNewStandardAndOptions()
        {
            // Arrange
            var staged = StandardFactory.CreateFull();

            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStagingStandard(staged)
                .WithStagingStandardOption(staged.StandardUId, "Option A")
                .WithStagingStandardOption(staged.StandardUId, "Option B"))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                var actual = fixture.GetStandard(staged.StandardUId);

                actual.Should().BeEquivalentTo(
                    staged,
                    options => options.Excluding(s => s.UpdatedAt));

                actual.UpdatedAt.Should().NotBeNull();
                actual.UpdatedAt.Value.Should()
                    .BeOnOrAfter(fixture.MergeStartedAt);
                actual.UpdatedAt.Value.Should()
                    .BeOnOrBefore(fixture.MergeFinishedAt);

                fixture.GetStandardOptions().Should().BeEquivalentTo(new[]
                {
                    new StandardOptionModel
                    {
                        StandardUId = staged.StandardUId,
                        OptionName = "Option A"
                    },
                    new StandardOptionModel
                    {
                        StandardUId = staged.StandardUId,
                        OptionName = "Option B"
                    }
                });
            }
        }

        [Test]
        public async Task MergeStandardsFromStaging_RetainsStandardAbsentFromStaging()
        {
            // Arrange
            var retained = StandardFactory.CreateFull(
                referenceNumber: "ST0001",
                version: "1.0",
                larsCode: 123,
                updatedAt: new DateTime(2000, 1, 1));

            var incoming = StandardFactory.CreateFull(
                referenceNumber: "ST0002",
                version: "1.0",
                larsCode: 456);

            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(retained)
                .WithStandardOption(retained.StandardUId, "Retained option")
                .WithStagingStandard(incoming))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                fixture.GetStandard(retained.StandardUId)
                    .Should().BeEquivalentTo(retained);

                fixture.GetStandard(incoming.StandardUId)
                    .Should().NotBeNull();

                fixture.GetStandards().Should().HaveCount(2);

                fixture.GetStandardOptions().Should().BeEquivalentTo(new[]
                {
                    new StandardOptionModel
                    {
                        StandardUId = retained.StandardUId,
                        OptionName = "Retained option"
                    }
                });
            }
        }

        [Test]
        public async Task MergeStandardsFromStaging_LeavesLiveDataUnchanged_WhenBothStagingTablesAreEmpty()
        {
            // Arrange
            var live = StandardFactory.CreateFull(
                updatedAt: new DateTime(2000, 1, 1));

            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(live)
                .WithStandardOption(live.StandardUId, "Option A"))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                fixture.GetStandards().Should()
                    .BeEquivalentTo(new[] { live });

                fixture.GetStandardOptions().Should().BeEquivalentTo(new[]
                {
                    new StandardOptionModel
                    {
                        StandardUId = live.StandardUId,
                        OptionName = "Option A"
                    }
                });
            }
        }

        [Test]
        public async Task MergeStandardsFromStaging_RestoresMissingStandardsStatusesAndOptionNames()
        {
            // Arrange
            var stagedFirst = StandardFactory.CreateFull(
                referenceNumber: "ST0267",
                larsCode: 267,
                version: "1.1");

            var liveFirst = StandardFactory.CreateFull(
                referenceNumber: "ST0267",
                larsCode: 267,
                version: "1.1",
                updatedAt: new DateTime(2000, 1, 1));

            liveFirst.Status = "In development";

            var stagedSecond = StandardFactory.CreateFull(
                referenceNumber: "ST0267",
                larsCode: 267,
                version: "2.0");

            stagedSecond.Status = "In development";

            var liveSecond = StandardFactory.CreateFull(
                referenceNumber: "ST0267",
                larsCode: 267,
                version: "2.0",
                updatedAt: new DateTime(2000, 1, 1));

            var missing = StandardFactory.CreateFull(
                referenceNumber: "ST0002",
                larsCode: 456,
                version: "2.0");

            var optionStandard = StandardFactory.CreateFull(
                referenceNumber: "ST0087",
                larsCode: 87,
                updatedAt: new DateTime(2000, 1, 1));

            var stagedOptionStandard = StandardFactory.CreateFull(
                referenceNumber: "ST0087",
                larsCode: 87);

            const string liveOption =
                "Children, Young People and Families Manager within the Community";

            const string stagedOption =
                "Children, Young People & Families Manager within the Community";

            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(liveFirst)
                .WithStandard(liveSecond)
                .WithStandard(optionStandard)
                .WithStagingStandard(stagedFirst)
                .WithStagingStandard(stagedSecond)
                .WithStagingStandard(missing)
                .WithStagingStandard(stagedOptionStandard)
                .WithStandardOption(optionStandard.StandardUId, liveOption)
                .WithStagingStandardOption(
                    optionStandard.StandardUId,
                    stagedOption))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                fixture.GetStandards().Should().BeEquivalentTo(
                    new[]
                    {
                        stagedFirst,
                        stagedSecond,
                        missing,
                        stagedOptionStandard
                    },
                    options => options.Excluding(s => s.UpdatedAt));

                foreach (var expected in new[]
                {
                    stagedFirst,
                    stagedSecond,
                    missing
                })
                {
                    var actual = fixture.GetStandard(expected.StandardUId);

                    actual.UpdatedAt.Should().NotBeNull();
                    actual.UpdatedAt.Value.Should()
                        .BeOnOrAfter(fixture.MergeStartedAt);
                    actual.UpdatedAt.Value.Should()
                        .BeOnOrBefore(fixture.MergeFinishedAt);
                }

                fixture.GetStandard(optionStandard.StandardUId)
                    .UpdatedAt.Should().Be(optionStandard.UpdatedAt);

                fixture.GetStandardOptions().Should().BeEquivalentTo(new[]
                {
                    new StandardOptionModel
                    {
                        StandardUId = optionStandard.StandardUId,
                        OptionName = stagedOption
                    }
                });
            }
        }

        [Test]
        public async Task MergeStandardsFromStaging_RollsBackStandardsAndOptions_WhenOptionsInsertFails()
        {
            // Arrange
            var live = StandardFactory.CreateFull(
                referenceNumber: "ST0001",
                version: "1.0",
                larsCode: 123,
                updatedAt: new DateTime(2000, 1, 1));

            var staged = StandardFactory.CreateFull(
                title: "Updated title",
                referenceNumber: "ST0001",
                version: "1.0",
                larsCode: 123);

            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(live)
                .WithStandardOption(live.StandardUId, "Original option")
                .WithStagingStandard(staged)
                .WithStagingStandardOption(
                    live.StandardUId,
                    "Duplicate option")
                .WithStagingStandardOption(
                    live.StandardUId,
                    "Duplicate option"))
            {
                // Act
                Func<Task> act = () => fixture.MergeStandardsFromStaging();

                // Assert
                var result = await act.Should().ThrowAsync<SqlException>();

                result.Which.Number.Should().BeOneOf(2601, 2627);

                fixture.GetStandards().Should()
                    .BeEquivalentTo(new[] { live });

                fixture.GetStandardOptions().Should().BeEquivalentTo(new[]
                {
                    new StandardOptionModel
                    {
                        StandardUId = live.StandardUId,
                        OptionName = "Original option"
                    }
                });
            }
        }
    }
}