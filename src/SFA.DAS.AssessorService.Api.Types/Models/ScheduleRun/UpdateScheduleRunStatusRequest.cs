using System;

namespace SFA.DAS.AssessorService.Api.Types.Models.ScheduleRun
{
    public class UpdateScheduleRunStatusRequest
    {
        public Guid ScheduleRunId { get; set; }
        public ScheduleRunStatus ScheduleRunStatus { get; set; }
    }
}
