using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface  ICertificateBatchLogRepository
    {
        Task<CertificateBatchLog> GetCertificateBatchLog(string certificateReference, int batchNumber);
        Task UpdateCertificatesReadyToPrintInBatch(Guid[] certificateIds, int batchNumber);
    }
}
