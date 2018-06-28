using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IIlrRepository
    {
        Task<IEnumerable<Ilr>> SearchForLearner(SearchRequest searchRequest);
        Task<Ilr> Get(long uln, int standardCode);
        Task StoreSearchLog(SearchLog log);
    }
}