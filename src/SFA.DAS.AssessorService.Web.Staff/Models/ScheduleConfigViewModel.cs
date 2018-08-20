using SFA.DAS.AssessorService.Web.Staff.Domain;
using System;

namespace SFA.DAS.AssessorService.Web.Staff.Models
{
    public class ScheduleConfigViewModel
    {
        public Guid Id { get; set; }
        public DateTime RunTime { get; set; }
        public string Interval { get; set; }
        public ScheduleInterval? ScheduleInterval { get; set; }
        public bool IsRecurring { get; set; }
        public ScheduleJobType ScheduleType { get; set; }
    }
}
