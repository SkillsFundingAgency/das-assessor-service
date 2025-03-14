using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Application.Api.TaskQueue;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Register.Query
{
    [TestFixture]
    public class GetAssessmentsTests
    {
        private Mock<IMediator> _mediatorMock;
        private Mock<IBackgroundTaskQueue> _backgroundTaskQueueMock;
        private Mock<ILogger<RegisterQueryController>> _loggerMock;
        private RegisterQueryController _sut;

        [SetUp]
        public void Setup()
        {
            _mediatorMock = new Mock<IMediator>();
            _backgroundTaskQueueMock = new Mock<IBackgroundTaskQueue>();
            _loggerMock = new Mock<ILogger<RegisterQueryController>>();
            _sut = new RegisterQueryController(_mediatorMock.Object, _backgroundTaskQueueMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task GetAssessments_Should_Call_Mediator_With_Correct_Parameters()
        {
            // Arrange
            string standard = "ST0123";
            long ukprn = 12345678;
            var expectedResponse = new GetAssessmentsResponse(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), 5);

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetAssessmentsRequest>(r =>
                    r.Ukprn == ukprn && r.StandardReference == standard),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _sut.GetAssessments(standard, ukprn) as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
            result.Value.Should().BeEquivalentTo(expectedResponse);

            _mediatorMock.Verify(m =>
                m.Send(It.Is<GetAssessmentsRequest>(r =>
                    r.Ukprn == ukprn && r.StandardReference == standard),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task GetAssessments_Should_Call_Mediator_With_Null_Standard_When_Not_Provided()
        {
            // Arrange
            string standard = null;
            long ukprn = 12345678;
            var expectedResponse = new GetAssessmentsResponse(new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc), 5);

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetAssessmentsRequest>(r =>
                    r.Ukprn == ukprn && r.StandardReference == null),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _sut.GetAssessments(standard, ukprn) as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
            result.Value.Should().BeEquivalentTo(expectedResponse);

            _mediatorMock.Verify(m =>
                m.Send(It.Is<GetAssessmentsRequest>(r =>
                    r.Ukprn == ukprn && r.StandardReference == null),
                    It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
