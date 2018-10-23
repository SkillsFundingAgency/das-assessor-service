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
        Task<List<CertificateLogSummary>> GetCertificateLogsFor(Guid certificateId,
            bool allRecords);

        Task<StaffReposBatchSearchResult> GetCertificateLogsForBatch(int batchNumber, int page, int pageSize);
        Task<StaffReposBatchLogResult> GetBatchLogs(int page, int pageSize);
        Task<IEnumerable<Ilr>> SearchForLearnerByCertificateReference(string certRef);
        
        Task<IEnumerable<Ilr>> SearchForLearnerByUln(long uln, bool isPrivatelyFunded);
        Task<bool> IsPrivateCertificateForUln(long uln);
        Task<bool> IsPrivateCertificateForCertificateReference(string certificateReference);
        Task<Ilr> GetPrivateCertificate(string certificatReference);
    }


    public class StaffReposBatchSearchResult
    {
        public IEnumerable<CertificateLog> PageOfResults { get; set; }

        public int TotalCount { get; set; }
    }

    public class StaffReposBatchLogResult
    {
        public IEnumerable<BatchLog> PageOfResults { get; set; }

        public int TotalCount { get; set; }
    }
}