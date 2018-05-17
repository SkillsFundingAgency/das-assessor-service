using System.Threading.Tasks;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data
{
    public class BatchLogRepository : IBatchLogRepository
    {
        private readonly AssessorDbContext _assessorDbContext;

        public BatchLogRepository(AssessorDbContext assessorDbContext)
        {
            _assessorDbContext = assessorDbContext;
        }

        public async Task<BatchLog> Create(BatchLog batchLog)
        {
            await _assessorDbContext.BatchLogs.AddAsync(batchLog);
            _assessorDbContext.SaveChanges();

            return batchLog;
        }
    }
}