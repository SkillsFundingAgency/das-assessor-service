using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.Learner;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.LearnerDetails.GetPipelinesCountHandlerTests
{
    [TestFixture]
    public class When_called
    {
        private GetPipelinesCountHandler _sut;
        private Mock<IIlrRepository> _mockIlrRepository;
        private Mock<ILogger<GetPipelinesCountHandler>> _mockLogger;

        [SetUp]
        public void Arrange()
        {
            _mockIlrRepository = new Mock<IIlrRepository>();
            _mockLogger = new Mock<ILogger<GetPipelinesCountHandler>>();

            _sut = new GetPipelinesCountHandler(_mockIlrRepository.Object, _mockLogger.Object); 
        }

        [TestCase("EPA0200", 287, 5)]
        [TestCase("EPA0200", null, 55)]
        public async Task Then_epao_pipelines_count_is_returned(string epaoId, int? stdCode, int count)
        {
            // Arrange
            _mockIlrRepository
                .Setup(r => r.GetEpaoPipelinesCount(epaoId, stdCode))
                .ReturnsAsync(count);

            // Act
            var result = await _sut.Handle(new GetPipelinesCountRequest(epaoId, stdCode), new CancellationToken());

            // Assert
            _mockIlrRepository
                .Verify(r => r.GetEpaoPipelinesCount(epaoId, stdCode), Times.Once);

            result.Should().Be(count);
        }
    }
}