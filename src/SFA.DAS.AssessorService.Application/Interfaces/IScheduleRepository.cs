using System;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IScheduleRepository
    {
        Task<ScheduleRun> GetNextScheduleToRunNow(int scheduleType);
        Task<ScheduleRun> GetNextScheduledRun(int scheduleType);
        Task CompleteScheduleRun(Guid scheduleRunId);
        Task QueueImmediateRun(int scheduleType);
        Task SetScheduleRun(ScheduleRun scheduleRun);
        Task DeleteScheduleRun(Guid scheduleRunId);
    }
}