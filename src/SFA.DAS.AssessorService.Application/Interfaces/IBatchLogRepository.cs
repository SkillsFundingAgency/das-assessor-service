using System;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IBatchLogRepository
    {
        Task<BatchLog> Create(BatchLog batchLog);
        Task<ValidationResponse> UpdateBatchLogSentToPrinter(BatchLog updatedBatchLog);
        Task<ValidationResponse> UpdateBatchLogPrinted(BatchLog updatedBatchLog);
        Task UpsertCertificatesReadyToPrintInBatch(int batchNumber, Guid[] certificateIds);
    }
}