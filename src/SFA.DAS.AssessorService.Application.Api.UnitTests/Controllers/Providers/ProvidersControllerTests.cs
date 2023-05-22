using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Application.Api.TaskQueue;
using System.Net;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Providers
{
    [TestFixture]
    public class ProvidersControllerTests
    {
        private Mock<IBackgroundTaskQueue> _mockBackgroundTaskQueue;
        private ProvidersController _sut;

        [SetUp]
        public void SetupTests()
        {
            _mockBackgroundTaskQueue = new Mock<IBackgroundTaskQueue>();
            _sut = new ProvidersController(_mockBackgroundTaskQueue.Object, Mock.Of<ILogger<ProvidersController>>());
        }


        [Test, AutoData]
        public void When_PostToRefreshProvidersCache_Then_BackgroundTaskIsQueued()
        {   
            // Act
            var controllerResult = _sut.RefreshProvidersCache() as ObjectResult;

            // Assert
            _mockBackgroundTaskQueue.Verify(m => m.QueueBackgroundRequest(
                It.Is<UpdateProvidersCacheRequest>(p => p.UpdateType == ProvidersCacheUpdateType.RefreshExistingProviders), "refresh providers cache"), 
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
