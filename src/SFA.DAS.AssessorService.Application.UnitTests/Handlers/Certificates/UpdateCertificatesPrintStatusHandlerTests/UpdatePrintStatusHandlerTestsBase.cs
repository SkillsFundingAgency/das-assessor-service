using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.UpdateCertificatesPrintStatusHandlerTests
{
    public class UpdateCertificatesPrintStatusHandlerTestsBase
    {
        protected UpdateCertificatesPrintStatusHandler _sut;

        protected Mock<ICertificateRepository> _certificateRepository;
        protected Mock<ICertificateBatchLogRepository> _certificateBatchLogRepository;
        protected Mock<IMediator> _mediator;
        protected Mock<ILogger<UpdateCertificatesPrintStatusHandler>> _logger;

        protected static int _batch111 = 111;
        protected static int _batch222 = 222;

        protected static DateTime _batch111SentToPrinterAt = DateTime.UtcNow.AddDays(-3);

        protected static DateTime _batch222SentToPrinterAt = DateTime.UtcNow.AddDays(-3);
        protected static DateTime _batch222PrintedAt = DateTime.UtcNow.AddDays(-2);
        protected static DateTime _batch222PrintNotifiedAt = DateTime.UtcNow.AddDays(-2).AddHours(12);

        protected static DateTime _deliveredAt = DateTime.UtcNow.AddDays(-2).AddHours(3); // delivered after printing but before print notified
        protected static DateTime _notDeliveredAt = DateTime.UtcNow.AddDays(-2).AddHours(9); // not delivered after printing after print notified
        protected static DateTime _deliveryNotifiedAt = DateTime.UtcNow.AddDays(-1).AddHours(12);

        protected static string _certificateReference1 = "00000001";
        protected static string _certificateReference2 = "00000002";
        protected static string _certificateReference3 = "00000003";
        protected static string _certificateReference4 = "00000004";
        protected static string _certificateReference5 = "00000005";
        
        protected static string _certificateReferenceReprintedAfterPrinted = "00000006";
        protected static string _certificateReferenceDeletedAfterPrinted = "00000007";

        protected static string _certificateNotDeliveredReason1 = "The address given did not match the building street address";

        protected BatchLog _sentToPrinterBatchLog = new BatchLog { Id = Guid.NewGuid(), BatchNumber = _batch111 };
        protected BatchLog _printedBatchLog = new BatchLog { Id = Guid.NewGuid(), BatchNumber = _batch222 };

        public void BaseArrange()
        {
            _certificateRepository = new Mock<ICertificateRepository>();
            _certificateBatchLogRepository = new Mock<ICertificateBatchLogRepository>();
            _mediator = new Mock<IMediator>();

            _certificateRepository.Setup(r => r.GetCertificate(It.IsIn(_certificateReference1)))
                .Returns((string certificateReference) => Task.FromResult(
                    new Certificate
                    {
                        Id = Guid.NewGuid(),
                        CertificateReference = certificateReference,
                        UpdatedAt = _batch111SentToPrinterAt,
                        BatchNumber = _batch111,
                        Status = CertificateStatus.SentToPrinter,
                        ToBePrinted = _batch111SentToPrinterAt
                    }));

            _certificateBatchLogRepository.Setup(r => r.GetCertificateBatchLog(It.IsIn(_certificateReference1), _batch111))
               .Returns((string certificateReference, int batchNumber) => Task.FromResult(
                    new CertificateBatchLog
                    {
                        Id = Guid.NewGuid(),
                        CertificateReference = certificateReference,
                        UpdatedAt = _batch111SentToPrinterAt,
                        BatchNumber = batchNumber,
                        Status = CertificateStatus.SentToPrinter,
                        StatusAt = _batch111SentToPrinterAt
                    }));

            _certificateRepository.Setup(r => r.GetCertificate(It.IsIn(_certificateReference2, _certificateReference3)))
                .Returns((string certificateReference) => Task.FromResult(
                    new Certificate
                    {
                        Id = Guid.NewGuid(),
                        CertificateReference = certificateReference,
                        UpdatedAt = _batch222PrintNotifiedAt,
                        BatchNumber = _batch222,
                        Status = CertificateStatus.Printed,
                        ToBePrinted = _batch222SentToPrinterAt
                    }));

            _certificateBatchLogRepository.Setup(r => r.GetCertificateBatchLog(It.IsIn(_certificateReference2, _certificateReference3), _batch222))
             .Returns((string certificateReference, int batchNumber) => Task.FromResult(
                  new CertificateBatchLog
                  {
                      Id = Guid.NewGuid(),
                      CertificateReference = certificateReference,
                      UpdatedAt = _batch222PrintNotifiedAt,
                      BatchNumber = batchNumber,
                      Status = CertificateStatus.Printed,
                      StatusAt = _batch222PrintedAt
                  }));

            _certificateRepository.Setup(r => r.GetCertificate(It.IsIn(_certificateReference4)))
                .Returns((string certificateReference) => Task.FromResult<Certificate>(null));

            _certificateRepository.Setup(r => r.GetCertificate(It.IsIn(_certificateReference5)))
              .Returns((string certificateReference) => Task.FromResult(
                  new Certificate
                  {
                      Id = Guid.NewGuid(),
                      CertificateReference = certificateReference,
                      UpdatedAt = DateTime.UtcNow.AddDays(1),
                      BatchNumber = _batch222,
                      Status = CertificateStatus.Delivered
                  }));

            _certificateBatchLogRepository.Setup(r => r.GetCertificateBatchLog(It.IsIn(_certificateReference5), _batch222))
              .Returns((string certificateReference, int batchNumber) => Task.FromResult(
                  new CertificateBatchLog
                  {
                      Id = Guid.NewGuid(),
                      CertificateReference = certificateReference,
                      UpdatedAt = _deliveryNotifiedAt,
                      BatchNumber = batchNumber,
                      Status = CertificateStatus.Delivered,
                      StatusAt = _deliveredAt
                  }));


            _certificateRepository.Setup(r => r.GetCertificate(It.IsIn(_certificateReferenceReprintedAfterPrinted)))
                .Returns((string certificateReference) => Task.FromResult(
                    new Certificate
                    {
                        Id = Guid.NewGuid(),
                        CertificateReference = certificateReference,
                        UpdatedAt = DateTime.UtcNow,
                        BatchNumber = null,
                        Status = CertificateStatus.Reprint,
                    }));

            _certificateBatchLogRepository.Setup(r => r.GetCertificateBatchLog(It.IsIn(_certificateReferenceReprintedAfterPrinted), _batch222))
              .Returns((string certificateReference, int batchNumber) => Task.FromResult(
                  new CertificateBatchLog
                  {
                      Id = Guid.NewGuid(),
                      CertificateReference = certificateReference,
                      UpdatedAt = _batch222PrintNotifiedAt,
                      BatchNumber = batchNumber,
                      Status = CertificateStatus.Printed,
                      StatusAt = _batch222PrintedAt
                  }));

            _certificateRepository.Setup(r => r.GetCertificate(It.IsIn(_certificateReferenceDeletedAfterPrinted)))
                .Returns((string certificateReference) => Task.FromResult(
                    new Certificate
                    {
                        Id = Guid.NewGuid(),
                        CertificateReference = certificateReference,
                        UpdatedAt = DateTime.UtcNow,
                        BatchNumber = _batch222,
                        Status = CertificateStatus.Deleted
                    }));

            _certificateBatchLogRepository.Setup(r => r.GetCertificateBatchLog(It.IsIn(_certificateReferenceDeletedAfterPrinted), _batch222))
                .Returns((string certificateReference, int batchNumber) => Task.FromResult(
                    new CertificateBatchLog
                    {
                        Id = Guid.NewGuid(),
                        CertificateReference = certificateReference,
                        UpdatedAt = _batch222PrintNotifiedAt,
                        BatchNumber = batchNumber,
                        Status = CertificateStatus.Printed,
                        StatusAt = _batch222PrintedAt
                    }));

            _certificateRepository.Setup(r => r.UpdateSentToPrinter(It.IsAny<Certificate>(), It.IsAny<int>(), It.IsAny<DateTime>()))
                .Returns(Task.CompletedTask);

            _mediator.Setup(r => r.Send(It.Is<GetForBatchNumberBatchLogRequest>(p => p.BatchNumber == _batch111 || p.BatchNumber == _batch222), It.IsAny<CancellationToken>()))
                .Returns((GetForBatchNumberBatchLogRequest request, CancellationToken token) => Task.FromResult(new BatchLogResponse { BatchNumber = request.BatchNumber }));

            _mediator.Setup(r => r.Send(It.Is<GetForBatchNumberBatchLogRequest>(p => p.BatchNumber != _batch111 && p.BatchNumber != _batch222), It.IsAny<CancellationToken>()))
                .Returns((GetForBatchNumberBatchLogRequest request, CancellationToken token) => Task.FromResult<BatchLogResponse>(null));

            _logger = new Mock<ILogger<UpdateCertificatesPrintStatusHandler>>();

            _sut = new UpdateCertificatesPrintStatusHandler(_certificateRepository.Object, _certificateBatchLogRepository.Object, _mediator.Object, _logger.Object);
        }
    }
}
