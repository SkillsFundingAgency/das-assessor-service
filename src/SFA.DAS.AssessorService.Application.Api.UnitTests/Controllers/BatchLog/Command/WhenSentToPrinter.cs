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
    public class WhenSentToPrinter
    {
        private BatchLogController _sut;
        private Mock<IMediator> _mediator;
        private IActionResult _result;

        private int _batchNumber = 1;
        private List<string> _certificateReference = new List<string> { "00000001", "00000002" };

        [SetUp]
        public async Task Arrange()
        {
            _mediator = new Mock<IMediator>();            
            
            _mediator.Setup(q => q.Send(It.IsAny<SentToPrinterBatchLogRequest>(), new CancellationToken()))
                .Returns(Task.FromResult(new ValidationResponse()));
            
            _sut = new BatchLogController(_mediator.Object);
            _result = await _sut.SentToPrinter(new SentToPrinterBatchLogRequest { BatchNumber = _batchNumber, CertificateReferences = _certificateReference });
        }

        [Test]
        public void ThenShouldReturnOk()
        {
            var result = _result as OkObjectResult;
            result.Should().NotBeNull();
        }

        [Test]
        public void ThenShouldCallSentToPrinterRequest()
        {
            _mediator.Verify(x => x.Send(
                It.Is<SentToPrinterBatchLogRequest>(s => s.BatchNumber == _batchNumber && s.CertificateReferences.SequenceEqual(_certificateReference)), 
                It.IsAny<CancellationToken>()), 
                Times.Once());
        }
    }
}
