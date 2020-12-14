using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IBatchLogQueryRepository
    {
        Task<BatchLog> Get(int batchNumber);

        Task<BatchLog> GetLastBatchLog();

        Task<int?> GetBatchNumberReadyToPrint();
    }
}