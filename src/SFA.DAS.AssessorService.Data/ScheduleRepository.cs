﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Data
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly IDbConnection _connection;

        public ScheduleRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<ScheduleRun> GetScheduleRun(Guid scheduleRunId)
        {
            return await _connection.QuerySingleAsync<ScheduleRun>("SELECT * FROM ScheduleRuns WHERE Id = @scheduleRunId", new { scheduleRunId });
        }

        public async Task<IEnumerable<ScheduleRun>> GetAllScheduleRun(int scheduleType)
        {
            return await _connection.QueryAsync<ScheduleRun>(@"SELECT *
                                    FROM ScheduleRuns
                                    WHERE ScheduleType = @scheduleType
                                    AND IsComplete = 0
                                    ORDER BY RunTime", new { scheduleType });
        }

        public async Task<ScheduleRun> GetNextScheduleToRunNow(int scheduleType)
        {
            return await _connection.QueryFirstOrDefaultAsync<ScheduleRun>(@"SELECT TOP(1) * 
                                    FROM ScheduleRuns 
                                    WHERE ScheduleType = @scheduleType 
                                    AND IsComplete = 0 
                                    AND RunTime <= GetUtcDate()
                                    ORDER BY RunTime", new {scheduleType});
        }

        public async Task<ScheduleRun> GetNextScheduledRun(int scheduleType)
        {
            return await _connection.QueryFirstOrDefaultAsync<ScheduleRun>(@"SELECT TOP(1) * 
                                    FROM ScheduleRuns 
                                    WHERE ScheduleType = @scheduleType 
                                    AND IsComplete = 0 
                                    ORDER BY RunTime", new {scheduleType});
        }

        public async Task CompleteScheduleRun(Guid scheduleRunId)
        {
            var schedule = await _connection.QuerySingleAsync<ScheduleRun>("SELECT * FROM ScheduleRuns WHERE Id = @scheduleRunId", new {scheduleRunId});
            if (!schedule.IsComplete)
            {
                await _connection.ExecuteAsync("UPDATE ScheduleRuns SET IsComplete = 1 WHERE Id = @scheduleRunId", new {scheduleRunId});
                if (schedule.IsRecurring)
                {
                    var newScheduleTime = schedule.RunTime.AddMinutes((int)schedule.Interval);
                    await _connection.ExecuteAsync(
                        "INSERT ScheduleRuns (RunTime, Interval, IsRecurring, ScheduleType) VALUES (@runTime, @interval, @isRecurring, @scheduleType)",
                        new {runTime = newScheduleTime, schedule.Interval, IsRecurring = true, schedule.ScheduleType});
                }   
            }
        }

        public async Task QueueImmediateRun(int scheduleType) 
        {
            await _connection.ExecuteAsync(
                "INSERT ScheduleRuns (RunTime, ScheduleType) VALUES (@runTime, @scheduleType)",
                new {runTime = DateTime.UtcNow, scheduleType});
        }

        public async Task CreateScheduleRun(ScheduleRun scheduleRun)
        {
            await _connection.ExecuteAsync(
                @"INSERT ScheduleRuns (RunTime, Interval, IsRecurring, ScheduleType) 
                               VALUES (@runTime, @interval, @isRecurring, @scheduleType)", scheduleRun);
        }

        public async Task DeleteScheduleRun(Guid scheduleRunId)
        {
            await _connection.ExecuteAsync("DELETE ScheduleRuns WHERE Id = @scheduleRunId", new {scheduleRunId});
        }
    }
}