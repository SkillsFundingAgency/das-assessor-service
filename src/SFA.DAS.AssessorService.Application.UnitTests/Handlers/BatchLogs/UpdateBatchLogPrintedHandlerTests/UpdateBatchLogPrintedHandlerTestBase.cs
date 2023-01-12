using Moq;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Application.UnitTests.Handlers.BatchLogs.UpdateBatchLogPrintedHandlerTests
{
    public class UpdateBatchLogPrintedHandlerTestBase
    {
        protected Mock<IBatchLogRepository> _batchLogRepository;

        protected static int _validBatchNumber = 111;
        protected static int _invalidBatchNumber = 222;

        protected BatchLog _validBatchLog = new BatchLog { Id = Guid.NewGuid(), BatchNumber = _validBatchNumber };

        public void BaseArrange()
        {
            _batchLogRepository = new Mock<IBatchLogRepository>();
            _batchLogRepository.Setup(p => p.UpdateBatchLogPrinted(
                It.Is<BatchLog>(b =>
                    b.BatchNumber == _validBatchNumber &&
                    b.BatchData.BatchNumber == _validBatchNumber)))
                .ReturnsAsync(new ValidationResponse());

            _batchLogRepository.Setup(p => p.UpdateBatchLogPrinted(
                It.Is<BatchLog>(b =>
                    b.BatchNumber == _invalidBatchNumber &&
                    b.BatchData.BatchNumber == _invalidBatchNumber)))
                .ReturnsAsync(new ValidationResponse() { Errors = new List<ValidationErrorDetail>() { new ValidationErrorDetail() } });
        }
    }
}
