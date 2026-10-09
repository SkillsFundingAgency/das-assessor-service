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
    public class MergeStandardsFromStagingFieldChangesTests : TestBase
    {
        [TestCase(nameof(StandardModel.IFateReferenceNumber))]
        [TestCase(nameof(StandardModel.LarsCode))]
        [TestCase(nameof(StandardModel.Title))]
        [TestCase(nameof(StandardModel.Version))]
        [TestCase(nameof(StandardModel.Level))]
        [TestCase(nameof(StandardModel.Status))]
        [TestCase(nameof(StandardModel.TypicalDuration))]
        [TestCase(nameof(StandardModel.MaxFunding))]
        [TestCase(nameof(StandardModel.IsActive))]
        [TestCase(nameof(StandardModel.LastDateStarts))]
        [TestCase(nameof(StandardModel.EffectiveFrom))]
        [TestCase(nameof(StandardModel.EffectiveTo))]
        [TestCase(nameof(StandardModel.VersionEarliestStartDate))]
        [TestCase(nameof(StandardModel.VersionLatestStartDate))]
        [TestCase(nameof(StandardModel.VersionLatestEndDate))]
        [TestCase(nameof(StandardModel.VersionApprovedForDelivery))]
        [TestCase(nameof(StandardModel.ProposedTypicalDuration))]
        [TestCase(nameof(StandardModel.ProposedMaxFunding))]
        [TestCase(nameof(StandardModel.EPAChanged))]
        [TestCase(nameof(StandardModel.StandardPageUrl))]
        [TestCase(nameof(StandardModel.TrailblazerContact))]
        [TestCase(nameof(StandardModel.Route))]
        [TestCase(nameof(StandardModel.VersionMajor))]
        [TestCase(nameof(StandardModel.VersionMinor))]
        [TestCase(nameof(StandardModel.IntegratedDegree))]
        [TestCase(nameof(StandardModel.EqaProviderName))]
        [TestCase(nameof(StandardModel.EqaProviderContactName))]
        [TestCase(nameof(StandardModel.EqaProviderContactEmail))]
        [TestCase(nameof(StandardModel.OverviewOfRole))]
        [TestCase(nameof(StandardModel.CoronationEmblem))]
        [TestCase(nameof(StandardModel.EpaoMustBeApprovedByRegulatorBody))]
        public async Task MergeStandardsFromStaging_UpdatesPropertyAndTimestamp_WhenChanged(
            string propertyName)
        {
            // Arrange
            var live = StandardFactory.CreateFull(
                updatedAt: new DateTime(2000, 1, 1));

            var staged = StandardFactory.CreateFull();

            // these values are different to what the CreateFull sets by default
            var changedValues = new StandardModel
            {
                IFateReferenceNumber = "ST9999",
                LarsCode = 456,
                Title = "Updated standard",
                Version = "2.0",
                Level = 4,
                Status = "In development",
                TypicalDuration = 36,
                MaxFunding = 22000,
                IsActive = 0,
                LastDateStarts = new DateTime(2029, 1, 1),
                EffectiveFrom = new DateTime(2021, 2, 1),
                EffectiveTo = new DateTime(2029, 3, 1),
                VersionEarliestStartDate = new DateTime(2021, 4, 1),
                VersionLatestStartDate = new DateTime(2029, 5, 1),
                VersionLatestEndDate = new DateTime(2031, 6, 1),
                VersionApprovedForDelivery = new DateTime(2021, 7, 1),
                ProposedTypicalDuration = 42,
                ProposedMaxFunding = 25000,
                EPAChanged = true,
                StandardPageUrl = "https://example.org/updated-standard",
                TrailblazerContact = "updated-trailblazer@example.org",
                Route = "Construction",
                VersionMajor = 2,
                VersionMinor = 1,
                IntegratedDegree = "Integrated degree",
                EqaProviderName = "Updated EQA provider",
                EqaProviderContactName = "Updated contact",
                EqaProviderContactEmail = "updated-eqa@example.org",
                OverviewOfRole = "Updated overview",
                CoronationEmblem = true,
                EpaoMustBeApprovedByRegulatorBody = true
            };

            var property = typeof(StandardModel).GetProperty(propertyName);
            var changedValue = property.GetValue(changedValues);

            property.SetValue(staged, changedValue);

            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(live)
                .WithStagingStandard(staged))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                changedValue.Should().NotBe(property.GetValue(live), "the test must change the selected property");

                var actual = fixture.GetStandard(live.StandardUId);
                actual.Should().BeEquivalentTo(staged, options => options.Excluding(s => s.UpdatedAt));

                actual.UpdatedAt.Should().NotBeNull();
                actual.UpdatedAt.Value.Should().BeOnOrAfter(fixture.MergeStartedAt);
                actual.UpdatedAt.Value.Should().BeOnOrBefore(fixture.MergeFinishedAt);
            }
        }


        [TestCase(nameof(StandardModel.IFateReferenceNumber))]
        [TestCase(nameof(StandardModel.Title))]
        [TestCase(nameof(StandardModel.Status))]
        [TestCase(nameof(StandardModel.StandardPageUrl))]
        [TestCase(nameof(StandardModel.TrailblazerContact))]
        [TestCase(nameof(StandardModel.Route))]
        [TestCase(nameof(StandardModel.EqaProviderName))]
        [TestCase(nameof(StandardModel.EqaProviderContactName))]
        [TestCase(nameof(StandardModel.EqaProviderContactEmail))]
        [TestCase(nameof(StandardModel.OverviewOfRole))]
        public async Task MergeStandardsFromStaging_UpdatesPropertyAndTimestamp_WhenOnlyCaseChanges(
            string propertyName)
        {
            // Arrange
            var live = StandardFactory.CreateFull(
                updatedAt: new DateTime(2000, 1, 1));

            var staged = StandardFactory.CreateFull();

            var property = typeof(StandardModel).GetProperty(propertyName);
            var originalValue = (string)property.GetValue(staged);
            var changedValue = originalValue.ToUpperInvariant();

            // Already-uppercase values, such as the reference number,
            // must be changed to lowercase instead.
            if (string.Equals(
                originalValue,
                changedValue,
                StringComparison.Ordinal))
            {
                changedValue = originalValue.ToLowerInvariant();
            }

            property.SetValue(staged, changedValue);

            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(live)
                .WithStagingStandard(staged))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                changedValue.Should().NotBe(originalValue, "the test must change the letter case");

                var actual = fixture.GetStandard(live.StandardUId);
                ((string)property.GetValue(actual)).Should().Be(changedValue);
                actual.Should().BeEquivalentTo(staged, options => options.Excluding(s => s.UpdatedAt));

                actual.UpdatedAt.Should().NotBeNull();
                actual.UpdatedAt.Should().NotBe(live.UpdatedAt);
                actual.UpdatedAt.Value.Should().BeOnOrAfter(fixture.MergeStartedAt);
                actual.UpdatedAt.Value.Should().BeOnOrBefore(fixture.MergeFinishedAt);
            }
        }
    }
}