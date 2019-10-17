using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Web.ViewModels.Apply;

namespace SFA.DAS.AssessorService.Web.Controllers.Apply
{
    [Authorize]
    public class StandardController : Controller
    {
        private readonly IApplicationApiClient _apiClient;

        public StandardController(IApplicationApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet("Standard/{id}")]
        public IActionResult Index(Guid id)
        {
            var standardViewModel = new StandardViewModel { Id = id };
            return View("~/Views/Application/Standard/FindStandard.cshtml", standardViewModel);
        }

        [HttpPost("Standard/{id}")]
        public async Task<IActionResult> Search(StandardViewModel model)
        {
            if (string.IsNullOrEmpty(model.StandardToFind) || model.StandardToFind.Length <= 2)
            {
                ModelState.AddModelError(nameof(model.StandardToFind), "Enter a valid search string (more than 2 characters)");
                TempData["ShowErrors"] = true;
                return RedirectToAction(nameof(Index), new { model.Id });
            }

            var results = await _apiClient.GetStandards();

            model.Results = results.Where(r => r.Title.ToLower().Contains(model.StandardToFind.ToLower())).ToList();

            return View("~/Views/Application/Standard/FindStandardResults.cshtml", model);
        }

        [HttpGet("standard/{id}/confirm-standard/{standardCode}")]
        public async Task<IActionResult> StandardConfirm(Guid id, int standardCode)
        {
            var application = await _apiClient.GetApplication(id);
            var standardViewModel = new StandardViewModel { Id = id, StandardCode = standardCode};
            var results = await _apiClient.GetStandards();
            standardViewModel.SelectedStandard = results.FirstOrDefault(r => r.StandardId == standardCode);
            standardViewModel.ApplicationStatus = application.ApplyData.Apply.StandardCode == standardCode ? application.ApplicationStatus : string.Empty;
            return View("~/Views/Application/Standard/ConfirmStandard.cshtml", standardViewModel);
        }

        [HttpPost("standard/{id}/confirm-standard/{standardCode}")]
        public async Task<IActionResult> StandardConfirm(StandardViewModel model, Guid id, int standardCode)
        {
            var application = await _apiClient.GetApplication(id);
            var results = await _apiClient.GetStandards();
            model.SelectedStandard = results.FirstOrDefault(r => r.StandardId == standardCode);

            model.ApplicationStatus = application.ApplyData.Apply.StandardCode == standardCode ? application.ApplicationStatus : string.Empty;

            if (!model.IsConfirmed)
            {
                ModelState.AddModelError(nameof(model.IsConfirmed), "Please tick to confirm");
                TempData["ShowErrors"] = true;
                return View("~/Views/Application/Standard/ConfirmStandard.cshtml", model);
            }

            if (!string.IsNullOrEmpty(model.ApplicationStatus))
            {
                return View("~/Views/Application/Standard/ConfirmStandard.cshtml", model);
            }

            await _apiClient.UpdateStandardData(id, standardCode, model.SelectedStandard?.ReferenceNumber, model.SelectedStandard.Title);

            return RedirectToAction("Applications", "Application");
        }

    }
}