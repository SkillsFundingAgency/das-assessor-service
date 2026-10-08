using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.AssessorService.Data.IntegrationTests.Factories;

namespace SFA.DAS.AssessorService.Data.IntegrationTests.Repositories.StandardRepositoryTests
{
    [TestFixture]
    [NonParallelizable]
    public class MergeStandardsFromStagingFieldChangesTests : TestBase
    {
        [Test]
        public async Task MergeStandardsFromStaging_UpdatesLarsCode_WhenChanged()
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            staged.LarsCode = 456;

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

        [Test]
        public async Task MergeStandardsFromStaging_UpdatesTitle_WhenChanged()
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            staged.Title = "Updated standard";

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

        [Test]
        public async Task MergeStandardsFromStaging_UpdatesVersion_WhenChanged()
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            staged.Version = "2.0";

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

        [Test]
        public async Task MergeStandardsFromStaging_UpdatesLevel_WhenChanged()
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            staged.Level = 4;

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

        [Test]
        public async Task MergeStandardsFromStaging_UpdatesStatus_WhenChanged()
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
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
                actual.Should().BeEquivalentTo(staged, options => options.Excluding(s => s.UpdatedAt));
                actual.UpdatedAt.Should().NotBeNull();
                actual.UpdatedAt.Value.Should().BeOnOrAfter(fixture.MergeStartedAt);
                actual.UpdatedAt.Value.Should().BeOnOrBefore(fixture.MergeFinishedAt);
            }
        }

        [Test]
        public async Task MergeStandardsFromStaging_UpdatesTypicalDuration_WhenChanged()
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            staged.TypicalDuration = 36;

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

        [Test]
        public async Task MergeStandardsFromStaging_UpdatesMaxFunding_WhenChanged()
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            staged.MaxFunding = 22000;

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

        [Test]
        public async Task MergeStandardsFromStaging_UpdatesIsActive_WhenChanged()
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            staged.IsActive = 0;

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

        [Test]
        public async Task MergeStandardsFromStaging_UpdatesLastDateStarts_WhenChanged()
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            staged.LastDateStarts = new DateTime(2029, 1, 1);

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

        [Test]
        public async Task MergeStandardsFromStaging_UpdatesEffectiveFrom_WhenChanged()
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            staged.EffectiveFrom = new DateTime(2021, 2, 1);

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

        [Test]
        public async Task MergeStandardsFromStaging_UpdatesEffectiveTo_WhenChanged()
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            staged.EffectiveTo = new DateTime(2029, 3, 1);

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

        [Test]
        public async Task MergeStandardsFromStaging_UpdatesVersionEarliestStartDate_WhenChanged()
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            staged.VersionEarliestStartDate = new DateTime(2021, 4, 1);

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

        [Test]
        public async Task MergeStandardsFromStaging_UpdatesVersionLatestStartDate_WhenChanged()
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            staged.VersionLatestStartDate = new DateTime(2029, 5, 1);

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

        [Test]
        public async Task MergeStandardsFromStaging_UpdatesVersionLatestEndDate_WhenChanged()
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            staged.VersionLatestEndDate = new DateTime(2031, 6, 1);

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

        [Test]
        public async Task MergeStandardsFromStaging_UpdatesVersionApprovedForDelivery_WhenChanged()
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            staged.VersionApprovedForDelivery = new DateTime(2021, 7, 1);

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

        [Test]
        public async Task MergeStandardsFromStaging_UpdatesProposedTypicalDuration_WhenChanged()
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            staged.ProposedTypicalDuration = 42;

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

        [Test]
        public async Task MergeStandardsFromStaging_UpdatesProposedMaxFunding_WhenChanged()
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            staged.ProposedMaxFunding = 25000;

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

        [Test]
        public async Task MergeStandardsFromStaging_UpdatesEPAChanged_WhenChanged()
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            staged.EPAChanged = true;

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

        [Test]
        public async Task MergeStandardsFromStaging_UpdatesStandardPageUrl_WhenChanged()
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            staged.StandardPageUrl = "https://example.org/updated-standard";

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

        [Test]
        public async Task MergeStandardsFromStaging_UpdatesTrailBlazerContact_WhenChanged()
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            staged.TrailblazerContact = "updated-trailblazer@example.org";

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

        [Test]
        public async Task MergeStandardsFromStaging_UpdatesRoute_WhenChanged()
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            staged.Route = "Construction";

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

        [Test]
        public async Task MergeStandardsFromStaging_UpdatesVersionMajor_WhenChanged()
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            staged.VersionMajor = 2;

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

        [Test]
        public async Task MergeStandardsFromStaging_UpdatesVersionMinor_WhenChanged()
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            staged.VersionMinor = 1;

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

        [Test]
        public async Task MergeStandardsFromStaging_UpdatesIntegratedDegree_WhenChanged()
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            staged.IntegratedDegree = "Integrated degree";

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

        [Test]
        public async Task MergeStandardsFromStaging_UpdatesEqaProviderName_WhenChanged()
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            staged.EqaProviderName = "Updated EQA provider";

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

        [Test]
        public async Task MergeStandardsFromStaging_UpdatesEqaProviderContactName_WhenChanged()
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            staged.EqaProviderContactName = "Updated contact";

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

        [Test]
        public async Task MergeStandardsFromStaging_UpdatesEqaProviderContactEmail_WhenChanged()
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            staged.EqaProviderContactEmail = "updated-eqa@example.org";

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

        [Test]
        public async Task MergeStandardsFromStaging_UpdatesOverviewOfRole_WhenChanged()
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            staged.OverviewOfRole = "Updated overview";

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

        [Test]
        public async Task MergeStandardsFromStaging_UpdatesCoronationEmblem_WhenChanged()
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            staged.CoronationEmblem = true;

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

        [Test]
        public async Task MergeStandardsFromStaging_UpdatesEpaoMustBeApprovedByRegulatorBody_WhenChanged()
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.CreateFull();
            staged.EpaoMustBeApprovedByRegulatorBody = true;

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
