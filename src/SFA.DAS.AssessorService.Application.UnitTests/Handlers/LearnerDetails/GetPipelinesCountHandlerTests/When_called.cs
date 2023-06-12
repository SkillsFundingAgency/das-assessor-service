using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.Learner;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Settings;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.LearnerDetails.GetPipelinesCountHandlerTests
{
    [TestFixture]
    public class When_called
    {
        private GetPipelinesCountHandler _sut;
        private Mock<IApiConfiguration> _mockConfig;
        private Mock<ILearnerRepository> _mockLearnerRepository;
        private Mock<ILogger<GetPipelinesCountHandler>> _mockLogger;

        [SetUp]
        public void Arrange()
        {
            _mockConfig = new Mock<IApiConfiguration>();
            _mockLearnerRepository = new Mock<ILearnerRepository>();
            _mockLogger = new Mock<ILogger<GetPipelinesCountHandler>>();
            

            _sut = new GetPipelinesCountHandler(_mockConfig.Object, _mockLearnerRepository.Object, _mockLogger.Object); 
        }

        [TestCase("EPA0200", 287, 5)]
        [TestCase("EPA0200", null, 55)]
        public async Task Then_epao_pipelines_count_is_returned(string epaoId, int? stdCode, int count)
        {
            // Arrange
            _mockLearnerRepository
                .Setup(r => r.GetEpaoPipelinesCount(epaoId, stdCode, It.IsAny<int>()))
                .ReturnsAsync(count);

            _mockConfig
                .Setup(r => r.PipelineCutoff).Returns(6);

            // Act
            var result = await _sut.Handle(new GetPipelinesCountRequest(epaoId, stdCode), new CancellationToken());

            // Assert
            _mockLearnerRepository
                .Verify(r => r.GetEpaoPipelinesCount(epaoId, stdCode, 6), Times.Once);

            result.Should().Be(count);
        }
    }
}