using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IIlrRepository
    {
        Task<IEnumerable<Ilr>> SearchForLearnerByUln(long uln);
        Task<int> GetEpaoPipelinesCount(string epaOrgId, int? stdCode = null);

        Task<Ilr> Get(long uln, int stdCode);

        Task StoreSearchLog(SearchLog log);
        
        Task Create(Ilr ilr);

        Task Update(Ilr ilr);
    }
}