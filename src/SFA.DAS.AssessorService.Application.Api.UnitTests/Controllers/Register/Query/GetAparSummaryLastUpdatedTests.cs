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
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Register.Query
{
    public class GetAparSummaryLastUpdatedTests
    {
        private Mock<IBackgroundTaskQueue> _mockBackgroundTaskQueue;
        private static Mock<IMediator> _mediator;
        private static Mock<ILogger<RegisterQueryController>> _logger;
        private static RegisterQueryController _sut;
        private static object _result;
        private static DateTime _expectedLastUpdated;

        [SetUp]
        public async Task Arrange()
        {
            _mockBackgroundTaskQueue = new Mock<IBackgroundTaskQueue>();
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<RegisterQueryController>>();
            _expectedLastUpdated = DateTime.UtcNow;

            _mediator.Setup(m => m.Send(It.IsAny<GetAparSummaryLastUpdatedRequest>(), new CancellationToken()))
                .ReturnsAsync(_expectedLastUpdated);

            _sut = new RegisterQueryController(_mediator.Object, _mockBackgroundTaskQueue.Object, _logger.Object);

            _result = await _sut.GetAparSummaryLastUpdated();
        }

        [Test]
        public void GetAparSummaryLastUpdated_MediatorShouldSendGetAparSummaryLastUpdatedRequest_WhenCalled()
        {
            _mediator.Verify(m => m.Send(It.IsAny<GetAparSummaryLastUpdatedRequest>(), new CancellationToken()));
        }


        [Test]
        public void GetAparSummaryLastUpdated_ShouldReturnOk_WhenCalled()
        {
            _result.Should().BeOfType<OkObjectResult>();
        }

        [Test]
        public void GetAparSummaryLastUpdatedy_ResultsAreOfTypeDateTime_WhenCalled()
        {
            ((OkObjectResult)_result).Value.Should().BeOfType<DateTime>();
        }

        [Test]
        public void GetAparSummaryLastUpdated_ResultsMatchExpectedDateTime_WhenCalled()
        {
            var organisations = ((OkObjectResult)_result).Value as DateTime?;
            organisations.Should().Be(_expectedLastUpdated);
        }
    }
}
