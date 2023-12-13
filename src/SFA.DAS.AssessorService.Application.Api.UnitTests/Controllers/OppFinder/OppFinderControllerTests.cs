using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Application.Api.TaskQueue;
using System;
using System.Net;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.OppFinder
{
    [TestFixture]
    public class OppFinderControllerTests
    {
        private Mock<IMediator> _mockMediator;
        private Mock<IBackgroundTaskQueue> _mockBackgroundTaskQueue;
        private OppFinderController _sut;

        [SetUp]
        public void SetupTests()
        {
            _mockMediator = new Mock<IMediator>();
            _mockBackgroundTaskQueue = new Mock<IBackgroundTaskQueue>();
            _sut = new OppFinderController(_mockMediator.Object, _mockBackgroundTaskQueue.Object, Mock.Of<ILogger<OppFinderController>>());
        }


        [Test, AutoData]
        public void When_PostToUpdateStandardSummary_Then_BackgroundTaskIsQueued()
        {
            // Act
            var controllerResult = _sut.UpdateStandardSummary() as ObjectResult;

            // Assert
            _mockBackgroundTaskQueue.Verify(m => m.QueueBackgroundRequest(
                It.IsAny<UpdateStandardSummaryRequest>(), "update standard summary", 
                It.IsAny<Action<object, TimeSpan, ILogger<TaskQueueHostedService>>>()), 
                Times.Once);
        }

        [Test]
        public void When_UpdateStandardSummaryHasNoErrors_Then_ReturnsAccepted()
        {
            // Act
            var controllerResult = _sut.UpdateStandardSummary() as ObjectResult;

            // Assert

            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.Accepted);
        }
    }
}
