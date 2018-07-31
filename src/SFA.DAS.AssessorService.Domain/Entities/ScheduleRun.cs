using System;

namespace SFA.DAS.AssessorService.Domain.Entities
{
    public class ScheduleRun
    {
        public Guid Id { get; set; }
        public DateTime RunTime { get; set; }
        public bool IsComplete { get; set; }
        public long? Interval { get; set; }
        public bool IsRecurring { get; set; }
        public ScheduleType ScheduleType { get; set; }
    }

    public enum ScheduleType
    {
        PrintRun = 1
    }
}