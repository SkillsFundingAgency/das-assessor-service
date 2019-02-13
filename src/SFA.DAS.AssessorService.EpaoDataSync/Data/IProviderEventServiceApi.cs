using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.EpaoDataSync.Data.Types;

namespace SFA.DAS.AssessorService.EpaoDataSync.Data
{
    public interface IProviderEventServiceApi
    {
        Task<List<SubmissionEvent>> GetLatestLearnerEventForStandards(long uln, long sinceEventId = 0);
        Task<SubmissionEvents> GetSubmissionsEventsByTime(string sinceTime, long pageNumber);
        Task<SubmissionEvents> GetSubmissionsEventsByEventId(long sinceEventId, long pageNumber);
    }
}
