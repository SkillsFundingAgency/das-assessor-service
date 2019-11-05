using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Web.ViewModels.Apply;

namespace SFA.DAS.AssessorService.Web.Controllers.Apply
{
    [Authorize]
    public class StandardController : Controller
    {
        private readonly IApplicationApiClient _apiClient;
        private readonly IOrganisationsApiClient _orgApiClient;
        private readonly IQnaApiClient _qnaApiClient;
        private readonly IContactsApiClient _contactsApiClient;

        public StandardController(IApplicationApiClient apiClient, IOrganisationsApiClient orgApiClient, IQnaApiClient qnaApiClient, IContactsApiClient contactsApiClient)
        {
            _apiClient = apiClient;
            _orgApiClient = orgApiClient;
            _qnaApiClient = qnaApiClient;
            _contactsApiClient = contactsApiClient;
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

            var standards = await _apiClient.GetStandards();

            model.Results = standards.Where(s => s.Title.Contains(model.StandardToFind, StringComparison.InvariantCultureIgnoreCase)).ToList();

            return View("~/Views/Application/Standard/FindStandardResults.cshtml", model);
        }

        [HttpGet("standard/{id}/confirm-standard/{standardCode}")]
        public async Task<IActionResult> ConfirmStandard(Guid id, int standardCode)
        {
            var application = await _apiClient.GetApplication(id);
            var standardViewModel = new StandardViewModel { Id = id, StandardCode = standardCode };
            if (!CanUpdateApplicationAsync(application))
            {
                return RedirectToAction("Applications", "Application");
            }

            var standards = await _apiClient.GetStandards();
            standardViewModel.SelectedStandard = standards.FirstOrDefault(s => s.StandardId == standardCode);
            standardViewModel.ApplicationStatus = await ApplicationStandardStatus(application, standardCode);
            return View("~/Views/Application/Standard/ConfirmStandard.cshtml", standardViewModel);
        }

        [HttpPost("standard/{id}/confirm-standard/{standardCode}")]
        public async Task<IActionResult> ConfirmStandard(StandardViewModel model, Guid id, int standardCode)
        {
            var application = await _apiClient.GetApplication(id);
            if (!CanUpdateApplicationAsync(application))
            {
                return RedirectToAction("Applications", "Application");
            }

            var standards = await _apiClient.GetStandards();
            model.SelectedStandard = standards.FirstOrDefault(s => s.StandardId == standardCode);
            model.ApplicationStatus = await ApplicationStandardStatus(application, standardCode);

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

        private  bool CanUpdateApplicationAsync(ApplicationResponse application)
        {
            const int STANDARD_SEQUENCE_NO = 2;

            bool canUpdate = false;

            var validApplicationStatuses = new string[] { ApplicationStatus.InProgress };
            var validApplicationSequenceStatuses = new string[] { ApplicationSequenceStatus.Draft };

            if (application?.ApplyData != null && validApplicationStatuses.Contains(application.ApplicationStatus))
            {
                var sequence = application.ApplyData.Sequences?.FirstOrDefault(seq => seq.IsActive && seq.SequenceNo == STANDARD_SEQUENCE_NO);

                if (sequence != null && validApplicationSequenceStatuses.Contains(sequence.Status))
                {
                    canUpdate = true;
                }
            }

            return canUpdate;
        }

        private async Task<string> ApplicationStandardStatus(ApplicationResponse application, int standardCode)
        {
            const int STANDARD_SEQUENCE_NO = 2;

            var validApplicationStatuses = new string[] { ApplicationStatus.InProgress };
            var validApplicationSequenceStatuses = new string[] { ApplicationSequenceStatus.Draft };

            var applicationData = await _qnaApiClient.GetApplicationData(application.ApplicationId);
            var org = await _orgApiClient.GetEpaOrganisationById(applicationData.OrganisationReferenceId);
            var standards = await _orgApiClient.GetOrganisationStandardsByOrganisation(org?.OrganisationId);
            var standard = standards?.SingleOrDefault(x => x.StandardCode == standardCode);

            if(standard == null && application?.ApplyData != null)
            {
                var userId = await GetUserId();
                var applications = await _apiClient.GetApplications(userId, false);
                foreach( var app in applications)
                {
                    if (app.OrganisationId == org?.Id && app.ApplyData.Apply.StandardCode == standardCode)
                    {
                        if (validApplicationStatuses.Contains(application.ApplicationStatus))
                        {
                            var sequence = application.ApplyData.Sequences?.FirstOrDefault(seq => seq.IsActive && seq.SequenceNo == STANDARD_SEQUENCE_NO);

                            if (sequence != null && validApplicationSequenceStatuses.Contains(sequence.Status))
                            {
                                return sequence.Status;
                            }
                        }
                    }
                }
            }
            else if ((standard.EffectiveTo == null || standard.EffectiveTo > DateTime.UtcNow) && org.Status == OrganisationStatus.Live)
                return ApplicationStatus.Approved;

            return string.Empty;
        }

        private async Task<Guid> GetUserId()
        {
            var signinId = User.Claims.First(c => c.Type == "sub")?.Value;
            var contact =  await _contactsApiClient.GetContactBySignInId(signinId);

            return contact?.Id ?? Guid.Empty;
        }
    }
}