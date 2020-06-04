using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IBatchLogQueryRepository
    {
        Task<BatchLog> GetForBatchNumber(int batchNumber);

        Task<List<Certificate>> GetCertificates(int batchNumber);

        Task<BatchLog> GetLastBatchLog();
    }
}