using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Handlers.Certificates;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.Certificates.UpdateCertificatesPrintStatusHandlerTests
{
    public class UpdateCertificatesPrintStatusHandlerTestsBase
    {
        public class CertificatePrintStatusUpdateHandlerTestsFixture
        {
            protected Mock<ICertificateRepository> _certificateRepository;
            protected Mock<IBatchLogQueryRepository> _batchLogQueryRepository;
            protected Mock<IMediator> _mediator;
            protected Mock<ILogger<CertificatePrintStatusUpdateHandler>> _logger;

            protected CertificatePrintStatusUpdateHandler _sut;

            public CertificatePrintStatusUpdateHandlerTestsFixture()
            {
                _certificateRepository = new Mock<ICertificateRepository>();
                _batchLogQueryRepository = new Mock<IBatchLogQueryRepository>();
                _mediator = new Mock<IMediator>();
                _logger = new Mock<ILogger<CertificatePrintStatusUpdateHandler>>();

                _sut = new CertificatePrintStatusUpdateHandler(_certificateRepository.Object, _batchLogQueryRepository.Object, _mediator.Object, _logger.Object);
            }

            public CertificatePrintStatusUpdateHandlerTestsFixture WithCertificate(string reference, string status, DateTime statusAt, int batchNumber,  DateTime toBePrinted)
            {
                _certificateRepository.Setup(r => r.GetCertificate(reference))
                    .Returns(Task.FromResult(
                        new Certificate
                        {
                            Id = Guid.NewGuid(),
                            CertificateReference = reference,
                            Status = status,
                            UpdatedAt = statusAt,
                            BatchNumber = batchNumber,
                            ToBePrinted = toBePrinted
                        }));

                return this;
            }

            public CertificatePrintStatusUpdateHandlerTestsFixture WithCertificateBatchLog(int batchNumber, string certificateReference, string status, DateTime statusAt, string reasonForChange, DateTime updatedAt )
            {
                _batchLogQueryRepository.Setup(r => r.GetCertificateBatchLog(batchNumber, certificateReference))
                    .Returns(Task.FromResult(
                        new CertificateBatchLog
                        {
                            Id = Guid.NewGuid(),
                            BatchNumber = batchNumber,
                            CertificateReference = certificateReference,
                            Status = status,
                            StatusAt = statusAt,
                            ReasonForChange = reasonForChange,
                            UpdatedAt = updatedAt
                        }));

                return this;
            }

            public CertificatePrintStatusUpdateHandlerTestsFixture WithBatchLog(int batchNumber)
            {
                _mediator.Setup(r => r.Send(It.Is<GetBatchLogRequest>(p => p.BatchNumber == batchNumber), It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult(
                        new BatchLogResponse 
                        { 
                            BatchNumber = batchNumber 
                        }));

                return this;
            }

            public async Task<ValidationResponse> Handle(CertificatePrintStatusUpdateRequest request)
            {
                return await _sut.Handle(request, new CancellationToken());
            }

            public void VerifyUpdatePrintStatusCalled(string certificateReference, int batchNumber, 
                string status, DateTime statusAt, string reasonForChange, 
                bool updateCertificate, bool updateCertificateLog)
            {
                _certificateRepository.Verify(r => r.UpdatePrintStatus(
                    It.Is<Certificate>(c => c.CertificateReference == certificateReference), 
                    batchNumber, 
                    status, statusAt, reasonForChange, updateCertificate, updateCertificateLog),
                    Times.Once);
            }

            public void VerifyUpdatePrintStatusNotCalled(string certificateReference, int batchNumber)
            {
                _certificateRepository.Verify(r => r.UpdatePrintStatus(
                    It.Is<Certificate>(c => c.CertificateReference == certificateReference),
                    batchNumber,
                    It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()),
                    Times.Never);
            }
        }
    }
}
