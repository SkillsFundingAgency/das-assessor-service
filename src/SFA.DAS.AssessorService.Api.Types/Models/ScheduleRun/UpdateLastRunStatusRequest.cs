using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.ScheduleRun
{
    public class UpdateLastRunStatusRequest
    {
        public Guid ScheduleRunId { get; set; }
        public LastRunStatus LastRunStatus { get; set; }
    }
}
