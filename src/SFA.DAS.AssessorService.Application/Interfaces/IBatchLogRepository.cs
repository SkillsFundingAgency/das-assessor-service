using System;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IBatchLogRepository
    {
        Task<BatchLog> Create(BatchLog batchLog);
        Task<BatchLogResponse> GetBatchLogFromPeriodAndBatchNumber(string requestPeriod, string requestBatchNumber);
        Task<ValidationResponse> UpdateBatchLogBatchWithDataRequest(Guid requestId, string requestBatchData);
    }
}