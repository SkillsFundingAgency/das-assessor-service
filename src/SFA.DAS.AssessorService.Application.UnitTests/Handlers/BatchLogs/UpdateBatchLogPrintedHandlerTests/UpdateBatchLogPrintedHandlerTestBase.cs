using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData.Printing;
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
            _batchLogRepository.Setup(p => p.UpdateBatchLogPrinted(_validBatchNumber, It.IsAny<BatchData>())).ReturnsAsync(new ValidationResponse());
            _batchLogRepository.Setup(p => p.UpdateBatchLogPrinted(_invalidBatchNumber, It.IsAny<BatchData>())).ReturnsAsync(new ValidationResponse() { Errors = new List<ValidationErrorDetail>() { new ValidationErrorDetail() } });
        }
    }
}
