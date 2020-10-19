using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Api.Types.Models.ScheduleRun
{
    public class SendScheduleRunStatusRequest
    {
        public Guid ScheduleRunId { get; set; }
        public ScheduleRunStatus ScheduleRunStatus { get; set; }
    }
}
