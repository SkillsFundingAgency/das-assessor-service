using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.UpdateCertificatesPrintStatusHandlerTests
{
    public class UpdateCertificatesPrintStatusHandlerTestsBase
    {
        protected CertificatePrintStatusUpdateHandler _sut;

        protected Mock<ICertificateRepository> _certificateRepository;
        protected Mock<ICertificateBatchLogRepository> _certificateBatchLogRepository;
        protected Mock<IMediator> _mediator;
        protected Mock<ILogger<CertificatePrintStatusUpdateHandler>> _logger;

        protected static int _batch111 = 111;
        protected static int _batch222 = 222;

        protected static DateTime _batch111SentToPrinterAt = DateTime.UtcNow.AddDays(-3);

        protected static DateTime _batch222SentToPrinterAt = DateTime.UtcNow.AddDays(-3);
        protected static DateTime _batch222PrintedAt = _batch222SentToPrinterAt.AddDays(1);
        protected static DateTime _batch222PrintNotifiedAt = _batch222PrintedAt.AddHours(12);

        protected static DateTime _deliveredAt = _batch222PrintedAt.AddHours(3); // delivered after printing but before print notified
        protected static DateTime _deliveredAtNotifiedAt = _deliveredAt.AddHours(24);

        protected static DateTime _notDeliveredAt = _batch222PrintNotifiedAt.AddHours(3); // not delivered after printing after print notified

        protected static string _certificateReference1 = "00000001";
        protected static string _certificateReference2 = "00000002";
        protected static string _certificateReference3 = "00000003";
        protected static string _certificateReference4 = "00000004";
        protected static string _certificateReference5 = "00000005";
        
        protected static string _certificateReferenceReprintedAfterPrinted = "00000006";
        protected static DateTime _certificateReferenceReprintedAfterPrintedAt = _batch222PrintedAt.AddHours(6);

        protected static string _certificateReferenceDeletedAfterPrinted = "00000007";
        protected static DateTime _certificateReferenceDeletedAfterPrintedAt = _batch222PrintedAt.AddHours(12);

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
                      UpdatedAt = _deliveredAtNotifiedAt,
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
                        UpdatedAt = _certificateReferenceReprintedAfterPrintedAt,
                        BatchNumber = null,
                        Status = CertificateStatus.Reprint,
                    }));

            _certificateBatchLogRepository.Setup(r => r.GetCertificateBatchLog(It.IsIn(_certificateReferenceReprintedAfterPrinted), _batch111))
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
                        UpdatedAt = _certificateReferenceDeletedAfterPrintedAt,
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

            _certificateBatchLogRepository.Setup(r => r.GetCertificateBatchLog(It.IsIn(_certificateReference5), _batch111))
              .Returns((string certificateReference, int batchNumber) => Task.FromResult(
                  new CertificateBatchLog
                  {
                      Id = Guid.NewGuid(),
                      CertificateReference = certificateReference,
                      Status = CertificateStatus.Delivered,
                      StatusAt = DateTime.UtcNow.AddDays(1)
                  }));

            _certificateRepository.Setup(r => r.GetCertificate(It.IsIn(_certificateReference4)))
                .Returns((string certificateReference) => Task.FromResult<Certificate>(null));

            _certificateRepository.Setup(r => r.UpdatePrintStatus(It.IsAny<Certificate>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(Task.CompletedTask);

            _mediator.Setup(r => r.Send(It.Is<GetBatchLogRequest>(p => new[] { _batch111, _batch222 }.Contains(p.BatchNumber)), It.IsAny<CancellationToken>()))
                .Returns((GetBatchLogRequest request, CancellationToken token) => Task.FromResult(new BatchLogResponse { BatchNumber = request.BatchNumber }));

            _mediator.Setup(r => r.Send(It.Is<GetBatchLogRequest>(p => !(new[] { _batch111, _batch222 }.Contains(p.BatchNumber))), It.IsAny<CancellationToken>()))
                .Returns((GetBatchLogRequest request, CancellationToken token) => Task.FromResult<BatchLogResponse>(null));

            _logger = new Mock<ILogger<CertificatePrintStatusUpdateHandler>>();

            _sut = new CertificatePrintStatusUpdateHandler(_certificateRepository.Object, _certificateBatchLogRepository.Object, _mediator.Object, _logger.Object);
        }
    }
}
