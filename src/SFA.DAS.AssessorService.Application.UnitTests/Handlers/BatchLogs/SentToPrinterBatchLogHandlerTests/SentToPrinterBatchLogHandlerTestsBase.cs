using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Handlers.BatchLogs;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.BatchLogs.SentToPrinterBatchLogHandlerTests
{
    public class SentToPrinterBatchLogHandlerTestsBase
    {
        /*protected SentToPrinterBatchLogHandler _sut;

        protected Mock<ICertificateRepository> _certificateRepository;
        protected Mock<IBatchLogQueryRepository> _batchLogQueryRepository;
        protected Mock<ILogger<SentToPrinterBatchLogHandler>> _logger;

        protected static int _batchNumber = 222;
        protected static DateTime _printedAt = DateTime.UtcNow;

        protected static string _certificateReference1 = "00000001";
        protected static string _certificateReference2 = "00000002";
        protected static string _certificateReference3 = "00000003";
        
        protected BatchLog _batchLog = new BatchLog { Id = Guid.NewGuid(), BatchNumber = _batchNumber };
        protected ValidationResponse _updateCertificatesPrintStatusRequestResponse = new ValidationResponse();
        protected ValidationResponse _response;

        protected void BaseArrange()
        {
            _certificateRepository = new Mock<ICertificateRepository>();
            _batchLogQueryRepository = new Mock<IBatchLogQueryRepository>();

            _certificateRepository.Setup(r => r.GetCertificate(It.IsIn(_certificateReference1, _certificateReference2)))
                .Returns((string certificateReference) => Task.FromResult(
                    new Certificate
                    {
                        Id = Guid.NewGuid(),
                        CertificateReference = certificateReference
                    }));

            _certificateRepository.Setup(r => r.GetCertificate(It.IsNotIn(_certificateReference1, _certificateReference2)))
                .Returns((string certificateReference) => Task.FromResult<Certificate>(null));

            _certificateRepository.Setup(r => r.UpdateSentToPrinter(It.IsAny<Certificate>(), It.IsAny<int>(), It.IsAny<DateTime>()))
                .Returns(Task.CompletedTask);

            _batchLogQueryRepository.Setup(r => r.GetForBatchNumber(It.IsIn(_batchNumber)))
                .Returns(Task.FromResult(_batchLog));

            _batchLogQueryRepository.Setup(r => r.GetForBatchNumber(It.IsNotIn(_batchNumber)))
                .Returns(Task.FromResult<BatchLog>(null));

            _logger = new Mock<ILogger<SentToPrinterBatchLogHandler>>();

            _sut = new SentToPrinterBatchLogHandler(_certificateRepository.Object, _batchLogQueryRepository.Object, _logger.Object);
        }*/
    }
}
