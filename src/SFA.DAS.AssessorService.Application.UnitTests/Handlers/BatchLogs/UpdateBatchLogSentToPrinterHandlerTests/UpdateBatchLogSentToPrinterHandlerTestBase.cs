using Moq;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData.Printing;
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
            
            _batchLogRepository.Setup(p => p.UpdateBatchLogSentToPrinter(_validBatchNumber,
                It.IsAny<DateTime>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<BatchData>()
                )).ReturnsAsync(new ValidationResponse());

            _batchLogRepository.Setup(p => p.UpdateBatchLogSentToPrinter(_invalidBatchNumber,
                It.IsAny<DateTime>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                It.IsAny<DateTime>(),
                It.IsAny<BatchData>()
                )).ReturnsAsync(new ValidationResponse() { Errors = new List<ValidationErrorDetail>() { new ValidationErrorDetail() } });
        }
    }
}
