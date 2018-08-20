using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Extensions;
using SFA.DAS.AssessorService.Web.Staff.Domain;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{
    [Authorize]
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
            List<ScheduleConfigViewModel> viewModels = new List<ScheduleConfigViewModel>();

            foreach(var schedule in await _apiClient.GetAllScheduledRun((int)ScheduleJobType.PrintRun))
            {
                ScheduleConfigViewModel viewModel = new ScheduleConfigViewModel
                {
                    Id = schedule.Id,
                    RunTime = schedule.RunTime.UtcToTimeZoneTime(),
                    Interval = schedule.Interval.HasValue ? TimeSpan.FromMinutes(schedule.Interval.Value).Humanize().Titleize() : "-",
                    IsRecurring = schedule.IsRecurring,
                    ScheduleType = (ScheduleJobType)schedule.ScheduleType,
                };

                if (Enum.TryParse<ScheduleInterval>(schedule.Interval.ToString(), out var scheduleInterval) && Enum.IsDefined(typeof(ScheduleInterval), scheduleInterval))
                {
                    viewModel.ScheduleInterval = scheduleInterval;
                }

                viewModels.Add(viewModel);
            }

            return View(viewModels);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid scheduleRunId)
        {
            var schedule = await _apiClient.GetScheduleRun(scheduleRunId);

            ScheduleConfigViewModel viewModel = new ScheduleConfigViewModel
            {
                Id = schedule.Id,
                RunTime = schedule.RunTime.UtcToTimeZoneTime(),
                Interval = schedule.Interval.HasValue ? TimeSpan.FromMinutes(schedule.Interval.Value).Humanize().Titleize() : "-",
                IsRecurring = schedule.IsRecurring,
                ScheduleType = (ScheduleJobType)schedule.ScheduleType,
            };

            if(Enum.TryParse<ScheduleInterval>(schedule.Interval.ToString(), out var scheduleInterval) && Enum.IsDefined(typeof(ScheduleInterval), scheduleInterval))
            {
                viewModel.ScheduleInterval = scheduleInterval;
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ScheduleConfigViewModel viewModel)
        {
            if (viewModel != null)
            {
                await _apiClient.DeleteScheduleRun(viewModel.Id);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = new ScheduleConfigViewModel
            {
                ScheduleType = ScheduleJobType.PrintRun,
                RunTime = DateTime.UtcNow.UtcToTimeZoneTime(),
                IsRecurring = false,   
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ScheduleConfigViewModel viewModel)
        {
            if (viewModel != null)
            {
                ScheduleRun schedule = new ScheduleRun
                {
                    ScheduleType = (ScheduleType)viewModel.ScheduleType,
                    RunTime = viewModel.RunTime.UtcFromTimeZoneTime(),
                    IsRecurring = viewModel.IsRecurring,
                    Interval = (int?)viewModel.ScheduleInterval
                };

                await _apiClient.CreateScheduleRun(schedule);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> RunNow(int scheduleType)
        {
            var viewModel = new ScheduleConfigViewModel
            {
                ScheduleType = (ScheduleJobType)scheduleType,
                RunTime = DateTime.UtcNow.UtcToTimeZoneTime(),
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> RunNow(ScheduleConfigViewModel viewModel)
        {
            if (viewModel != null)
            {
                await _apiClient.RunNowScheduledRun((int)viewModel.ScheduleType);
            }

            return RedirectToAction("Index");
        }
    }
}