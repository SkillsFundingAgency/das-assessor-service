using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.ExternalApis.SubmissionEvents.Types;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IIlrRepository
    {
        Task<IEnumerable<Ilr>> SearchForLearnerByUln(long uln);
        Task<Ilr> Get(long uln, int standardCode);
        Task StoreSearchLog(SearchLog log);
        Task<IEnumerable<Ilr>> Search(string searchQuery);
        Task<IEnumerable<Ilr>> SearchForLearnerByUlnAndFamilyName(long uln, string familyName);
        Task RefreshFromSubmissionEventData(Guid id, SubmissionEvent subEvent);
        Task MarkAllUpToDate(long uln);
    }
}