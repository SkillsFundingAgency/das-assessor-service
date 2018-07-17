using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Application.Interfaces;
using SFA.DAS.AssessorService.Domain.Entities;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly IScheduleRepository _scheduleRepository;

        public ScheduleController(IScheduleRepository scheduleRepository)
        {
            _scheduleRepository = scheduleRepository;
        }

        [HttpGet("api/v1/schedule", Name="GetNextScheduleRun")]
        public async Task<IActionResult> GetNextScheduleRun(int scheduleType)
        {
            var scheduleRun = await _scheduleRepository.GetNextScheduleRun(scheduleType);
            return Ok(scheduleRun);
        }

        [HttpPost("api/v1/schedule",Name="CompleteScheduleRun")]
        public async Task<IActionResult> CompleteScheduleRun(Guid scheduleRunId)
        {
            await _scheduleRepository.CompleteScheduleRun(scheduleRunId);
            return Ok();
        }

        [HttpPost("api/v1/schedule/runnow", Name="RunNow")]
        public async Task<IActionResult> RunNow(int scheduleType)
        {
            await _scheduleRepository.QueueImmediateRun(scheduleType);
            return Ok();
        }

        [HttpPut("api/v1/schedule/set", Name="Set")]
        public async Task<IActionResult> Set([FromBody]ScheduleRun scheduleRun)
        {
            await _scheduleRepository.SetScheduleRun(scheduleRun);
            return Ok();
        }
    }
}