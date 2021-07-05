using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Extensions;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.StartupConfiguration;
using SFA.DAS.AssessorService.Web.ViewModels.Apply;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Controllers.Apply
{
    [Authorize]
    public class StandardController : AssessorController
    {
        private readonly IOrganisationsApiClient _orgApiClient;
        private readonly IQnaApiClient _qnaApiClient;
        private readonly IStandardVersionClient _standardVersionApiClient;
        private readonly IApplicationService _applicationService;
        private readonly IWebConfiguration _config;

        public StandardController(IApplicationApiClient apiClient, IOrganisationsApiClient orgApiClient, IQnaApiClient qnaApiClient, IContactsApiClient contactsApiClient, 
            IStandardVersionClient standardVersionApiClient, IApplicationService applicationService, IHttpContextAccessor httpContextAccessor, IWebConfiguration config)
            : base(apiClient, contactsApiClient, httpContextAccessor)
        {
            _orgApiClient = orgApiClient;
            _qnaApiClient = qnaApiClient;
            _standardVersionApiClient = standardVersionApiClient;
            _applicationService = applicationService;
            _config = config;
        }

        [HttpGet("Standard")]
        public IActionResult Index()
        {
            var standardViewModel = new StandardVersionViewModel();
            return View("~/Views/Application/Standard/FindStandard.cshtml", standardViewModel);
        }

        [HttpPost("Standard")]
        public async Task<IActionResult> Search(StandardVersionViewModel model)
        {
            if (string.IsNullOrEmpty(model.StandardToFind) || model.StandardToFind.Length <= 2)
            {
                ModelState.AddModelError(nameof(model.StandardToFind), "Enter a valid search string (more than 2 characters)");
                TempData["ShowErrors"] = true;
                return RedirectToAction(nameof(Index));
            }

            var standards = await _standardVersionApiClient.GetLatestStandardVersions();
            model.Results = standards
                .Where(s => s.Title.Contains(model.StandardToFind, StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            return View("~/Views/Application/Standard/FindStandardResults.cshtml", model);
        }

        [HttpGet("standard/confirm-standard/{standardReference}")]
        [HttpGet("standard/confirm-standard/{standardReference}/{version}")]
        public async Task<IActionResult> ConfirmStandard(string standardReference, string version)
        {
            var contact = await GetUserContact();
            var org = await _orgApiClient.GetOrganisationByUserId(contact.Id);

            var standardVersions = (await _orgApiClient.GetAppliedStandardVersionsForEPAO(org?.EndPointAssessorOrganisationId, standardReference))
                                        .OrderBy(s => s.Version);
            var latestStandard = standardVersions.LastOrDefault();
            bool anyExistingVersions = standardVersions.Any(x => x.ApprovedStatus == ApprovedStatus.Approved);

            if (!string.IsNullOrWhiteSpace(version))
            {
                // specific version selected (from standversion view)
                var standardViewModel = new StandardVersionViewModel { StandardReference = standardReference };
                standardViewModel.SelectedStandard = (StandardVersion)standardVersions.FirstOrDefault(x => x.Version.ToString() == version);
                standardViewModel.Results = new List<StandardVersion>() { standardViewModel.SelectedStandard };
                standardViewModel.ApplicationStatus = await ApplicationStandardStatus(org, standardReference, new List<string>() { version });
                return View("~/Views/Application/Standard/ConfirmStandard.cshtml", standardViewModel);
            }
            else if (anyExistingVersions)
            {
                // existing approved versions for this standard
                var model = new StandardVersionApplicationViewModel { StandardReference = standardReference };
                model.SelectedStandard = new StandardVersionApplication(latestStandard);
                model.Results = ApplyVersionStatuses(standardVersions).OrderByDescending(x => x.Version).ToList();
                return View("~/Views/Application/Standard/StandardVersion.cshtml", model);
            }
            else
            {
                // no existing approved versions for this standard
                var standardViewModel = new StandardVersionViewModel { StandardReference = standardReference };
                standardViewModel.Results = standardVersions.Select(s => (StandardVersion)s).ToList();
                standardViewModel.SelectedStandard = (StandardVersion)latestStandard;
                if (standardVersions.Count() == 1)
                    standardViewModel.ApplicationStatus = await ApplicationStandardStatus(org, standardReference, new List<string>() { standardVersions.First().Version.VersionToString() });
                return View("~/Views/Application/Standard/ConfirmStandard.cshtml", standardViewModel);
            }
        }

        [HttpPost("standard/confirm-standard/{standardReference}")]
        [HttpPost("standard/confirm-standard/{standardReference}/{version}")]
        public async Task<IActionResult> ConfirmStandard(StandardVersionViewModel model, string standardReference, string version)
        {
            var contact = await GetUserContact();
            var org = await _orgApiClient.GetOrganisationByUserId(contact.Id);

            var standardVersions = (await _orgApiClient.GetAppliedStandardVersionsForEPAO(org?.EndPointAssessorOrganisationId, standardReference))
                                        .OrderBy(s => s.Version);

            bool anyExistingVersions = standardVersions.Any(x => x.ApprovedStatus == ApprovedStatus.Approved);

            AppliedStandardVersion selectedStandard = null;
            string applicationStatus = null;
            List<string> versions = null;

            if(string.IsNullOrWhiteSpace(version))
            {
                selectedStandard = standardVersions.LastOrDefault();
                versions = model.SelectedVersions;
                if (model.SelectedVersions != null)
                    applicationStatus = await ApplicationStandardStatus(org, standardReference, model.SelectedVersions);
            }
            else
            {
                selectedStandard = standardVersions.FirstOrDefault(x => x.Version.ToString() == version);
                versions = new List<string> { selectedStandard.Version.VersionToString() };
            }

            // check that the confirm checkbox has been selected
            if (!model.IsConfirmed)
            {
                ModelState.AddModelError(nameof(model.IsConfirmed), "Confirm you have read the assessment plan");
                TempData["ShowConfirmedError"] = true;
            }

            // check that a version has been selected
            if (string.IsNullOrWhiteSpace(version) &&
                standardVersions.Count() > 1 &&
                (model.SelectedVersions == null || !model.SelectedVersions.Any()))
            {
                ModelState.AddModelError(nameof(model.SelectedVersions), "You must select at least one version");
                TempData["ShowVersionError"] = true;
            }

            if (!ModelState.IsValid || !string.IsNullOrWhiteSpace(applicationStatus))
            {
                model.Results = string.IsNullOrWhiteSpace(version) ? standardVersions.Select(s => (StandardVersion)s).ToList() :
                                    new List<StandardVersion>() { selectedStandard };
                model.SelectedStandard = (StandardVersion)selectedStandard;
                model.ApplicationStatus = applicationStatus;
                return View("~/Views/Application/Standard/ConfirmStandard.cshtml", model);
            }
            else
            {
                ApplicationResponse application;
                if (anyExistingVersions)
                    application = await CreateStandardApplication(contact, org, selectedStandard.LarsCode, selectedStandard.IFateReferenceNumber, selectedStandard.Title, versions, StandardApplicationTypes.Version);
                else
                    application = await CreateStandardApplication(contact, org, selectedStandard.LarsCode, selectedStandard.IFateReferenceNumber, selectedStandard.Title, versions, StandardApplicationTypes.Full);

                return RedirectToAction("SequenceSignPost", "Application", new { application.Id });
            }
        }

        [HttpGet("standard/opt-in/{standardReference}/{version}")]
        public async Task<IActionResult> OptIn(string standardReference, decimal version)
        {
            var standards = await _standardVersionApiClient.GetStandardVersionsByIFateReferenceNumber(standardReference);
            var stdVersion = standards.First(x => x.Version.Equals(version.ToString(), StringComparison.InvariantCultureIgnoreCase));

            var model = new StandardOptInViewModel()
            {
                StandardReference = stdVersion.IFateReferenceNumber,
                StandardTitle = stdVersion.Title,
                Version = stdVersion.Version,
                EffectiveFrom = stdVersion.VersionEarliestStartDate ?? DateTime.Today,
                EffectiveTo = stdVersion.VersionLatestEndDate
            };

            return View("~/Views/Application/Standard/OptIn.cshtml", model);
        }

        [HttpPost("standard/opt-in/{standardReference}/{version}")]
        public async Task<IActionResult> OptInPost(string standardReference, decimal version)
        {
            var contact = await GetUserContact();
            var org = await _orgApiClient.GetOrganisationByUserId(contact.Id);

            var standards = await _standardVersionApiClient.GetStandardVersionsByIFateReferenceNumber(standardReference);
            var stdVersion = standards.First(x => x.Version.Equals(version.VersionToString(), StringComparison.InvariantCultureIgnoreCase));
            var application = await CreateStandardApplication(contact, org, stdVersion.LarsCode, stdVersion.IFateReferenceNumber, stdVersion.Title, new List<string> { version.VersionToString() }, StandardApplicationTypes.OptIn);
            await _orgApiClient.OrganisationStandardVersionOptIn(application.Id, contact.Id, org.EndPointAssessorOrganisationId, standardReference, version, stdVersion.StandardUId, $"Opted in by EPAO by {contact.Username}");

            return RedirectToAction("OptInConfirmation", "Application", new { Id = application.Id });
        }

        private async Task<ApplicationResponse> CreateStandardApplication(ContactResponse contact, OrganisationResponse org, int larsCode, string iFateReferenceNumber, string title, List<string> versions, string standardApplicationType)
        {
            var createApplicationRequest = await _applicationService.BuildInitialRequest(contact, org, _config.ReferenceFormat);
            createApplicationRequest.StandardApplicationType = standardApplicationType;
            var id = await _applicationApiClient.CreateApplication(createApplicationRequest);
            var application = await _applicationApiClient.GetApplication(id);

            await _applicationApiClient.UpdateStandardData(application.Id, larsCode, iFateReferenceNumber, title, versions, standardApplicationType);
            if (standardApplicationType == StandardApplicationTypes.Version)
            {
                // update QnA application data for the Application Type
                var applicationData = await _qnaApiClient.GetApplicationData(application.ApplicationId);
                applicationData.ApplicationType = StandardApplicationTypes.Version;
                await _qnaApiClient.UpdateApplicationData(application.ApplicationId, applicationData);
            }

            return application;
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

        private async Task<string> ApplicationStandardStatus(OrganisationResponse org, string iFateReferenceNumber, List<string> versions)
        {
            var validApplicationStatuses = new string[] { ApplicationStatus.InProgress };
            var validApplicationSequenceStatuses = new string[] { ApplicationSequenceStatus.Draft };

            var standards = await _orgApiClient.GetAppliedStandardVersionsForEPAO(org?.EndPointAssessorOrganisationId, iFateReferenceNumber);
            var matchingStandards = standards.Where(x => versions.Contains(x.Version.VersionToString()));

            if (matchingStandards.Any(x => x.ApprovedStatus == ApprovedStatus.Approved))
                return ApplicationStatus.Approved;
            else
            {
                var inProgressApplications = matchingStandards.Where(x => x.ApprovedStatus == ApprovedStatus.ApplyInProgress &&
                                                validApplicationStatuses.Contains(x.ApplicationStatus));
                foreach(var app in inProgressApplications)
                {
                    var sequence = app.ApplyData.Sequences?.FirstOrDefault(seq => seq.IsActive && seq.SequenceNo == ApplyConst.STANDARD_SEQUENCE_NO);
                    if (sequence != null && validApplicationSequenceStatuses.Contains(sequence.Status))
                    {
                        return sequence.Status;
                    }
                }
            }

            return string.Empty;
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
            else if (version.ApprovedStatus == ApprovedStatus.Withdrawn)
                versionStatus = VersionStatus.Withdrawn;
            else if (version.ApprovedStatus == ApprovedStatus.FeedbackAdded)
                versionStatus = VersionStatus.FeedbackAdded;
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