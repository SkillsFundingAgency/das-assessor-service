using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Handlers.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.UpdateCertificatesPrintStatusHandlerTests
{
    public class UpdateCertificatesPrintStatusHandlerTestsBase
    {
        protected UpdateCertificatesPrintStatusHandler _sut;

        protected Mock<ICertificateRepository> _certificateRepository;
        protected Mock<IMediator> _mediator;
        protected Mock<ILogger<UpdateCertificatesPrintStatusHandler>> _logger;

        protected static int _batchNumber = 222;
        protected static DateTime _printedAt = DateTime.UtcNow;

        protected static string _certificateReference1 = "00000001";
        protected static string _certificateReference2 = "00000002";
        protected static string _certificateReference3 = "00000003";
        protected static string _certificateReferenceUpdateAfterSentToPrinter = "00000004";
        protected static string _certificateReferenceDeletedAfterSentToPrinter = "00000005";

        protected BatchLog _batchLog = new BatchLog { Id = Guid.NewGuid(), BatchNumber = _batchNumber };
        
        public void BaseArrange()
        {
            _certificateRepository = new Mock<ICertificateRepository>();
            _mediator = new Mock<IMediator>();

            _certificateRepository.Setup(r => r.GetCertificate(It.IsIn(_certificateReference1, _certificateReference2)))
                .Returns((string certificateReference) => Task.FromResult(
                    new Certificate
                    {
                        Id = Guid.NewGuid(),
                        CertificateReference = certificateReference,
                        UpdatedAt = DateTime.UtcNow.AddDays(-1)
                    }));

            _certificateRepository.Setup(r => r.GetCertificate(It.IsIn(_certificateReferenceUpdateAfterSentToPrinter)))
                .Returns((string certificateReference) => Task.FromResult(
                    new Certificate
                    {
                        Id = Guid.NewGuid(),
                        CertificateReference = certificateReference,
                        Status = CertificateStatus.Submitted,
                        UpdatedAt = DateTime.UtcNow.AddDays(1)
                    }));

            _certificateRepository.Setup(r => r.GetCertificate(It.IsIn(_certificateReferenceDeletedAfterSentToPrinter)))
                .Returns((string certificateReference) => Task.FromResult(
                    new Certificate
                    {
                        Id = Guid.NewGuid(),
                        CertificateReference = certificateReference,
                        Status = CertificateStatus.Deleted,
                        UpdatedAt = DateTime.UtcNow.AddDays(-1)
                    }));

            _certificateRepository.Setup(r => r.GetCertificate(It.IsIn(_certificateReference3)))
                .Returns((string certificateReference) => Task.FromResult<Certificate>(null));

            _certificateRepository.Setup(r => r.UpdateSentToPrinter(It.IsAny<Certificate>(), It.IsAny<int>(), It.IsAny<DateTime>()))
                .Returns(Task.CompletedTask);

            _mediator.Setup(r => r.Send(It.Is<GetForBatchNumberBatchLogRequest>(p => p.BatchNumber == _batchNumber), It.IsAny<CancellationToken>()))
                .Returns((GetForBatchNumberBatchLogRequest request, CancellationToken token) => Task.FromResult(new BatchLogResponse { BatchNumber = request.BatchNumber }));

            _mediator.Setup(r => r.Send(It.Is<GetForBatchNumberBatchLogRequest>(p => p.BatchNumber != _batchNumber), It.IsAny<CancellationToken>()))
                .Returns((GetForBatchNumberBatchLogRequest request, CancellationToken token) => Task.FromResult<BatchLogResponse>(null));

            _logger = new Mock<ILogger<UpdateCertificatesPrintStatusHandler>>();

            _sut = new UpdateCertificatesPrintStatusHandler(_certificateRepository.Object, _mediator.Object, _logger.Object);
        }
    }
}
