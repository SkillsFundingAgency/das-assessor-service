using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.AssessorService.Api.Types.Models.ScheduleRun;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;
using LastRunStatus = SFA.DAS.AssessorService.Domain.Entities.LastRunStatus;

namespace SFA.DAS.AssessorService.Data
{
    public class ScheduleRepository : Repository, IScheduleRepository
    {
        public ScheduleRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public async Task<ScheduleRun> GetScheduleRun(Guid scheduleRunId)
        {
            return await _unitOfWork.Connection.QuerySingleAsync<ScheduleRun>("SELECT * FROM ScheduleRuns WHERE Id = @scheduleRunId", new { scheduleRunId });
        }

        public async Task<IEnumerable<ScheduleRun>> GetAllScheduleRun(int scheduleType)
        {
            return await _unitOfWork.Connection.QueryAsync<ScheduleRun>(@"SELECT *
                                    FROM ScheduleRuns
                                    WHERE ScheduleType = @scheduleType
                                    AND IsComplete = 0
                                    ORDER BY RunTime", new { scheduleType });
        }

        public async Task<ScheduleRun> GetNextScheduleToRunNow(int scheduleType)
        {
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<ScheduleRun>(@"SELECT TOP(1) * 
                                    FROM ScheduleRuns 
                                    WHERE ScheduleType = @scheduleType 
                                    AND IsComplete = 0 
                                    AND RunTime <= GetUtcDate()
                                    AND (LastRunStatus <> @scheduleRunStatus or LastRunStatus is null)
                                    ORDER BY RunTime", new {scheduleType, scheduleRunStatus = LastRunStatus.Failed});
        }

        public async Task<ScheduleRun> GetNextScheduledRun(int scheduleType)
        {
            return await _unitOfWork.Connection.QueryFirstOrDefaultAsync<ScheduleRun>(@"SELECT TOP(1) * 
                                    FROM ScheduleRuns 
                                    WHERE ScheduleType = @scheduleType 
                                    AND IsComplete = 0 
                                    AND (LastRunStatus <> @scheduleRunStatus or LastRunStatus is null)
                                    ORDER BY RunTime", new {scheduleType, scheduleRunStatus = LastRunStatus.Failed });
        }

        public async Task CompleteScheduleRun(Guid scheduleRunId)
        {
            var schedule = await _unitOfWork.Connection.QuerySingleAsync<ScheduleRun>("SELECT * FROM ScheduleRuns WHERE Id = @scheduleRunId", new {scheduleRunId});
            if (!schedule.IsComplete)
            {
                await _unitOfWork.Connection.ExecuteAsync("UPDATE ScheduleRuns SET IsComplete = 1 WHERE Id = @scheduleRunId", 
                    new {scheduleRunId });
                if (schedule.IsRecurring)
                {
                    var newScheduleTime = schedule.RunTime.AddMinutes((int)schedule.Interval);
                    await _unitOfWork.Connection.ExecuteAsync(
                        "INSERT ScheduleRuns (RunTime, Interval, IsRecurring, ScheduleType, LastRunStatus) VALUES (@runTime, @interval, @isRecurring, @scheduleType, @lastRunStatus)",
                        new {runTime = newScheduleTime, schedule.Interval, IsRecurring = true, schedule.ScheduleType, lastRunStatus = LastRunStatus.Completed});
                }   
            }
        }

        public async Task UpdateLastRunStatus(UpdateLastRunStatusRequest request)
        {
            await _unitOfWork.Connection.ExecuteAsync("UPDATE ScheduleRuns SET LastRunStatus=@scheduleRunStatus WHERE Id = @scheduleRunId",
                new { scheduleRunId = request.ScheduleRunId, scheduleRunStatus = request.LastRunStatus });
        }

        public async Task QueueImmediateRun(int scheduleType) 
        {
            await _unitOfWork.Connection.ExecuteAsync(
                "INSERT ScheduleRuns (RunTime, ScheduleType) VALUES (@runTime, @scheduleType)",
                new {runTime = DateTime.UtcNow, scheduleType});
        }

        public async Task CreateScheduleRun(ScheduleRun scheduleRun)
        {
            await _unitOfWork.Connection.ExecuteAsync(
                @"INSERT ScheduleRuns (RunTime, Interval, IsRecurring, ScheduleType) 
                               VALUES (@runTime, @interval, @isRecurring, @scheduleType)", scheduleRun);
        }

        public async Task DeleteScheduleRun(Guid scheduleRunId)
        {
            await _unitOfWork.Connection.ExecuteAsync("DELETE ScheduleRuns WHERE Id = @scheduleRunId", new {scheduleRunId});
        }
    }
}