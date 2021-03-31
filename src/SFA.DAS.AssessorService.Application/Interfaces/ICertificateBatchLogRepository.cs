using SFA.DAS.AssessorService.Domain.DTOs;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface  ICertificateBatchLogRepository
    {
        Task<CertificateBatchLog> GetCertificateBatchLog(string certificateReference, int batchNumber);
        Task<List<CertificatePrintSummary>> GetCertificatesForBatch(int batchNumber);
        Task UpsertCertificatesReadyToPrintInBatch(Guid[] certificateIds, int batchNumber);
    }
}
