using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Dashboard;
using SFA.DAS.AssessorService.Application.Handlers.Dashboard;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Settings;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Dashboard.GetEpaoDashboardHandlerTests
{
    [TestFixture]
    public class When_called
    {
        private GetEpaoDashboardHandler _sut;
        private Mock<IWebConfiguration> _mockConfig;
        private Mock<IDashboardRepository> _mockRepository;
        private Mock<ILogger<GetEpaoDashboardHandler>> _mockLogger;

        [SetUp]
        public void Arrange()
        {
            _mockConfig = new Mock<IWebConfiguration>();
            _mockRepository = new Mock<IDashboardRepository>();
            _mockLogger = new Mock<ILogger<GetEpaoDashboardHandler>>();


            _sut = new GetEpaoDashboardHandler(_mockConfig.Object, _mockRepository.Object, _mockLogger.Object);
        }

        [TestCase("EPA0200", 5)]
        [TestCase("EPA0200", 55)]
        public async Task Then_epao_dashboard_count_is_returned(string epaoId, int count)
        {
            // Arrange
            _mockRepository
                .Setup(r => r.GetEpaoDashboard(epaoId, It.IsAny<int>()))
                .ReturnsAsync(new EpaoDashboardResult { Pipeline = count });

            _mockConfig
                .Setup(r => r.PipelineCutoff).Returns(6);

            // Act
            var result = await _sut.Handle(new GetEpaoDashboardRequest { EpaoId = epaoId }, new CancellationToken());

            // Assert
            _mockRepository
                .Verify(r => r.GetEpaoDashboard(epaoId, 6), Times.Once);

            result.PipelinesCount.Should().Be(count);
        }
    }
}