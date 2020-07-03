using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.DTOs.Staff;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IStaffCertificateRepository
    {
        Task<List<CertificateForSearch>> GetCertificatesFor(long[] ulns);
        
        Task<List<CertificateLogSummary>> GetAllCertificateLogs(Guid certificateId);
        Task<List<CertificateLogSummary>> GetSummaryCertificateLogs(Guid certificateId);
        Task<CertificateLogSummary> GetLatestCertificateLog(Guid certificateId);

        Task<StaffReposBatchSearchResult> GetCertificateLogsForBatch(int batchNumber, int page, int pageSize);
        Task<StaffReposBatchLogResult> GetBatchLogs(int page, int pageSize);
    }


    public class StaffReposBatchSearchResult
    {
        public DateTime SentToPrinterAt { get; set; }
        public DateTime? PrintedAt { get; set; }

        public IEnumerable<CertificateBatchLogSummary> PageOfResults { get; set; }

        public int TotalCount { get; set; }
    }

    public class StaffReposBatchLogResult
    {
        public IEnumerable<BatchLogSummary> PageOfResults { get; set; }

        public int TotalCount { get; set; }
    }
}