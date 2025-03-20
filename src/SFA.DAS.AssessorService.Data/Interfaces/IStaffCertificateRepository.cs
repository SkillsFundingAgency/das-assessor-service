using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.DTOs.Staff;

namespace SFA.DAS.AssessorService.Data.Interfaces
{
    public interface IStaffCertificateRepository
    {
        Task<List<CertificateForSearch>> GetCertificatesFor(long[] ulns);
        
        Task<List<CertificateLogSummary>> GetAllCertificateLogs(Guid certificateId);
        Task<List<CertificateLogSummary>> GetSummaryCertificateLogs(Guid certificateId);
        Task<CertificateLogSummary> GetLatestCertificateLog(Guid certificateId);

        Task<GetCertificateLogsForBatchResult> GetCertificateLogsForBatch(int batchNumber, int page, int pageSize);
        Task<GetBatchLogsResult> GetBatchLogs(int page, int pageSize);
        Task<List<CertificateLogSummary>> GetLatestCertificateLogs(Guid certificateId, int count);
    }
}