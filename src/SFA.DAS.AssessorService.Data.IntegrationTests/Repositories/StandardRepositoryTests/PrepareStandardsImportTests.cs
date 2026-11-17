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
    public class PrepareStandardsImportTests : TestBase
    {
        [Test]
        public async Task PrepareStandardsImport_ClearsStagingAndKeepsLiveData()
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            var staged = StandardFactory.Create(referenceNumber: "ST0002", larsCode: 456);
            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(live)
                .WithStandardOption(live.StandardUId, "Live option")
                .WithStagingStandard(staged)
                .WithStagingStandardOption(staged.StandardUId, "Staged option"))
            {
                // Act
                await fixture.PrepareStandardsImport();

                // Assert
                fixture.GetStagingStandards().Should().BeEmpty();
                fixture.GetStagingStandardOptions().Should().BeEmpty();
                fixture.GetStandards().Should().BeEquivalentTo(new[] { live });
                fixture.GetStandardOptions().Should().BeEquivalentTo(new[]
                {
                    new StandardOptionModel { StandardUId = live.StandardUId, OptionName = "Live option" }
                });
            }
        }

        [Test]
        public async Task PrepareStandardsImport_LeavesLiveDataUnchanged_WhenStagingIsAlreadyEmpty()
        {
            // Arrange
            var live = StandardFactory.CreateFull(updatedAt: new DateTime(2000, 1, 1));
            using (var fixture = new MergeStandardsFromStagingTestsFixture()
                .WithStandard(live)
                .WithStandardOption(live.StandardUId, "Live option"))
            {
                // Act
                await fixture.PrepareStandardsImport();
                await fixture.PrepareStandardsImport();

                // Assert
                fixture.GetStagingStandards().Should().BeEmpty();
                fixture.GetStagingStandardOptions().Should().BeEmpty();
                fixture.GetStandards().Should().BeEquivalentTo(new[] { live });
                fixture.GetStandardOptions().Should().BeEquivalentTo(new[]
                {
                    new StandardOptionModel { StandardUId = live.StandardUId, OptionName = "Live option" }
                });
            }
        }
    }
}
