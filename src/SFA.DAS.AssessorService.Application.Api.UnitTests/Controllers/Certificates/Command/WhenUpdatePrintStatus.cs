using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Domain.Consts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Certificates
{
    public class WhenUpdatePrintStatus
    {
        private CertificateController _sut;
        private Mock<IMediator> _mediator;
        private IActionResult _result;

        private List<CertificatePrintStatus> _certificatePrintStatuses = new List<CertificatePrintStatus>
        {
            new CertificatePrintStatus
            {
                BatchNumber = 1,
                CertificateReference = "00000001",
                Status = CertificateStatus.Delivered,
                StatusChangedAt = DateTime.UtcNow
            },
            new CertificatePrintStatus
            {
                BatchNumber = 1,
                CertificateReference = "00000002",
                Status = CertificateStatus.NotDelivered,
                StatusChangedAt = DateTime.UtcNow
            }
        };
        
        [SetUp]
        public async Task Arrange()
        {
            _mediator = new Mock<IMediator>();            
            
            _mediator.Setup(q => q.Send(It.IsAny<SentToPrinterBatchLogRequest>(), new CancellationToken()))
                .Returns(Task.FromResult(new ValidationResponse()));
            
            _sut = new CertificateController(_mediator.Object);
            _result = await _sut.UpdatePrintStatus(new UpdateCertificatesPrintStatusRequest { CertificatePrintStatuses = _certificatePrintStatuses });
        }

        [Test]
        public void ThenShouldReturnOk()
        {
            var result = _result as OkObjectResult;
            result.Should().NotBeNull();
        }

        [Test]
        public void ThenShouldCallUpdateCertificatesPrintStatusRequest()
        {
            _mediator.Verify(x => x.Send(
                It.Is<UpdateCertificatesPrintStatusRequest>(s => s.CertificatePrintStatuses.SequenceEqual(_certificatePrintStatuses)), 
                It.IsAny<CancellationToken>()), 
                Times.Once());
        }
    }
}
