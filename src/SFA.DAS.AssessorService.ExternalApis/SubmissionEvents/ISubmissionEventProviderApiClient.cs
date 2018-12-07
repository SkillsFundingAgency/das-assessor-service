using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.ExternalApis.SubmissionEvents.Types;

namespace SFA.DAS.AssessorService.ExternalApis.SubmissionEvents
{
    public interface ISubmissionEventProviderApiClient
    {
        Task<List<SubmissionEvent>> GetLatestLearnerEventForStandards(long uln, long sinceEventId = 0);
        
    }
}
