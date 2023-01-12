using Moq;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.BatchLogs.UpdateBatchLogSentToPrinterHandlerTests
{
    public class UpdateBatchLogSentToPrinterHandlerTestBase
    {
        protected Mock<IBatchLogRepository> _batchLogRepository;

        protected static int _validBatchNumber = 111;
        protected static int _invalidBatchNumber = 222;

        protected BatchLog _validBatchLog = new BatchLog { Id = Guid.NewGuid(), BatchNumber = _validBatchNumber };

        public void BaseArrange()
        {
            _batchLogRepository = new Mock<IBatchLogRepository>();

            _batchLogRepository.Setup(p => p.UpdateBatchLogSentToPrinter(
                It.Is<BatchLog>(b => b.BatchNumber == _validBatchNumber)))
                .ReturnsAsync(new ValidationResponse());

            _batchLogRepository.Setup(p => p.UpdateBatchLogSentToPrinter(
                It.Is<BatchLog>(b => b.BatchNumber == _invalidBatchNumber)))
                .ReturnsAsync(new ValidationResponse() { Errors = new List<ValidationErrorDetail>() { new ValidationErrorDetail() } });
        }
    }
}
