using SFA.DAS.AssessorService.Domain.DTOs;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IBatchLogQueryRepository
    {
        Task<BatchLog> Get(int batchNumber);

        Task<CertificateBatchLog> GetCertificateBatchLog(int batchNumber, string certificateReference);

        Task<BatchLog> GetLastBatchLog();

        Task<int?> GetBatchNumberReadyToPrint();

        Task<List<CertificatePrintSummary>> GetCertificatesForBatch(int batchNumber);
    }
}