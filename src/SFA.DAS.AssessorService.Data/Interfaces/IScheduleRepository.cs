﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.ScheduleRun;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data.Interfaces
{
    public interface IScheduleRepository
    {
        Task<ScheduleRun> GetScheduleRun(Guid scheduleRunId);
        Task<IEnumerable<ScheduleRun>> GetAllScheduleRun(int scheduleType);
        Task<ScheduleRun> GetNextScheduleToRunNow(int scheduleType);
        Task<ScheduleRun> GetNextScheduledRun(int scheduleType);
        Task CompleteScheduleRun(Guid scheduleRunId);
        Task QueueImmediateRun(int scheduleType);
        Task CreateScheduleRun(ScheduleRun scheduleRun);
        Task DeleteScheduleRun(Guid scheduleRunId);
        Task UpdateLastRunStatus(UpdateLastRunStatusRequest request);
    }
}