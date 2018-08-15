using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{
    public class ScheduleConfigController : Controller
    {
        private readonly ApiClient _apiClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public ScheduleConfigController(ApiClient apiClient, IHttpContextAccessor contextAccessor)
        {
            _apiClient = apiClient;
            _contextAccessor = contextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var schedules = await _apiClient.GetAllScheduledRun(1);

            return View(schedules);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid scheduleRunId)
        {
            var schedule = await _apiClient.GetScheduleRun(scheduleRunId);

            return View(schedule);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ScheduleRun schedule)
        {
            if (schedule != null)
            {
                await _apiClient.DeleteScheduleRun(schedule.Id);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var schedule = new ScheduleRun
            {
                ScheduleType = ScheduleType.PrintRun
            };

            return View(schedule);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ScheduleRun schedule)
        {
            if (schedule != null)
            {
                await _apiClient.CreateScheduleRun(schedule);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> RunNow(int scheduleType)
        {
            ScheduleRun runNow = new ScheduleRun
            {
                ScheduleType = (ScheduleType)scheduleType,
                RunTime = DateTime.UtcNow
            };

            return View(runNow);
        }

        [HttpPost]
        public async Task<IActionResult> RunNow(ScheduleRun schedule)
        {
            if (schedule != null)
            {
                await _apiClient.RunNowScheduledRun((int)schedule.ScheduleType);
            }

            return RedirectToAction("Index");
        }
    }
}