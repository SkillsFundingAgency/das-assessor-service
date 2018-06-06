using System;
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

            //if ((batchLog.ScheduledDate.Month != DateTime.Now.Month) && (batchLog.BatchCreated.Month != DateTime.Now.Month))
            //        batchLog.BatchNumber = 0;

            return batchLog;
        }
    }
}