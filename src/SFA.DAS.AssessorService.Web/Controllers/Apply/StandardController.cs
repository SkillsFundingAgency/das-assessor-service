using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public StandardController(IApplicationApiClient apiClient, IOrganisationsApiClient orgApiClient, IQnaApiClient qnaApiClient, IContactsApiClient contactsApiClient,
            IStandardVersionClient standardVersionApiClient, IHttpContextAccessor httpContextAccessor)
        {
            _apiClient = apiClient;
            _orgApiClient = orgApiClient;
            _qnaApiClient = qnaApiClient;
            _contactsApiClient = contactsApiClient;
            _standardVersionApiClient = standardVersionApiClient;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("Standard/{id}")]
        public IActionResult Index(Guid id)
        {
            var standardViewModel = new StandardVersionViewModel { Id = id };
            return View("~/Views/Application/Standard/FindStandard.cshtml", standardViewModel);
        }

        [HttpPost("Standard/{id}")]
        public async Task<IActionResult> Search(StandardVersionViewModel model)
        {
            if (string.IsNullOrEmpty(model.StandardToFind) || model.StandardToFind.Length <= 2)
            {
                ModelState.AddModelError(nameof(model.StandardToFind), "Enter a valid search string (more than 2 characters)");
                TempData["ShowErrors"] = true;
                return RedirectToAction(nameof(Index), new { model.Id });
            }

            var standards = await _standardVersionApiClient.GetLatestStandardVersions();
            model.Results = standards
                .Where(s => s.Title.Contains(model.StandardToFind, StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            return View("~/Views/Application/Standard/FindStandardResults.cshtml", model);
        }

        [HttpGet("standard/{id}/confirm-standard/{standardReference}")]
        [HttpGet("standard/{id}/confirm-standard/{standardReference}/{version}")]
        [ApplicationAuthorize(routeId: "Id")]
        public async Task<IActionResult> ConfirmStandard(Guid id, string standardReference, string version)
        {
            var application = await _apiClient.GetApplication(id);
            if (!CanUpdateApplicationAsync(application))
            {
                return RedirectToAction("Applications", "Application");
            }

            var org = await _orgApiClient.GetEpaOrganisation(application.OrganisationId.ToString());
            var standardVersions = (await _orgApiClient.GetAppliedStandardVersionsForEPAO(org?.OrganisationId, standardReference))
                                        .OrderBy(s => s.Version);
            var latestStandard = standardVersions.LastOrDefault();
            bool anyExistingVersions = standardVersions.Any(x => x.ApprovedStatus == ApprovedStatus.Approved);

            if (!anyExistingVersions)
            {
                // no existing approved versions for this standard
                var standardViewModel = new StandardVersionViewModel { Id = id, StandardReference = standardReference };
                standardViewModel.Results = standardVersions.Select(s => (StandardVersion)s).ToList();
                standardViewModel.SelectedStandard = (StandardVersion)latestStandard;
                standardViewModel.ApplicationStatus = await ApplicationStandardStatus(application, standardViewModel.SelectedStandard.LarsCode);
                return View("~/Views/Application/Standard/ConfirmStandard.cshtml", standardViewModel);
            }
            else if (!string.IsNullOrWhiteSpace(version))
            {
                // specific version selected (from standversion view)
                var standardViewModel = new StandardVersionViewModel { Id = id, StandardReference = standardReference };
                standardViewModel.SelectedStandard = (StandardVersion)standardVersions.FirstOrDefault(x => x.Version.ToString() == version);
                standardViewModel.Results = new List<StandardVersion>() { standardViewModel.SelectedStandard };
                return View("~/Views/Application/Standard/ConfirmStandard.cshtml", standardViewModel);
            }
            else
            {
                // existing approved versions for this standard
                var model = new StandardVersionApplicationViewModel { Id = id, StandardReference = standardReference };
                model.SelectedStandard = new StandardVersionApplication(latestStandard);
                model.Results = ApplyVersionStatuses(standardVersions).OrderByDescending(x => x.Version).ToList();
                return View("~/Views/Application/Standard/StandardVersion.cshtml", model);
            }
        }

        [HttpPost("standard/{id}/confirm-standard/{standardReference}")]
        [HttpPost("standard/{id}/confirm-standard/{standardReference}/{version}")]
        public async Task<IActionResult> ConfirmStandard(StandardVersionViewModel model, Guid id, string standardReference, string version)
        {
            var application = await _apiClient.GetApplication(id);
            if (!CanUpdateApplicationAsync(application))
            {
                return RedirectToAction("Applications", "Application");
            }

            var org = await _orgApiClient.GetEpaOrganisation(application.OrganisationId.ToString());
            var standardVersions = (await _orgApiClient.GetAppliedStandardVersionsForEPAO(org?.OrganisationId, standardReference))
                                        .OrderBy(s => s.Version);

            model.SelectedStandard = string.IsNullOrWhiteSpace(version) ? (StandardVersion)standardVersions.LastOrDefault() :(StandardVersion)standardVersions.FirstOrDefault(x => x.Version.ToString() == version);
            model.Results = standardVersions.Select(s => (StandardVersion)s).ToList(); 
            model.ApplicationStatus = await ApplicationStandardStatus(application, model.SelectedStandard.LarsCode);

            if (!model.IsConfirmed)
            {
                ModelState.AddModelError(nameof(model.IsConfirmed), "Please tick to confirm");
                TempData["ShowConfirmedError"] = true;
                return View("~/Views/Application/Standard/ConfirmStandard.cshtml", model);
            }

            if (string.IsNullOrWhiteSpace(version))
            {
                // if there is only one version then it is automatically selected 
                if (standardVersions.Count() > 1 && (model.SelectedVersions == null || !model.SelectedVersions.Any()))
                {
                    ModelState.AddModelError(nameof(model.SelectedVersions), "You must select at least one version");
                    TempData["ShowVersionError"] = true;
                    return View("~/Views/Application/Standard/ConfirmStandard.cshtml", model);
                }

                if (!string.IsNullOrEmpty(model.ApplicationStatus))
                {
                    return View("~/Views/Application/Standard/ConfirmStandard.cshtml", model);
                }
            }

            var versions = (standardVersions.Count() > 1 && string.IsNullOrWhiteSpace(version)) ? model.SelectedVersions : new List<string>() { model.SelectedStandard.Version };
            bool anyExistingVersions = standardVersions.Any(x => x.ApprovedStatus == ApprovedStatus.Approved);

            if (anyExistingVersions)
            {
                await _apiClient.UpdateStandardData(id, model.SelectedStandard.LarsCode, model.SelectedStandard.IFateReferenceNumber, model.SelectedStandard.Title, versions, ApplicationTypes.Version);

                // update QnA application data for the Application Type
                var applicationData = await _qnaApiClient.GetApplicationData(application.ApplicationId);
                applicationData.ApplicationType = ApplicationTypes.Version;
                await _qnaApiClient.UpdateApplicationData(application.ApplicationId, applicationData);
            }
            else
                await _apiClient.UpdateStandardData(id, model.SelectedStandard.LarsCode, model.SelectedStandard.IFateReferenceNumber, model.SelectedStandard.Title, versions, application.ApplicationType);

            return RedirectToAction("SequenceSignPost", "Application", new { Id = id });
        }

        [HttpGet("standard/{id}/opt-in/{standardReference}/{version}")]
        [ApplicationAuthorize(routeId: "Id")]
        public async Task<IActionResult> OptIn(Guid id, string standardReference, string version)
        {
            var application = await _apiClient.GetApplication(id);

            if (!CanUpdateApplicationAsync(application))
            {
                return RedirectToAction("Applications", "Application");
            }

            return View("~/Views/Application/Standard/OptIn.cshtml");
        }

        private bool CanUpdateApplicationAsync(ApplicationResponse application)
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
            if (standard == null && application?.ApplyData != null)
            {
                var userId = await GetUserId();
                var applications = await _apiClient.GetStandardApplications(userId);
                foreach (var app in applications)
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
            var signinId = _httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == "sub")?.Value;
            var contact = await _contactsApiClient.GetContactBySignInId(signinId);

            return contact?.Id ?? Guid.Empty;
        }

        private IEnumerable<StandardVersionApplication> ApplyVersionStatuses(IEnumerable<AppliedStandardVersion> versions)
        {
            bool approved = false;
            bool changed = false;
            var results = new List<StandardVersionApplication>();

            foreach (var version in versions.OrderBy(s => s.Version))
            {
                var stdVersion = new StandardVersionApplication(version);

                if (version.ApprovedStatus == ApprovedStatus.Approved)
                {
                    approved = true;
                    changed = false;
                    stdVersion.VersionStatus = VersionStatus.Approved;
                }
                else
                {
                    stdVersion.VersionStatus = MapUnapprovedVersionStatus(version, approved, changed);

                    if (approved && version.EPAChanged)
                        changed = true;
                }

                results.Add(stdVersion);
            }
        
            return results;
        }

        private string MapUnapprovedVersionStatus(AppliedStandardVersion version, bool approved, bool previouslyChanged)
        {
            string versionStatus = null;

            if (version.ApprovedStatus == ApprovedStatus.ApplyInProgress)
                versionStatus =  VersionStatus.InProgress;
            else if (version.ApprovedStatus == ApprovedStatus.NotYetApplied && approved)
            {
                if (version.EPAChanged || previouslyChanged)
                    versionStatus = VersionStatus.NewVersionChanged;
                else
                    versionStatus = VersionStatus.NewVersionNoChange;
            }

            return versionStatus;
        }
    }
}