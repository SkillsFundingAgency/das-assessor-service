using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Application.Api.TaskQueue;
using System;
using System.Net;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Providers
{
    [TestFixture]
    public class ProvidersControllerTests
    {
        private Mock<IBackgroundTaskQueue> _backgroundTaskQueue;
        private ProvidersController _sut;

        [SetUp]
        public void SetupTests()
        {
            _backgroundTaskQueue = new Mock<IBackgroundTaskQueue>();
            _sut = new ProvidersController(_backgroundTaskQueue.Object, Mock.Of<ILogger<ProvidersController>>());
        }


        [Test, AutoData]
        public void When_PostToRefreshProvidersCache_Then_BackgroundTaskIsQueued()
        {   
            // Act
            var controllerResult = _sut.RefreshProvidersCache() as ObjectResult;

            // Assert
            _backgroundTaskQueue.Verify(m => m.QueueBackgroundRequest(
                It.Is<UpdateProvidersCacheRequest>(p => p.UpdateType == ProvidersCacheUpdateType.RefreshExistingProviders), 
                "refresh providers cache", 
                It.IsAny<Action<object, TimeSpan, ILogger<TaskQueueHostedService>>>()), 
                Times.Once);
        }

        [Test]
        public void When_RefreshProvidersCacheHasNoErrors_Then_ReturnsAccepted()
        {
            // Act
            var controllerResult = _sut.RefreshProvidersCache() as ObjectResult;

            // Assert

            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.Accepted);
        }
    }
}
