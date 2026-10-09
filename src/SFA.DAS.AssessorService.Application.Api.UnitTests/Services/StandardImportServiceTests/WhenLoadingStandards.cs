using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Application.Api.Services;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Infrastructure.ApiClients.OuterApi;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Services.StandardImportServiceTests
{
    [TestFixture]
    public class WhenLoadingStandards
    {
        private Mock<IStandardRepository> _standardRepository;
        private StandardImportService _sut;
        private Standard _stagedStandard;

        [SetUp]
        public void SetUp()
        {
            _stagedStandard = null;
            _standardRepository = new Mock<IStandardRepository>(MockBehavior.Strict);

            _standardRepository
                .Setup(r => r.InsertStandardsIntoStaging(
                    It.IsAny<IEnumerable<Standard>>()))
                .Callback<IEnumerable<Standard>>(standards =>
                    _stagedStandard = standards.Single())
                .Returns(Task.CompletedTask);

            _sut = new StandardImportService(_standardRepository.Object);
        }

        [Test]
        public async Task Then_maps_the_standard_and_inserts_it_into_staging()
        {
            // Arrange
            var standard = new Fixture().Create<StandardDetailResponse>();

            // Act
            await _sut.StageStandards(new[] { standard });

            // Assert
            // Compare fields with matching names.
            _stagedStandard.Should().BeEquivalentTo(
                standard,
                options => options.ExcludingMissingMembers());

            // Compare fields mapped from nested objects.
            _stagedStandard.LastDateStarts.Should()
                .Be(standard.StandardDates.LastDateStarts);

            _stagedStandard.EffectiveFrom.Should()
                .Be(standard.StandardDates.EffectiveFrom);

            _stagedStandard.EffectiveTo.Should()
                .Be(standard.StandardDates.EffectiveTo);

            _stagedStandard.VersionApprovedForDelivery.Should()
                .Be(standard.VersionDetail.ApprovedForDelivery);

            _stagedStandard.VersionEarliestStartDate.Should()
                .Be(standard.VersionDetail.EarliestStartDate);

            _stagedStandard.VersionLatestStartDate.Should()
                .Be(standard.VersionDetail.LatestStartDate);

            _stagedStandard.VersionLatestEndDate.Should()
                .Be(standard.VersionDetail.LatestEndDate);

            _stagedStandard.ProposedTypicalDuration.Should()
                .Be(standard.VersionDetail.ProposedTypicalDuration);

            _stagedStandard.ProposedMaxFunding.Should()
                .Be(standard.VersionDetail.ProposedMaxFunding);

            _stagedStandard.EqaProviderName.Should()
                .Be(standard.EqaProvider.Name);

            _stagedStandard.EqaProviderContactName.Should()
                .Be(standard.EqaProvider.ContactName);

            _stagedStandard.EqaProviderContactEmail.Should()
                .Be(standard.EqaProvider.ContactEmail);

            _standardRepository.Verify(
                r => r.InsertStandardsIntoStaging(
                    It.IsAny<IEnumerable<Standard>>()),
                Times.Once);
        }

        [Test]
        public async Task Then_maps_missing_dates_and_eqa_provider_to_null()
        {
            // Arrange
            var standard = new Fixture().Create<StandardDetailResponse>();
            standard.StandardDates = null;
            standard.EqaProvider = null;

            // Act
            await _sut.StageStandards(new[] { standard });

            // Assert
            _stagedStandard.LastDateStarts.Should().BeNull();
            _stagedStandard.EffectiveFrom.Should().BeNull();
            _stagedStandard.EffectiveTo.Should().BeNull();
            _stagedStandard.EqaProviderName.Should().BeNull();
            _stagedStandard.EqaProviderContactName.Should().BeNull();
            _stagedStandard.EqaProviderContactEmail.Should().BeNull();

            _standardRepository.Verify(
                r => r.InsertStandardsIntoStaging(
                    It.IsAny<IEnumerable<Standard>>()),
                Times.Once);
        }
    }
}