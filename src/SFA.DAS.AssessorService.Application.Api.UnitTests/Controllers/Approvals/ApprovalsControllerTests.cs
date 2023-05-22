using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Learner;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Application.Api.TaskQueue;
using SFA.DAS.AssessorService.Application.Handlers.Approvals;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Approvals
{
    [TestFixture]
    public class ApprovalsControllerTests
    {
        private Mock<IMediator> _mockMediator;
        private Mock<IBackgroundTaskQueue> _mockBackgroundTaskQueue;
        private ApprovalsController _sut;

        [SetUp]
        public void SetupTests()
        {
            _mockMediator = new Mock<IMediator>();
            _mockBackgroundTaskQueue = new Mock<IBackgroundTaskQueue>();
            _sut = new ApprovalsController(_mockMediator.Object, _mockBackgroundTaskQueue.Object, Mock.Of<ILogger<ApprovalsController>>());
        }


        [Test]
        public void When_PostToImportingApprovals_Then_BackgroundTaskIsQueued()
        {
            // Act
            var controllerResult = _sut.GatherAndStoreApprovals() as ObjectResult;

            // Assert
            _mockBackgroundTaskQueue.Verify(m => m.QueueBackgroundRequest(It.IsAny<ImportApprovalsRequest>(), "gather and store approvals"), Times.Once);
        }

        [Test]
        public void When_GatherAndStoreApprovalsHasNoErrors_Then_ReturnsAccepted()
        {
            // Act
            var controllerResult = _sut.GatherAndStoreApprovals() as ObjectResult;

            // Assert

            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.Accepted);
        }

        [Test, AutoData]
        public async Task When_GettingApprovalsLearnerRecord_ThenGetAppovalsLearnerRecordHandlerIsCalled(int stdCode, long uln)
        {
            // Arrange

            // Act
            var controllerResult = await _sut.GetLearner(stdCode, uln) as ObjectResult;

            // Assert
            _mockMediator.Verify(m => m.Send(It.Is<GetApprovalsLearnerRecordRequest>(s => s.StdCode == stdCode && s.Uln == uln), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test, AutoData]
        public async Task When_GettingAprovalsLearnerRecordHasNoErrors_Then_ReturnOK(int stdCode, long uln, ApprovalsLearnerResult result)
        {
            // Arrange
            _mockMediator.Setup(s => s.Send(It.IsAny<GetApprovalsLearnerRecordRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);

            // Act
            var controllerResult = await _sut.GetLearner(stdCode, uln) as ObjectResult;

            // Assert
            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Test, AutoData]
        public async Task When_GettingAprovalsLearnerRecordNotFound_Then_ReturnNotFound(int stdCode, long uln)
        {
            // Arrange
            _mockMediator.Setup(s => s.Send(It.IsAny<GetApprovalsLearnerRecordRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync((ApprovalsLearnerResult)null);

            // Act
            var controllerResult = await _sut.GetLearner(stdCode, uln) as NotFoundResult;

            // Assert
            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}
