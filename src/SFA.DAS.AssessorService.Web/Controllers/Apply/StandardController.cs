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
using SFA.DAS.AssessorService.Web.StartupConfiguration;
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
        private readonly IStandardVersionClient _standardVersionApiClient;

        public StandardController(IApplicationApiClient apiClient, IOrganisationsApiClient orgApiClient, IQnaApiClient qnaApiClient, IContactsApiClient contactsApiClient, IStandardVersionClient standardVersionApiClient)
        {
            _apiClient = apiClient;
            _orgApiClient = orgApiClient;
            _qnaApiClient = qnaApiClient;
            _contactsApiClient = contactsApiClient;
            _standardVersionApiClient = standardVersionApiClient;
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
          
            model.OrganisationHasStandardWithVersions = await GenerateOrganisationHasStandardWithVersionsMapping(model.Id, model.Results);

            return View("~/Views/Application/Standard/FindStandardResults.cshtml", model);
        }

        [HttpGet("standard/{id}/confirm-standard/{standardCode}")]
        [ApplicationAuthorize(routeId: "Id")]
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

            return RedirectToAction("SequenceSignPost","Application", new { Id = id });
        }

        private  bool CanUpdateApplicationAsync(ApplicationResponse application)
        {
            bool canUpdate = false;

            var validApplicationStatuses = new string[] { ApplicationStatus.InProgress };
            var validApplicationSequenceStatuses = new string[] { ApplicationSequenceStatus.Draft };

            if (application?.ApplyData != null && validApplicationStatuses.Contains(application.ApplicationStatus))
            {
                var sequence = application.ApplyData.Sequences?.FirstOrDefault(seq => seq.IsActive && seq.SequenceNo == ApplyConst.STANDARD_SEQUENCE_NO);

                if (sequence != null && validApplicationSequenceStatuses.Contains(sequence.Status))
                {
                    canUpdate = true;
                }
            }

            return canUpdate;
        }

        private async Task<string> ApplicationStandardStatus(ApplicationResponse application, int standardCode)
        {
            var validApplicationStatuses = new string[] { ApplicationStatus.InProgress };
            var validApplicationSequenceStatuses = new string[] { ApplicationSequenceStatus.Draft };

            var applicationData = await _qnaApiClient.GetApplicationData(application.ApplicationId);
            var org = await _orgApiClient.GetEpaOrganisationById(applicationData.OrganisationReferenceId);
            var standards = await _orgApiClient.GetOrganisationStandardsByOrganisation(org?.OrganisationId);
            var standard = standards?.SingleOrDefault(x => x.StandardCode == standardCode);

            // does the org or the application not have the standard && 
            if(standard == null && application?.ApplyData != null)
            {
                var userId = await GetUserId();
                var applications = await _apiClient.GetStandardApplications(userId);
                foreach( var app in applications)
                {
                    if (app.OrganisationId == org?.Id && app.ApplyData.Apply.StandardCode == standardCode)
                    {
                        if (validApplicationStatuses.Contains(application.ApplicationStatus))
                        {
                            var sequence = application.ApplyData.Sequences?.FirstOrDefault(seq => seq.IsActive && seq.SequenceNo == ApplyConst.STANDARD_SEQUENCE_NO);

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


        // SV-643
        // Standards results page needs to use a different hyperlink depending whether or not the 
        // organisation is already approved for a standard that has version(s). So for every element in the search
        // results we'll set a flag to indicate whether or not the org is already approved for a version of that standard.
        private async Task<Dictionary<string, bool>> GenerateOrganisationHasStandardWithVersionsMapping(Guid id, List<Api.Types.Models.Standards.StandardCollation> searchResults)
        {
            var application = await _apiClient.GetApplication(id);
            var org = await _orgApiClient.GetEpaOrganisation(application.OrganisationId.ToString());
            var orgStandards = await _orgApiClient.GetOrganisationStandardsByOrganisation(org?.OrganisationId);

            var organisationHasStandard = new Dictionary<string, bool>();

            var allStandardVersions = await _standardVersionApiClient.GetAllStandardVersions();   // might be slow / a memory hog?

            foreach (var result in searchResults)
            {
                var hasStandard = orgStandards.Any(s => s.StandardCode == result.StandardId);
                var hasStandardWithVersions = false;
                if(hasStandard)
                {
                    // OK so we have the standard, but does it have any versions?
                    var standardVersions = allStandardVersions.Where(s => s.IFateReferenceNumber == result.ReferenceNumber);
                    hasStandardWithVersions = (null != allStandardVersions && allStandardVersions.Any());
                }
                organisationHasStandard.Add(result.ReferenceNumber, hasStandardWithVersions);
            }

            return organisationHasStandard;
        }
    }
}