using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Approvals
{
    [TestFixture]
    public class ApprovalsControllerTests
    {
        [Test]
        public async Task When_PostToImportingApprovals_Then_ImportApprovalsHandlerIsCalled()
        {
            // Arrange.

            var mockMediator = new Mock<IMediator>();
            var cut = new ApprovalsController(Mock.Of<ILogger<ApprovalsController>>(), mockMediator.Object);
            var importApprovalsRequest = new ImportApprovalsRequest();

            // Act.

            var controllerResult = await cut.GatherAndStoreApprovals() as ObjectResult;

            // Assert.

            mockMediator.Verify(m => m.Send(importApprovalsRequest, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task When_GatherAndStoreApprovalsHasNoErrors_Then_ReturnOK()
        {
            // Arrange.

            var mockMediator = new Mock<IMediator>();
            var cut = new ApprovalsController(Mock.Of<ILogger<ApprovalsController>>(), mockMediator.Object);
            var importApprovalsRequest = new ImportApprovalsRequest();

            // Act.

            var controllerResult = await cut.GatherAndStoreApprovals() as ObjectResult;

            // Assert.

            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }
    }
}
