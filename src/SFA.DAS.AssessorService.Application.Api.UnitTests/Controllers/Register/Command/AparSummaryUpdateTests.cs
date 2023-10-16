using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Application.Api.TaskQueue;
using System.Net;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Register.Query
{

    [TestFixture]
    public class AparSummaryUpdateTests
    {
        private Mock<IBackgroundTaskQueue> _backgroundTaskQueue;
        private Mock<IMediator> _mediator;
        private Mock<ILogger<RegisterQueryController>> _logger;
        private RegisterQueryController _sut;

        [SetUp]
        public void Arrange()
        {
            _backgroundTaskQueue = new Mock<IBackgroundTaskQueue>();
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<RegisterQueryController>>();
            
            _sut = new RegisterQueryController(_mediator.Object, _backgroundTaskQueue.Object, _logger.Object);
        }

        [Test, AutoData]
        public void When_PostToRefreshProvidersCache_Then_BackgroundTaskIsQueued()
        {
            // Act
            var controllerResult = _sut.AparSummaryUpdate() as ObjectResult;

            // Assert
            _backgroundTaskQueue.Verify(m => m.QueueBackgroundRequest(
                It.IsAny<AparSummaryUpdateRequest>(),
                "update APAR summary", "there were {0} changes made to APAR for EPAOs"),
                Times.Once);
        }

        [Test]
        public void When_RefreshProvidersCacheHasNoErrors_Then_ReturnsAccepted()
        {
            // Act
            var controllerResult = _sut.AparSummaryUpdate() as ObjectResult;

            // Assert

            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.Accepted);
        }
    }
}
