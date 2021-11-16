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
using SFA.DAS.AssessorService.Application.Handlers.Approvals;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Approvals
{
    [TestFixture]
    public class ApprovalsControllerTests
    {
        [Test, AutoData]
        public async Task When_PostToImportingApprovals_Then_ImportApprovalsHandlerIsCalled(Mock<IMediator> mockMediator)
        {
            // Arrange.
            var sut = SetupApprovalsController(mediator: mockMediator);

            // Act.

            var controllerResult = await sut.GatherAndStoreApprovals() as ObjectResult;

            // Assert.

            mockMediator.Verify(m => m.Send(It.IsAny<ImportApprovalsRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task When_GatherAndStoreApprovalsHasNoErrors_Then_ReturnOK()
        {
            // Arrange.
            var sut = SetupApprovalsController();

            // Act.

            var controllerResult = await sut.GatherAndStoreApprovals() as ObjectResult;

            // Assert.

            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Test, AutoData]
        public async Task When_GettingApprovalsLearnerRecord_ThenGetAppovalsLearnerRecordHandlerIsCalled(int stdCode, long uln, Mock<IMediator> mockMediator)
        {
            // Arrange.
            var sut = SetupApprovalsController(mediator: mockMediator);
            
            // Act.
            var controllerResult = await sut.GetLearner(stdCode, uln) as ObjectResult;

            // Assert.
            mockMediator.Verify(m => m.Send(It.Is<GetApprovalsLearnerRecordRequest>(s => s.StdCode == stdCode && s.Uln == uln), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test, AutoData]
        public async Task When_GettingAprovalsLearnerRecordHasNoErrors_Then_ReturnOK(int stdCode, long uln, ApprovalsLearnerResult result)
        {
            // Arrange.
            var sut = SetupApprovalsController(result);

            // Act.
            var controllerResult = await sut.GetLearner(stdCode, uln) as ObjectResult;

            // Assert.
            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Test, AutoData]
        public async Task When_GettingAprovalsLearnerRecordNotFound_Then_ReturnNotFound(int stdCode, long uln)
        {
            // Arrange.
            var sut = SetupApprovalsController();

            // Act.
            var controllerResult = await sut.GetLearner(stdCode, uln) as NotFoundResult;

            // Assert.
            controllerResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        private ApprovalsController SetupApprovalsController(ApprovalsLearnerResult result = null, Mock<IMediator> mediator = null)
        {
            var mockMediator = mediator ?? new Mock<IMediator>();
            mockMediator.Setup(s => s.Send(It.IsAny<GetApprovalsLearnerRecordRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);
            var sut = new ApprovalsController(Mock.Of<ILogger<ApprovalsController>>(), mockMediator.Object);

            return sut;
        }
    }
}
