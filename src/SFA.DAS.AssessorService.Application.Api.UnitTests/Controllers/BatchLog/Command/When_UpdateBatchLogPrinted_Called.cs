using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.BatchLogs;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.BatchLog
{
    public class When_UpdateBatchLogPrinted_Called
    {
        private BatchLogController _sut;
        private Mock<IMediator> _mediator;
        private IActionResult _result;

        private DateTime _utcNow = DateTime.UtcNow;
        private int _batchNumber = 1;

        [SetUp]
        public async Task Arrange()
        {
            _mediator = new Mock<IMediator>();

            _mediator.Setup(q => q.Send(It.IsAny<UpdateBatchLogPrintedRequest>(), new CancellationToken()))
                .Returns(Task.FromResult(new ValidationResponse()));

            _sut = new BatchLogController(_mediator.Object);
            _result = await _sut.UpdateBatchLogPrinted(_batchNumber, new UpdateBatchLogPrintedRequest
            {
                BatchNumber = _batchNumber,
                BatchDate = _utcNow,
                PostalContactCount = 1,
                TotalCertificateCount = 1,
                PrintedDate = _utcNow,
                DateOfResponse = _utcNow
            });
        }

        [Test]
        public void ThenShouldReturnOk()
        {
            var result = _result as OkObjectResult;
            result.Should().NotBeNull();
        }

        [Test]
        public void ThenShouldCallPrintedRequest()
        {
            _mediator.Verify(x => x.Send(
                It.Is<UpdateBatchLogPrintedRequest>(s =>
                    s.BatchNumber == _batchNumber &&
                    s.BatchDate == _utcNow &&
                    s.PostalContactCount == 1 &&
                    s.TotalCertificateCount == 1 &&
                    s.PrintedDate == _utcNow &&
                    s.DateOfResponse == _utcNow),
                It.IsAny<CancellationToken>()),
                Times.Once());
        }
    }
}
