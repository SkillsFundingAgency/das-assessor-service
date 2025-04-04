using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.DTOs;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.Interfaces
{
    public interface IBatchLogQueryRepository
    {
        Task<BatchLog> Get(int batchNumber);

        Task<CertificateBatchLog> GetCertificateBatchLog(int batchNumber, string certificateReference);

        Task<BatchLog> GetLastBatchLog();

        Task<int?> GetBatchNumberReadyToPrint();

        Task<List<CertificatePrintSummaryBase>> GetCertificatesForBatch(int batchNumber);
    }
}