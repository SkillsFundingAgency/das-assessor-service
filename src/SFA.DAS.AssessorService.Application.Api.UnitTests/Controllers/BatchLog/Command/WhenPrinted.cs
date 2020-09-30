using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.BatchLog
{
    public class WhenPrinted
    {
        private BatchLogController _sut;
        private Mock<IMediator> _mediator;
        private IActionResult _result;

        private int _batchNumber = 1;
        private DateTime _printedAt = DateTime.UtcNow;

        [SetUp]
        public async Task Arrange()
        {
            _mediator = new Mock<IMediator>();            
            
            _mediator.Setup(q => q.Send(It.IsAny<SentToPrinterBatchLogRequest>(), new CancellationToken()))
                .Returns(Task.FromResult(new ValidationResponse()));
            
            _sut = new BatchLogController(_mediator.Object);
            _result = await _sut.Printed(new PrintedBatchLogRequest { BatchNumber = _batchNumber, PrintedAt = _printedAt });
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
                It.Is<PrintedBatchLogRequest>(s => s.BatchNumber == _batchNumber && s.PrintedAt == _printedAt), 
                It.IsAny<CancellationToken>()), 
                Times.Once());
        }
    }
}
