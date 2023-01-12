using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Api.Controllers;
using SFA.DAS.AssessorService.Domain.Consts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.UnitTests.Controllers.Certificates
{
    public class When_UpdatePrintStatus_Called
    {
        private CertificateController _sut;
        private Mock<IMediator> _mediator;
        private IActionResult _result;

        private CertificatePrintStatusUpdate _certificatePrintStatusUpdateDelivered = new CertificatePrintStatusUpdate
        {
            BatchNumber = 1,
            CertificateReference = "00000001",
            Status = CertificateStatus.Delivered,
            StatusAt = DateTime.UtcNow
        };

        private CertificatePrintStatusUpdate _certificatePrintStatusUpdateNotDelivered = new CertificatePrintStatusUpdate
        {
            BatchNumber = 1,
            CertificateReference = "00000002",
            ReasonForChange = "Unable to deliver to door",
            Status = CertificateStatus.NotDelivered,
            StatusAt = DateTime.UtcNow
        };

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();

            _mediator.Setup(q => q.Send(It.IsAny<CertificatePrintStatusUpdateRequest>(), new CancellationToken()))
                .Returns(Task.FromResult(new ValidationResponse()));

            _sut = new CertificateController(_mediator.Object);
        }

        [Test]
        public async Task ThenShouldReturnOk_ForDelivered()
        {
            // Act
            _result = await _sut.UpdatePrintStatus(new CertificatePrintStatusUpdateRequest
            {
                BatchNumber = _certificatePrintStatusUpdateDelivered.BatchNumber,
                CertificateReference = _certificatePrintStatusUpdateDelivered.CertificateReference,
                Status = _certificatePrintStatusUpdateDelivered.Status,
                StatusAt = _certificatePrintStatusUpdateDelivered.StatusAt
            });

            // Assert
            var result = _result as OkObjectResult;
            result.Should().NotBeNull();
        }

        [Test]
        public async Task ThenShouldCallUpdateCertificatesPrintStatusRequest_ForDelivered()
        {
            // Act
            _result = await _sut.UpdatePrintStatus(new CertificatePrintStatusUpdateRequest
            {
                BatchNumber = _certificatePrintStatusUpdateDelivered.BatchNumber,
                CertificateReference = _certificatePrintStatusUpdateDelivered.CertificateReference,
                Status = _certificatePrintStatusUpdateDelivered.Status,
                StatusAt = _certificatePrintStatusUpdateDelivered.StatusAt
            });

            // Assert
            _mediator.Verify(x => x.Send(
                It.Is<CertificatePrintStatusUpdateRequest>(s => JsonConvert.SerializeObject(s).Equals(JsonConvert.SerializeObject(_certificatePrintStatusUpdateDelivered))),
                It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [Test]
        public async Task ThenShouldReturnOk_ForNotDelivered()
        {
            // Act
            _result = await _sut.UpdatePrintStatus(new CertificatePrintStatusUpdateRequest
            {
                BatchNumber = _certificatePrintStatusUpdateNotDelivered.BatchNumber,
                CertificateReference = _certificatePrintStatusUpdateNotDelivered.CertificateReference,
                ReasonForChange = _certificatePrintStatusUpdateNotDelivered.ReasonForChange,
                Status = _certificatePrintStatusUpdateNotDelivered.Status,
                StatusAt = _certificatePrintStatusUpdateNotDelivered.StatusAt
            });

            // Assert
            var result = _result as OkObjectResult;
            result.Should().NotBeNull();
        }

        [Test]
        public async Task ThenShouldCallUpdateCertificatesPrintStatusRequest_ForNotDelivered()
        {
            // Act
            _result = await _sut.UpdatePrintStatus(new CertificatePrintStatusUpdateRequest
            {
                BatchNumber = _certificatePrintStatusUpdateNotDelivered.BatchNumber,
                CertificateReference = _certificatePrintStatusUpdateNotDelivered.CertificateReference,
                ReasonForChange = _certificatePrintStatusUpdateNotDelivered.ReasonForChange,
                Status = _certificatePrintStatusUpdateNotDelivered.Status,
                StatusAt = _certificatePrintStatusUpdateNotDelivered.StatusAt
            });

            // Assert
            _mediator.Verify(x => x.Send(
                It.Is<CertificatePrintStatusUpdateRequest>(s => JsonConvert.SerializeObject(s).Equals(JsonConvert.SerializeObject(_certificatePrintStatusUpdateNotDelivered))),
                It.IsAny<CancellationToken>()),
                Times.Once());
        }
    }
}
