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

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Standards.GetEpaoPipelineStandardsHandlerTests
{
    [TestFixture]
    public class When_called
    {
        private GetEpaoPipelineStandardsHandler _sut;
        private Mock<IApiConfiguration> _mockConfig;
        private Mock<IStandardRepository> _mockRepository;
        private Mock<ILogger<GetEpaoPipelineStandardsHandler>> _mockLogger;

        [SetUp]
        public void Arrange()
        {
            _mockConfig = new Mock<IApiConfiguration>();
            _mockRepository = new Mock<IStandardRepository>();
            _mockLogger = new Mock<ILogger<GetEpaoPipelineStandardsHandler>>();

            _sut = new GetEpaoPipelineStandardsHandler(_mockConfig.Object, _mockRepository.Object, _mockLogger.Object); 
        }

        [TestCase("EPA0200", 3)]
        [TestCase("EPA0200", 2)]
        public async Task Then_number_of_epaos_in_pipeline_are_returned(string epaoId, int count)
        {
            // Arrange
            var fixture = new Fixture();
            
            _mockRepository
                .Setup(r => r.GetEpaoPipelineStandards(epaoId, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int?>()))
                .ReturnsAsync(new EpaoPipelineStandardsResult { PageOfResults = fixture.CreateMany<EpaoPipelineStandard>(count).ToList(), TotalCount = count });

            _mockConfig
                .Setup(r => r.PipelineCutoff).Returns(6);

            // Act
            var result = await _sut.Handle(new EpaoPipelineStandardsRequest(epaoId, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 1, 10), new CancellationToken());

            // Assert
            _mockRepository
                .Verify(r => r.GetEpaoPipelineStandards(epaoId, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 6, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int?>()), Times.Once);

            result.Items.Count().Should().Be(count);
        }
    }
}