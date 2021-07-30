using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Api.Controllers.Apply;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Application
{
    public class ApplicationControllerTests
    {
        private Mock<IMediator> _mockMediator;
        private Mock<ILogger<ApplicationController>> _mockLogger;

        private ApplicationController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockMediator = new Mock<IMediator>();
            _mockLogger = new Mock<ILogger<ApplicationController>>();

            _controller = new ApplicationController(_mockLogger.Object, _mockMediator.Object);
        }

        [Test]
        public async Task WhenPostingDeleteApplications_ThenApplicastionsDeleted()
        {
            // Arrange
            var request = new DeleteApplicationsRequest();

            // Act
            var controllerResult = await _controller.DeleteApplications(request) as NoContentResult;

            // Assert
            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.NoContent);

            _mockMediator.Verify(x => x.Send(request, It.IsAny<CancellationToken>()));
        }
    }
}
