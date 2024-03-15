using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface ILearnerRepository
    {
        Task<IEnumerable<Learner>> SearchForLearnerByUln(long uln);
        
        Task<Learner> Get(long uln, int stdCode);
        Task<ApprenticeLearner> Get(long apprenticeshipId);

        Task StoreSearchLog(SearchLog log);
        Task<int> GetEpaoPipelinesCount(string epaOrgId, int? stdCode, int pipelineCutoff);
    }
}