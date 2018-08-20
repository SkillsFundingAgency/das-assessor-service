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

        Task<List<CertificateLog>> GetCertificateLogsForBatch(int batchNumber);
    }
}