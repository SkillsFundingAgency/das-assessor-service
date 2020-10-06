using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data
{
    public class CertificateBatchLogRepository : ICertificateBatchLogRepository
    {
        private readonly AssessorDbContext _context;        

        public CertificateBatchLogRepository(AssessorDbContext context)
        {
            _context = context;            
        }

        public async Task<CertificateBatchLog> GetCertificateBatchLog(string certificateReference, int batchNumber)
        {
            var certificateBatchLog = await
                _context.CertificateBatchLogs
                .FirstOrDefaultAsync(q => q.CertificateReference == certificateReference && q.BatchNumber == batchNumber);

            return certificateBatchLog;
        }
    }
}
