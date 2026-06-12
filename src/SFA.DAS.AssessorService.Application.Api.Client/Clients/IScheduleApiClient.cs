using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Paging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IScheduleApiClient
    {
        Task<object> CreateScheduleRun(ScheduleRun schedule);
        Task<object> DeleteScheduleRun(Guid scheduleRunId);
        Task<IList<ScheduleRun>> GetAllScheduledRun(int scheduleType);
        Task<ScheduleRun> GetNextScheduledRun(int scheduleType);
        Task<ScheduleRun> GetScheduleRun(Guid scheduleRunId);
        Task RestartSchedule(Guid id);
        Task<object> RunNowScheduledRun(int scheduleType);
    }
}