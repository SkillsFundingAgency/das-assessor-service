using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.IntegrationTests.Factories;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Repositories.StandardRepositoryTests
{
    [TestFixture]
    [NonParallelizable]
    public class MergeStandardsFromStagingNullableFieldsTests : TestBase
    {
        [TestCase(true)]
        [TestCase(false)]
        public async Task MergeStandardsFromStaging_UpdatesLarsCode_WhenChangingToOrFromNull(bool changeToNull)
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            if (changeToNull)
                staged.LarsCode = null;
            else
                live.LarsCode = null;

            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(live)
                .WithStagingStandard(staged))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                var actual = fixture.GetStandard(live.StandardUId);
                actual.Should().BeEquivalentTo(staged, options => options.Excluding(s => s.UpdatedAt));
                actual.UpdatedAt.Should().NotBeNull();
                actual.UpdatedAt.Value.Should().BeOnOrAfter(fixture.MergeStartedAt);
                actual.UpdatedAt.Value.Should().BeOnOrBefore(fixture.MergeFinishedAt);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MergeStandardsFromStaging_UpdatesVersion_WhenChangingToOrFromNull(bool changeToNull)
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            if (changeToNull)
                staged.Version = null;
            else
                live.Version = null;

            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(live)
                .WithStagingStandard(staged))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                var actual = fixture.GetStandard(live.StandardUId);
                actual.Should().BeEquivalentTo(staged, options => options.Excluding(s => s.UpdatedAt));
                actual.UpdatedAt.Should().NotBeNull();
                actual.UpdatedAt.Value.Should().BeOnOrAfter(fixture.MergeStartedAt);
                actual.UpdatedAt.Value.Should().BeOnOrBefore(fixture.MergeFinishedAt);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MergeStandardsFromStaging_UpdatesLastDateStarts_WhenChangingToOrFromNull(bool changeToNull)
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            if (changeToNull)
                staged.LastDateStarts = null;
            else
                live.LastDateStarts = null;

            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(live)
                .WithStagingStandard(staged))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                var actual = fixture.GetStandard(live.StandardUId);
                actual.Should().BeEquivalentTo(staged, options => options.Excluding(s => s.UpdatedAt));
                actual.UpdatedAt.Should().NotBeNull();
                actual.UpdatedAt.Value.Should().BeOnOrAfter(fixture.MergeStartedAt);
                actual.UpdatedAt.Value.Should().BeOnOrBefore(fixture.MergeFinishedAt);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MergeStandardsFromStaging_UpdatesEffectiveFrom_WhenChangingToOrFromNull(bool changeToNull)
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            if (changeToNull)
                staged.EffectiveFrom = null;
            else
                live.EffectiveFrom = null;

            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(live)
                .WithStagingStandard(staged))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                var actual = fixture.GetStandard(live.StandardUId);
                actual.Should().BeEquivalentTo(staged, options => options.Excluding(s => s.UpdatedAt));
                actual.UpdatedAt.Should().NotBeNull();
                actual.UpdatedAt.Value.Should().BeOnOrAfter(fixture.MergeStartedAt);
                actual.UpdatedAt.Value.Should().BeOnOrBefore(fixture.MergeFinishedAt);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MergeStandardsFromStaging_UpdatesEffectiveTo_WhenChangingToOrFromNull(bool changeToNull)
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            if (changeToNull)
                staged.EffectiveTo = null;
            else
                live.EffectiveTo = null;

            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(live)
                .WithStagingStandard(staged))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                var actual = fixture.GetStandard(live.StandardUId);
                actual.Should().BeEquivalentTo(staged, options => options.Excluding(s => s.UpdatedAt));
                actual.UpdatedAt.Should().NotBeNull();
                actual.UpdatedAt.Value.Should().BeOnOrAfter(fixture.MergeStartedAt);
                actual.UpdatedAt.Value.Should().BeOnOrBefore(fixture.MergeFinishedAt);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MergeStandardsFromStaging_UpdatesVersionEarliestStartDate_WhenChangingToOrFromNull(bool changeToNull)
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            if (changeToNull)
                staged.VersionEarliestStartDate = null;
            else
                live.VersionEarliestStartDate = null;

            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(live)
                .WithStagingStandard(staged))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                var actual = fixture.GetStandard(live.StandardUId);
                actual.Should().BeEquivalentTo(staged, options => options.Excluding(s => s.UpdatedAt));
                actual.UpdatedAt.Should().NotBeNull();
                actual.UpdatedAt.Value.Should().BeOnOrAfter(fixture.MergeStartedAt);
                actual.UpdatedAt.Value.Should().BeOnOrBefore(fixture.MergeFinishedAt);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MergeStandardsFromStaging_UpdatesVersionLatestStartDate_WhenChangingToOrFromNull(bool changeToNull)
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            if (changeToNull)
                staged.VersionLatestStartDate = null;
            else
                live.VersionLatestStartDate = null;

            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(live)
                .WithStagingStandard(staged))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                var actual = fixture.GetStandard(live.StandardUId);
                actual.Should().BeEquivalentTo(staged, options => options.Excluding(s => s.UpdatedAt));
                actual.UpdatedAt.Should().NotBeNull();
                actual.UpdatedAt.Value.Should().BeOnOrAfter(fixture.MergeStartedAt);
                actual.UpdatedAt.Value.Should().BeOnOrBefore(fixture.MergeFinishedAt);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MergeStandardsFromStaging_UpdatesVersionLatestEndDate_WhenChangingToOrFromNull(bool changeToNull)
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            if (changeToNull)
                staged.VersionLatestEndDate = null;
            else
                live.VersionLatestEndDate = null;

            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(live)
                .WithStagingStandard(staged))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                var actual = fixture.GetStandard(live.StandardUId);
                actual.Should().BeEquivalentTo(staged, options => options.Excluding(s => s.UpdatedAt));
                actual.UpdatedAt.Should().NotBeNull();
                actual.UpdatedAt.Value.Should().BeOnOrAfter(fixture.MergeStartedAt);
                actual.UpdatedAt.Value.Should().BeOnOrBefore(fixture.MergeFinishedAt);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MergeStandardsFromStaging_UpdatesVersionApprovedForDelivery_WhenChangingToOrFromNull(bool changeToNull)
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            if (changeToNull)
                staged.VersionApprovedForDelivery = null;
            else
                live.VersionApprovedForDelivery = null;

            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(live)
                .WithStagingStandard(staged))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                var actual = fixture.GetStandard(live.StandardUId);
                actual.Should().BeEquivalentTo(staged, options => options.Excluding(s => s.UpdatedAt));
                actual.UpdatedAt.Should().NotBeNull();
                actual.UpdatedAt.Value.Should().BeOnOrAfter(fixture.MergeStartedAt);
                actual.UpdatedAt.Value.Should().BeOnOrBefore(fixture.MergeFinishedAt);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MergeStandardsFromStaging_UpdatesStandardPageUrl_WhenChangingToOrFromNull(bool changeToNull)
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            if (changeToNull)
                staged.StandardPageUrl = null;
            else
                live.StandardPageUrl = null;

            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(live)
                .WithStagingStandard(staged))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                var actual = fixture.GetStandard(live.StandardUId);
                actual.Should().BeEquivalentTo(staged, options => options.Excluding(s => s.UpdatedAt));
                actual.UpdatedAt.Should().NotBeNull();
                actual.UpdatedAt.Value.Should().BeOnOrAfter(fixture.MergeStartedAt);
                actual.UpdatedAt.Value.Should().BeOnOrBefore(fixture.MergeFinishedAt);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MergeStandardsFromStaging_UpdatesTrailBlazerContact_WhenChangingToOrFromNull(bool changeToNull)
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            if (changeToNull)
                staged.TrailblazerContact = null;
            else
                live.TrailblazerContact = null;

            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(live)
                .WithStagingStandard(staged))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                var actual = fixture.GetStandard(live.StandardUId);
                actual.Should().BeEquivalentTo(staged, options => options.Excluding(s => s.UpdatedAt));
                actual.UpdatedAt.Should().NotBeNull();
                actual.UpdatedAt.Value.Should().BeOnOrAfter(fixture.MergeStartedAt);
                actual.UpdatedAt.Value.Should().BeOnOrBefore(fixture.MergeFinishedAt);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MergeStandardsFromStaging_UpdatesRoute_WhenChangingToOrFromNull(bool changeToNull)
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            if (changeToNull)
                staged.Route = null;
            else
                live.Route = null;

            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(live)
                .WithStagingStandard(staged))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                var actual = fixture.GetStandard(live.StandardUId);
                actual.Should().BeEquivalentTo(staged, options => options.Excluding(s => s.UpdatedAt));
                actual.UpdatedAt.Should().NotBeNull();
                actual.UpdatedAt.Value.Should().BeOnOrAfter(fixture.MergeStartedAt);
                actual.UpdatedAt.Value.Should().BeOnOrBefore(fixture.MergeFinishedAt);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MergeStandardsFromStaging_UpdatesIntegratedDegree_WhenChangingToOrFromNull(bool changeToNull)
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            if (changeToNull)
                staged.IntegratedDegree = null;
            else
                live.IntegratedDegree = null;

            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(live)
                .WithStagingStandard(staged))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                var actual = fixture.GetStandard(live.StandardUId);
                actual.Should().BeEquivalentTo(staged, options => options.Excluding(s => s.UpdatedAt));
                actual.UpdatedAt.Should().NotBeNull();
                actual.UpdatedAt.Value.Should().BeOnOrAfter(fixture.MergeStartedAt);
                actual.UpdatedAt.Value.Should().BeOnOrBefore(fixture.MergeFinishedAt);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MergeStandardsFromStaging_UpdatesEqaProviderName_WhenChangingToOrFromNull(bool changeToNull)
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            if (changeToNull)
                staged.EqaProviderName = null;
            else
                live.EqaProviderName = null;

            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(live)
                .WithStagingStandard(staged))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                var actual = fixture.GetStandard(live.StandardUId);
                actual.Should().BeEquivalentTo(staged, options => options.Excluding(s => s.UpdatedAt));
                actual.UpdatedAt.Should().NotBeNull();
                actual.UpdatedAt.Value.Should().BeOnOrAfter(fixture.MergeStartedAt);
                actual.UpdatedAt.Value.Should().BeOnOrBefore(fixture.MergeFinishedAt);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MergeStandardsFromStaging_UpdatesEqaProviderContactName_WhenChangingToOrFromNull(bool changeToNull)
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            if (changeToNull)
                staged.EqaProviderContactName = null;
            else
                live.EqaProviderContactName = null;

            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(live)
                .WithStagingStandard(staged))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                var actual = fixture.GetStandard(live.StandardUId);
                actual.Should().BeEquivalentTo(staged, options => options.Excluding(s => s.UpdatedAt));
                actual.UpdatedAt.Should().NotBeNull();
                actual.UpdatedAt.Value.Should().BeOnOrAfter(fixture.MergeStartedAt);
                actual.UpdatedAt.Value.Should().BeOnOrBefore(fixture.MergeFinishedAt);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MergeStandardsFromStaging_UpdatesEqaProviderContactEmail_WhenChangingToOrFromNull(bool changeToNull)
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            if (changeToNull)
                staged.EqaProviderContactEmail = null;
            else
                live.EqaProviderContactEmail = null;

            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(live)
                .WithStagingStandard(staged))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                var actual = fixture.GetStandard(live.StandardUId);
                actual.Should().BeEquivalentTo(staged, options => options.Excluding(s => s.UpdatedAt));
                actual.UpdatedAt.Should().NotBeNull();
                actual.UpdatedAt.Value.Should().BeOnOrAfter(fixture.MergeStartedAt);
                actual.UpdatedAt.Value.Should().BeOnOrBefore(fixture.MergeFinishedAt);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MergeStandardsFromStaging_UpdatesOverviewOfRole_WhenChangingToOrFromNull(bool changeToNull)
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            if (changeToNull)
                staged.OverviewOfRole = null;
            else
                live.OverviewOfRole = null;

            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(live)
                .WithStagingStandard(staged))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                var actual = fixture.GetStandard(live.StandardUId);
                actual.Should().BeEquivalentTo(staged, options => options.Excluding(s => s.UpdatedAt));
                actual.UpdatedAt.Should().NotBeNull();
                actual.UpdatedAt.Value.Should().BeOnOrAfter(fixture.MergeStartedAt);
                actual.UpdatedAt.Value.Should().BeOnOrBefore(fixture.MergeFinishedAt);
            }
        }

    }
}
