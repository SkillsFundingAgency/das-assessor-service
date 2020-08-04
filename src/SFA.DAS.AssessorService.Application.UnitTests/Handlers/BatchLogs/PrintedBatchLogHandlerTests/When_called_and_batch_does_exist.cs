using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Handlers.BatchLogs;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.BatchLogs.PrintedBatchLogHandlerTests
{
    public class When_called_and_batch_does_exist
    {
        private Mock<IBatchLogQueryRepository> _batchLogQueryRepository;
        private Mock<ICertificateRepository> _certificateRepository;
        private Mock<IMediator> _mediator;
        private Mock<ILogger<PrintedBatchLogHandler>> _logger;

        private static int _batchNumber = 222;
        private static DateTime _printedAt = DateTime.UtcNow;

        private static string _certificateReference1 = "00000001";
        private static string _certificateReference2 = "00000002";

        private BatchLog _batchLog = new BatchLog { Id = Guid.NewGuid(), BatchNumber = _batchNumber };
        private List<Certificate> _certificates = new List<Certificate>
        {
            new Certificate
            {
                Id = Guid.NewGuid(),
                CertificateReference = _certificateReference1
            },
            new Certificate
            {
                Id = Guid.NewGuid(),
                CertificateReference = _certificateReference2
            }
        };

        private ValidationResponse _updateCertificatesPrintStatusRequestResponse = new ValidationResponse();
        private ValidationResponse _response;

        [SetUp]
        public async Task Arrange()
        {
            MappingBootstrapper.Initialize();

            _batchLogQueryRepository = new Mock<IBatchLogQueryRepository>();
            _batchLogQueryRepository.Setup(r => r.GetForBatchNumber(It.IsAny<int>())).Returns(Task.FromResult(_batchLog));

            _certificateRepository = new Mock<ICertificateRepository>();
            _certificateRepository.Setup(r => r.GetCertificatesForBatchLog(It.IsAny<int>())).Returns(Task.FromResult(_certificates));

            _mediator = new Mock<IMediator>();
            _mediator.Setup(r => r.Send(It.IsAny<UpdateCertificatesPrintStatusRequest>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(_updateCertificatesPrintStatusRequestResponse));
            _logger = new Mock<ILogger<PrintedBatchLogHandler>>();

            var sut = new PrintedBatchLogHandler(_batchLogQueryRepository.Object, _certificateRepository.Object, _mediator.Object, _logger.Object);

            _response = await sut.Handle(new PrintedBatchLogRequest { BatchNumber = _batchNumber, PrintedAt = _printedAt }, new CancellationToken());
        }

        [Test]
        public void Then_validation_response_is_valid_true()
        {
            _response.IsValid.Should().Be(true);
            _response.Errors.Count.Should().Be(0);
        }

        [Test]
        public void Then_certificate_update_print_status_message_is_sent()
        {
            _mediator.Verify(r => r.Send(It.Is<UpdateCertificatesPrintStatusRequest>(t => t.CertificatePrintStatuses.Count == 2), It.IsAny<CancellationToken>()));
            
            _mediator.Verify(r => r.Send(It.Is<UpdateCertificatesPrintStatusRequest>(t => t.CertificatePrintStatuses.Exists(p => CompareCertificatePrintStatus(p, new CertificatePrintStatus
            {
                CertificateReference = _certificateReference1,
                BatchNumber = _batchNumber,
                Status = CertificateStatus.Printed,
                StatusChangedAt = _printedAt
            }))), It.IsAny<CancellationToken>()));

            _mediator.Verify(r => r.Send(It.Is<UpdateCertificatesPrintStatusRequest>(t => t.CertificatePrintStatuses.Exists(p => CompareCertificatePrintStatus(p, new CertificatePrintStatus
            {
                CertificateReference = _certificateReference2,
                BatchNumber = _batchNumber,
                Status = CertificateStatus.Printed,
                StatusChangedAt = _printedAt
            }))), It.IsAny<CancellationToken>()));
        }

        private bool CompareCertificatePrintStatus(CertificatePrintStatus first, CertificatePrintStatus second)
        {
            return first.CertificateReference == second.CertificateReference &&
                first.BatchNumber == second.BatchNumber &&
                first.Status == second.Status &&
                first.StatusChangedAt == second.StatusChangedAt;
        }
    }
}
