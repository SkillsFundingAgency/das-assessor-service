using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.Standards;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Standards.GetEpaoPipelineStandardsExtractHandlerTests
{
    [TestFixture]
    public class When_called
    {
        private GetEpaoPipelineStandardsExtractHandler _sut;
        private Mock<IApiConfiguration> _mockConfig;
        private Mock<IStandardRepository> _mockRepository;
        private Mock<ILogger<GetEpaoPipelineStandardsExtractHandler>> _mockLogger;

        [SetUp]
        public void Arrange()
        {
            _mockConfig = new Mock<IApiConfiguration>();
            _mockRepository = new Mock<IStandardRepository>();
            _mockLogger = new Mock<ILogger<GetEpaoPipelineStandardsExtractHandler>>();
            
            _sut = new GetEpaoPipelineStandardsExtractHandler(_mockConfig.Object, _mockRepository.Object, _mockLogger.Object); 
        }

        [TestCase("EPA0200", 3)]
        [TestCase("EPA0200", 2)]
        public async Task Then_number_of_epaos_in_pipeline_extracts_are_returned(string epaoId, int count)
        {
            // Arrange
            var fixture = new Fixture();
            
            _mockRepository
                .Setup(r => r.GetEpaoPipelineStandardsExtract(epaoId, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(fixture.CreateMany<EpaoPipelineStandardExtract>(count).ToList());

            _mockConfig
                .Setup(r => r.PipelineCutoff).Returns(6);

            // Act
            var result = await _sut.Handle(new EpaoPipelineStandardsExtractRequest(epaoId, string.Empty, string.Empty, string.Empty), new CancellationToken());

            // Assert
            _mockRepository
                .Verify(r => r.GetEpaoPipelineStandardsExtract(epaoId, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 6), Times.Once);

            result.Count().Should().Be(count);
        }
    }
}