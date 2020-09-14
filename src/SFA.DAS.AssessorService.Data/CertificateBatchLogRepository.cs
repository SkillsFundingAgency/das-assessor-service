using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Data
{
    public class CertificateBatchLogRepository : ICertificateBatchLogRepository
    {
        private readonly AssessorDbContext _context;
        private readonly IDbConnection _connection;

        public CertificateBatchLogRepository(AssessorDbContext context,
            IDbConnection connection)
        {
            _context = context;
            _connection = connection;
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
