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
    public class MergeStandardsFromStagingOptionsTests : TestBase
    {
        [Test]
        public async Task MergeStandardsFromStaging_AddsOptions_WhenStandardHasNoLiveOptions()
        {
            // Arrange
            var standard = StandardFactory.Create();
            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(standard)
                .WithStagingStandard(standard)
                .WithStagingStandardOption(standard.StandardUId, "Option A")
                .WithStagingStandardOption(standard.StandardUId, "Option B"))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                fixture.GetStandardOptions().Should().BeEquivalentTo(new[]
                {
                    new StandardOptionModel { StandardUId = standard.StandardUId, OptionName = "Option A" },
                    new StandardOptionModel { StandardUId = standard.StandardUId, OptionName = "Option B" }
                });
            }
        }

        [Test]
        public async Task MergeStandardsFromStaging_KeepsMatchingOptions()
        {
            // Arrange
            var standard = StandardFactory.Create();
            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(standard)
                .WithStagingStandard(standard)
                .WithStandardOption(standard.StandardUId, "Option A")
                .WithStandardOption(standard.StandardUId, "Option B")
                .WithStagingStandardOption(standard.StandardUId, "Option A")
                .WithStagingStandardOption(standard.StandardUId, "Option B"))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                fixture.GetStandardOptions().Should().BeEquivalentTo(new[]
                {
                    new StandardOptionModel { StandardUId = standard.StandardUId, OptionName = "Option A" },
                    new StandardOptionModel { StandardUId = standard.StandardUId, OptionName = "Option B" }
                });
            }
        }

        [Test]
        public async Task MergeStandardsFromStaging_ReplacesCompleteList_WhenAnIncomingOptionIsNew()
        {
            // Arrange
            var standard = StandardFactory.Create();
            var other = StandardFactory.Create(referenceNumber: "ST0002", larsCode: 456);
            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(standard)
                .WithStandard(other)
                .WithStagingStandard(standard)
                .WithStandardOption(standard.StandardUId, "Option A")
                .WithStandardOption(standard.StandardUId, "Option B")
                .WithStandardOption(other.StandardUId, "Other option")
                .WithStagingStandardOption(standard.StandardUId, "Option A")
                .WithStagingStandardOption(standard.StandardUId, "Option C"))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                fixture.GetStandardOptions().Should().BeEquivalentTo(new[]
                {
                    new StandardOptionModel { StandardUId = standard.StandardUId, OptionName = "Option A" },
                    new StandardOptionModel { StandardUId = standard.StandardUId, OptionName = "Option C" },
                    new StandardOptionModel { StandardUId = other.StandardUId, OptionName = "Other option" }
                });
            }
        }

        [Test]
        public async Task MergeStandardsFromStaging_RetainsRemovedOptions_WhenThereAreNoIncomingNewOptions()
        {
            // Arrange
            var standard = StandardFactory.Create();
            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(standard)
                .WithStagingStandard(standard)
                .WithStandardOption(standard.StandardUId, "Option A")
                .WithStandardOption(standard.StandardUId, "Option B")
                .WithStagingStandardOption(standard.StandardUId, "Option A"))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                fixture.GetStandardOptions().Should().BeEquivalentTo(new[]
                {
                    new StandardOptionModel { StandardUId = standard.StandardUId, OptionName = "Option A" },
                    new StandardOptionModel { StandardUId = standard.StandardUId, OptionName = "Option B" }
                });
            }
        }

        [Test]
        public async Task MergeStandardsFromStaging_RetainsOptions_WhenOptionsStagingIsEmpty()
        {
            // Arrange
            var standard = StandardFactory.Create();
            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(standard)
                .WithStagingStandard(standard)
                .WithStandardOption(standard.StandardUId, "Option A"))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                fixture.GetStandardOptions().Should().BeEquivalentTo(new[]
                {
                    new StandardOptionModel { StandardUId = standard.StandardUId, OptionName = "Option A" }
                });
            }
        }

        [Test]
        public async Task MergeStandardsFromStaging_RetainsOptions_WhenOnlyOtherStandardsHaveStagedOptions()
        {
            // Arrange
            var standard = StandardFactory.Create();
            var other = StandardFactory.Create(referenceNumber: "ST0002", larsCode: 456);
            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(standard)
                .WithStagingStandard(standard)
                .WithStagingStandard(other)
                .WithStandardOption(standard.StandardUId, "Option A")
                .WithStagingStandardOption(other.StandardUId, "Other option"))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                fixture.GetStandardOptions().Should().BeEquivalentTo(new[]
                {
                    new StandardOptionModel { StandardUId = standard.StandardUId, OptionName = "Option A" },
                    new StandardOptionModel { StandardUId = other.StandardUId, OptionName = "Other option" }
                });
            }
        }

        [Test]
        public async Task MergeStandardsFromStaging_KeepsSameOptionNameOnDifferentStandards()
        {
            // Arrange
            var first = StandardFactory.Create();
            var second = StandardFactory.Create(referenceNumber: "ST0002", larsCode: 456);
            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStagingStandard(first)
                .WithStagingStandard(second)
                .WithStagingStandardOption(first.StandardUId, "Shared option")
                .WithStagingStandardOption(second.StandardUId, "Shared option"))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                fixture.GetStandardOptions().Should().BeEquivalentTo(new[]
                {
                    new StandardOptionModel { StandardUId = first.StandardUId, OptionName = "Shared option" },
                    new StandardOptionModel { StandardUId = second.StandardUId, OptionName = "Shared option" }
                });
            }
        }

        [Test]
        public async Task MergeStandardsFromStaging_PreservesUnicodeOptionNames()
        {
            // Arrange
            var standard = StandardFactory.Create();
            const string optionName = "安装与维护";
            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStagingStandard(standard)
                .WithStagingStandardOption(standard.StandardUId, optionName))
            {
                // Act
                await fixture.MergeStandardsFromStaging();

                // Assert
                fixture.GetStandardOptions().Should().BeEquivalentTo(new[]
                {
                    new StandardOptionModel { StandardUId = standard.StandardUId, OptionName = optionName }
                });
            }
        }
    }
}
