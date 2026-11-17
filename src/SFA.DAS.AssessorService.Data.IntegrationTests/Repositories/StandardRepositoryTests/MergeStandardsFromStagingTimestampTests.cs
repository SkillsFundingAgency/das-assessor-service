using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.IntegrationTests.Factories;
using SFA.DAS.AssessorService.Data.IntegrationTests.Models;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Repositories.StandardRepositoryTests
{
    [TestFixture]
    [NonParallelizable]
    public class MergeStandardsFromStagingTimestampTests : TestBase
    {
        [Test]
        public void Standards_StoresNullTimestamp_WhenNoTimestampIsProvided()
        {
            // Arrange
            var standard = StandardFactory.CreateFull();

            using (var fixture = new MergeStandardsFromStagingTestsFixture())
            {
                // Act
                fixture.WithStandard(standard);

                // Assert
                fixture.GetStandard(standard.StandardUId)
                    .UpdatedAt.Should().BeNull();
            }
        }

        [Test]
        public async Task MergeStandardsFromStaging_PreservesNullTimestamp_WhenStandardIsUnchanged()
        {
            // Arrange
            var standard = StandardFactory.CreateFull();

            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(standard)
                .WithStagingStandard(standard))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                fixture.GetStandard(standard.StandardUId)
                    .Should().BeEquivalentTo(standard);
            }
        }

        [Test]
        public async Task MergeStandardsFromStaging_PreservesExistingTimestamp_WhenStandardIsUnchanged()
        {
            // Arrange
            var live = StandardFactory.CreateFull(
                updatedAt: new DateTime(2000, 1, 1));

            var staged = StandardFactory.CreateFull();

            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(live)
                .WithStagingStandard(staged))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                fixture.GetStandard(live.StandardUId)
                    .Should().BeEquivalentTo(live);
            }
        }

        [Test]
        public async Task MergeStandardsFromStaging_SetsTimestamp_WhenChangedStandardPreviouslyHadNullTimestamp()
        {
            // Arrange
            var live = StandardFactory.CreateFull();
            var staged = StandardFactory.CreateFull();
            staged.Status = "In development";

            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(live)
                .WithStagingStandard(staged))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                var actual = fixture.GetStandard(live.StandardUId);

                actual.Should().BeEquivalentTo(
                    staged,
                    options => options.Excluding(s => s.UpdatedAt));

                actual.UpdatedAt.Should().NotBeNull();
                actual.UpdatedAt.Value.Should()
                    .BeOnOrAfter(fixture.MergeStartedAt);
                actual.UpdatedAt.Value.Should()
                    .BeOnOrBefore(fixture.MergeFinishedAt);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MergeStandardsFromStaging_DoesNotChangeTimestamp_WhenOnlyOptionsChange(
            bool hasTimestamp)
        {
            // Arrange
            var live = StandardFactory.CreateFull(
                updatedAt: hasTimestamp
                    ? new DateTime(2000, 1, 1)
                    : (DateTime?)null);

            var staged = StandardFactory.CreateFull();

            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(live)
                .WithStagingStandard(staged)
                .WithStandardOption(live.StandardUId, "Old option")
                .WithStagingStandardOption(live.StandardUId, "New option"))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                fixture.GetStandard(live.StandardUId)
                    .Should().BeEquivalentTo(live);

                fixture.GetStandardOptions().Should().BeEquivalentTo(new[]
                {
                    new StandardOptionModel
                    {
                        StandardUId = live.StandardUId,
                        OptionName = "New option"
                    }
                });
            }
        }

        [Test]
        public async Task MergeStandardsFromStaging_LeavesAllDataAndTimestampsUnchanged_OnSecondExecution()
        {
            // Arrange
            var live = StandardFactory.CreateFull(
                updatedAt: new DateTime(2000, 1, 1));

            var changed = StandardFactory.CreateFull();
            changed.Title = "Updated title";

            var inserted = StandardFactory.CreateFull(referenceNumber: "ST0003", larsCode: 456);
            var unchanged = StandardFactory.CreateFull(referenceNumber: "ST0004", larsCode: 789);

            var retained = StandardFactory.CreateFull(
                referenceNumber: "ST0005",
                larsCode: 101,
                updatedAt: new DateTime(2000, 1, 1));

            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(live)
                .WithStandard(unchanged)
                .WithStandard(retained)
                .WithStagingStandard(changed)
                .WithStagingStandard(inserted)
                .WithStagingStandard(unchanged)
                .WithStandardOption(live.StandardUId, "Old option")
                .WithStagingStandardOption(live.StandardUId, "New option")
                .WithStagingStandardOption(inserted.StandardUId, "Inserted option"))
            {
                await fixture.MergeStandardsFromStaging();

                var standardsAfterFirstMerge = fixture.GetStandards();
                var optionsAfterFirstMerge = fixture.GetStandardOptions();

                // Separate the executions beyond SQL DATETIME's precision.
                await Task.Delay(20);

                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                fixture.GetStandards().Should()
                    .BeEquivalentTo(standardsAfterFirstMerge);

                fixture.GetStandardOptions().Should()
                    .BeEquivalentTo(optionsAfterFirstMerge);
            }
        }
    }
}