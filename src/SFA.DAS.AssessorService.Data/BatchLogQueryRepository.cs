using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data
{
    public class BatchLogQueryRepository : IBatchLogQueryRepository
    {
        private readonly AssessorDbContext _assessorDbContext;

        public BatchLogQueryRepository(AssessorDbContext assessorDbContext)
        {
            _assessorDbContext = assessorDbContext;
        }

        public async Task<BatchLog> Get(int batchNumber)
        {
            var batchLog = await _assessorDbContext.BatchLogs
                .FirstOrDefaultAsync(b => b.BatchNumber == batchNumber);

            return batchLog;
        }

        public async Task<List<Certificate>> GetCertificates(int batchNumber)
        {
            var certificaes =  await (from certificateBatchLog in _assessorDbContext.CertificateBatchLogs
                          join certificate in _assessorDbContext.Certificates on
                               certificateBatchLog.CertificateReference equals certificate.CertificateReference
                          where certificateBatchLog.BatchNumber == batchNumber
                          select certificate).ToListAsync();

            return certificaes;
        }

        public async Task<BatchLog> GetLastBatchLog()
        {
            var batchLog = await _assessorDbContext.BatchLogs
                .OrderByDescending(q => q.BatchCreated)
                .FirstOrDefaultAsync();

            if (batchLog == null)
            {
                batchLog = new BatchLog
                {
                    BatchNumber = 0
                };
                return batchLog;
            }

            return batchLog;
        }
    }
}