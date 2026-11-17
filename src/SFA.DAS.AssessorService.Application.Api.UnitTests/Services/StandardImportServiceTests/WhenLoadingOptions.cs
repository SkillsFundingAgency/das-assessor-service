using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class WhenLoadingOptions
    {
        private Mock<IStandardRepository> _standardRepository;
        private StandardImportService _sut;
        private StandardOption[] _stagedOptions;

        [SetUp]
        public void SetUp()
        {
            _stagedOptions = null;
            _standardRepository = new Mock<IStandardRepository>(MockBehavior.Strict);

            _standardRepository
                .Setup(r => r.InsertOptionsIntoStaging(
                    It.IsAny<IEnumerable<StandardOption>>()))
                .Callback<IEnumerable<StandardOption>>(options =>
                    _stagedOptions = options.ToArray())
                .Returns(Task.CompletedTask);

            _sut = new StandardImportService(_standardRepository.Object);
        }

        [Test]
        public async Task Then_maps_the_options_and_inserts_them_into_staging()
        {
            // Arrange
            var standards = new[]
            {
                new StandardDetailResponse
                {
                    StandardUId = "ST0001_1.0",
                    Options = new List<string> { "Option A", "Option B" }
                },
                new StandardDetailResponse
                {
                    StandardUId = "ST0002_1.0",
                    Options = new List<string> { "Option C" }
                }
            };

            var expectedOptions = new[]
            {
                new StandardOption
                {
                    StandardUId = "ST0001_1.0",
                    OptionName = "Option A"
                },
                new StandardOption
                {
                    StandardUId = "ST0001_1.0",
                    OptionName = "Option B"
                },
                new StandardOption
                {
                    StandardUId = "ST0002_1.0",
                    OptionName = "Option C"
                }
            };

            // Act
            await _sut.StageOptions(standards);

            // Assert
            _stagedOptions.Should().BeEquivalentTo(expectedOptions);

            _standardRepository.Verify(
                r => r.InsertOptionsIntoStaging(
                    It.IsAny<IEnumerable<StandardOption>>()),
                Times.Once);
        }

        [Test]
        public async Task Then_handles_null_and_empty_option_lists()
        {
            // Arrange
            var standards = new[]
            {
                new StandardDetailResponse
                {
                    StandardUId = "ST0001_1.0",
                    Options = null
                },
                new StandardDetailResponse
                {
                    StandardUId = "ST0002_1.0",
                    Options = new List<string>()
                }
            };

            // Act
            await _sut.StageOptions(standards);

            // Assert
            _stagedOptions.Should().BeEmpty();

            _standardRepository.Verify(
                r => r.InsertOptionsIntoStaging(
                    It.IsAny<IEnumerable<StandardOption>>()),
                Times.Once);
        }
    }
}