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
    public class When_UpdateBatchLogSentToPrinter_Called
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
            
            _mediator.Setup(q => q.Send(It.IsAny<UpdateBatchLogSentToPrinterRequest>(), new CancellationToken()))
                .Returns(Task.FromResult(new ValidationResponse()));
            
            _sut = new BatchLogController(_mediator.Object);
            _result = await _sut.UpdateBatchLogSentToPrinter(_batchNumber, new UpdateBatchLogSentToPrinterRequest 
            { 
                BatchNumber = _batchNumber, 
                BatchCreated = _utcNow,
                NumberOfCertificates = 1,
                NumberOfCoverLetters = 1,
                CertificatesFileName = "TestFileName",
                FileUploadStartTime = _utcNow,
                FileUploadEndTime = _utcNow});
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
                It.Is<UpdateBatchLogSentToPrinterRequest>(s => 
                    s.BatchNumber == _batchNumber && 
                    s.BatchCreated == _utcNow &&
                    s.NumberOfCertificates == 1 &&
                    s.NumberOfCoverLetters == 1 &&
                    s.CertificatesFileName == "TestFileName" &&
                    s.FileUploadStartTime == _utcNow &&
                    s.FileUploadEndTime == _utcNow), 
                It.IsAny<CancellationToken>()), 
                Times.Once());
        }
    }
}
