using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.OrganisationStandards;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Api.Client.QnA;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Exceptions;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Extensions;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.StartupConfiguration;
using SFA.DAS.AssessorService.Web.ViewModels.Apply;
using SFA.DAS.AssessorService.Web.ViewModels.Standard;
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

        #region Routes
        public const string AddStandardSearchRouteGet = nameof(AddStandardSearchRouteGet);
        public const string AddStandardSearchRoutePost = nameof(AddStandardSearchRoutePost);
        public const string AddStandardSearchResultsRouteGet = nameof(AddStandardSearchResultsRouteGet);
        public const string AddStandardChooseVersionsRouteGet = nameof(AddStandardChooseVersionsRouteGet);
        public const string AddStandardChooseVersionsRoutePost = nameof(AddStandardChooseVersionsRoutePost);
        public const string AddStandardConfirmRouteGet = nameof(AddStandardConfirmRouteGet);
        public const string AddStandardConfirmRoutePost = nameof(AddStandardConfirmRoutePost);
        public const string AddStandardConfirmationRouteGet = nameof(AddStandardConfirmationRouteGet);
        public const string StandardDetailsRouteGet = nameof(StandardDetailsRouteGet);
        public const string OptInStandardVersionRouteGet = nameof(OptInStandardVersionRouteGet);
        public const string OptInStandardVersionRoutePost = nameof(OptInStandardVersionRoutePost);
        public const string OptInStandardVersionConfirmationRouteGet = nameof(OptInStandardVersionConfirmationRouteGet);
        public const string OptOutStandardVersionRouteGet = nameof(OptOutStandardVersionRouteGet);
        public const string OptOutStandardVersionRoutePost = nameof(OptOutStandardVersionRoutePost);
        public const string OptOutStandardVersionConfirmationRouteGet = nameof(OptOutStandardVersionConfirmationRouteGet);
        #endregion

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

        [PrivilegeAuthorize(Privileges.ApplyForStandard)]
        [HttpGet("standard/add-standard/{search?}", Name = AddStandardSearchRouteGet)]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        public IActionResult AddStandardSearch(string search)
        {
            var standardViewModel = new AddStandardSearchViewModel()
            {
                StandardToFind = ModelState
                    .GetAttemptedValueWhenInvalid(nameof(AddStandardSearchViewModel.StandardToFind), search)
            };

            return View(standardViewModel);
        }

        [PrivilegeAuthorize(Privileges.ApplyForStandard)]
        [HttpPost("standard/add-standard", Name = AddStandardSearchRoutePost)]
        [ModelStatePersist(ModelStatePersist.Store)]
        public IActionResult AddStandardSearch(AddStandardSearchViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return RedirectToRoute(AddStandardSearchRouteGet, new { search = model.StandardToFind });
            }
            
            return RedirectToRoute(AddStandardSearchResultsRouteGet, new { search = model.StandardToFind });
        }

        [PrivilegeAuthorize(Privileges.ApplyForStandard)]
        [HttpGet("standard/add-standard/{search}/results", Name = AddStandardSearchResultsRouteGet)]
        public async Task<IActionResult> AddStandardSearchResults(string search)
        {
            if (string.IsNullOrEmpty(search))
                throw new ArgumentException("Value cannot be null or empty", nameof(search));

            var allStandards = await _standardVersionApiClient.GetLatestStandardVersions();
            var approvedStandards = await _standardVersionApiClient.GetEpaoRegisteredStandardVersions(GetEpaOrgIdFromClaim());

            var model = new AddStandardSearchViewModel
            {
                Approved = approvedStandards
                    .GroupBy(standardVersion => standardVersion.IFateReferenceNumber)
                    .Where(group => group.Any())
                    .Select(group => group.FirstOrDefault())
                    .ToList(),
                Results = allStandards
                    .Where(s => s.Title.Contains(search, StringComparison.InvariantCultureIgnoreCase))
                    .ToList(),
                StandardToFind = search
            };

            return View(model);
        }

        [PrivilegeAuthorize(Privileges.ApplyForStandard)]
        [HttpGet("standard/add-standard/{search}/{referenceNumber}/choose-versions", Name = AddStandardChooseVersionsRouteGet)]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        public async Task<IActionResult> AddStandardChooseVersions(string search, string referenceNumber)
        {
            if (string.IsNullOrEmpty(search))
                throw new ArgumentException("Value cannot be null or empty", nameof(search));

            if (string.IsNullOrEmpty(referenceNumber))
                throw new ArgumentException("Value cannot be null or empty", nameof(search));

            var standardVersions = await _standardVersionApiClient.GetStandardVersionsByIFateReferenceNumber(referenceNumber);
            var model = new AddStandardConfirmViewModel
            {
                Search = search,
                StandardReference = referenceNumber,
                Standard = standardVersions.FirstOrDefault(),
                StandardVersions = standardVersions.ToList(),
                IsConfirmed = ModelState
                    .GetAttemptedValueWhenInvalid(nameof(AddStandardConfirmViewModel.IsConfirmed), false),
                SelectedVersions = ModelState
                    .GetAttemptedValueListWhenInvalid(nameof(AddStandardConfirmViewModel.SelectedVersions), new List<string>(), ','),
            };

            return View(model);
        }

        [PrivilegeAuthorize(Privileges.ApplyForStandard)]
        [HttpPost("standard/add-standard/choose-versions", Name = AddStandardChooseVersionsRoutePost)]
        [ModelStatePersist(ModelStatePersist.Store)]
        public IActionResult AddStandardChooseVersions(AddStandardConfirmViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToRoute(AddStandardChooseVersionsRouteGet, new { search = model.Search, referenceNumber = model.StandardReference });
            }

            return RedirectToRoute(AddStandardConfirmRouteGet, new { search = model.Search, referenceNumber = model.StandardReference, versions = model.SelectedVersions });
        }

        [PrivilegeAuthorize(Privileges.ApplyForStandard)]
        [HttpGet("standard/add-standard/{search}/{referenceNumber}/confirm", Name = AddStandardConfirmRouteGet)]
        public async Task<IActionResult> AddStandardConfirm(string search, string referenceNumber, [FromQuery] List<string> versions)
        {
            if(string.IsNullOrEmpty(search))
                throw new ArgumentException("Value cannot be null or empty", nameof(search));

            if(string.IsNullOrEmpty(referenceNumber))
                throw new ArgumentException("Value cannot be null or empty", nameof(referenceNumber));

            if (!(versions?.Any() ?? false))
                throw new ArgumentException("Value must contain elements", nameof(versions));
            
            var standardVersions = await _standardVersionApiClient.GetStandardVersionsByIFateReferenceNumber(referenceNumber);
            var model = new AddStandardConfirmViewModel
            {
                Search = search,
                StandardReference = referenceNumber,
                Standard = standardVersions.FirstOrDefault(),
                StandardVersions = standardVersions.ToList(),
                IsConfirmed = true,
                SelectedVersions = versions
            };

            return View(model);
        }

        [PrivilegeAuthorize(Privileges.ApplyForStandard)]
        [HttpPost("standard/add-standard/confirm", Name = AddStandardConfirmRoutePost)]
        public async Task<IActionResult> AddStandardConfirm(AddStandardConfirmViewModel model)
        {
            var epaOrganisation = await _orgApiClient.GetEpaOrganisation(GetEpaOrgIdFromClaim());

            var request = new OrganisationStandardAddRequest
            {
                OrganisationId = epaOrganisation.OrganisationId,
                StandardVersions = model.SelectedVersions,
                StandardReference = model.StandardReference,
                ContactId = await GetUserId()
            };

            await _orgApiClient.AddOrganisationStandard(request);

            return RedirectToRoute(AddStandardConfirmationRouteGet, new {referenceNumber = model.StandardReference});
        }

        [PrivilegeAuthorize(Privileges.ApplyForStandard)]
        [HttpGet("standard/add-standard/{referenceNumber}/confirmation", Name = AddStandardConfirmationRouteGet)]
        public async Task<IActionResult> AddStandardConfirmation(string referenceNumber)
        {
            if (string.IsNullOrEmpty(referenceNumber))
                throw new ArgumentOutOfRangeException(nameof(referenceNumber));

            var standardVersions = await _standardVersionApiClient.GetEpaoRegisteredStandardVersions(GetEpaOrgIdFromClaim(), referenceNumber);
            var model = new AddStandardConfirmationViewModel
            {
                Standard = standardVersions.FirstOrDefault(),
                StandardVersions = standardVersions.Select(x => x.Version).ToList(),
                FeedbackUrl = _config.FeedbackUrl,
            };

            return View(model);
        }

        [HttpGet("standard/standard-details/{referenceNumber}", Name = StandardDetailsRouteGet)]
        public async Task<IActionResult> StandardDetails(string referenceNumber)
        {
            if (string.IsNullOrEmpty(referenceNumber))
                throw new ArgumentException("The value must not be null or empty", nameof(referenceNumber));

            var allVersions = await _standardVersionApiClient.GetStandardVersionsByIFateReferenceNumber(referenceNumber);
            var approvedVersions = await _standardVersionApiClient.GetEpaoRegisteredStandardVersions(GetEpaOrgIdFromClaim(), referenceNumber);

            if (!allVersions.Any())
                throw new NotFoundException($"The standard reference {referenceNumber} does not have any versions");

            var model = new StandardDetailsViewModel
            {
                SelectedStandard = allVersions.FirstOrDefault(),
                AllVersions = allVersions.ToList(),
                ApprovedVersions = approvedVersions.ToList(),
            };

            return View(model);
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

        [HttpGet("standard/view-standard/{standardReference}")]
        public async Task<IActionResult> ViewStandard(string standardReference)
        {
            var contact = await GetUserContact();
            var org = await _orgApiClient.GetOrganisationByUserId(contact.Id);

            var existingApplications = (await _applicationApiClient.GetStandardApplications(contact.Id))?
                .Where(p => p.ApplicationStatus != ApplicationStatus.Declined);

            var existingEmptyApplication = existingApplications.FirstOrDefault(x => x.StandardCode == null);
            if (existingEmptyApplication != null)
                return RedirectToAction("ConfirmStandard", new { Id = existingEmptyApplication.Id, StandardReference = standardReference });
            else
            {
                var createApplicationRequest = await _applicationService.BuildInitialRequest(contact, org, _config.ReferenceFormat);
                var id = await _applicationApiClient.CreateApplication(createApplicationRequest);
                return RedirectToAction("ConfirmStandard", new { Id = id, StandardReference = standardReference });
            }
        }

        [HttpGet("standard/{id}/confirm-standard/{standardReference}")]
        [HttpGet("standard/{id}/confirm-standard/{standardReference}/{version}")]
        [ApplicationAuthorize(routeId: "Id")]
        public async Task<IActionResult> ConfirmStandard(Guid id, string standardReference, string version)
        {
            var application = await _applicationApiClient.GetApplication(id);
            if (!CanUpdateApplicationAsync(application))
            {
                return RedirectToAction("Applications", "Application");
            }

            var org = await _orgApiClient.GetEpaOrganisation(application.OrganisationId.ToString());
            var standardVersions = (await _orgApiClient.GetAppliedStandardVersionsForEPAO(org?.OrganisationId, standardReference))
                                        .OrderBy(s => s.Version);
            var earliestStandard = standardVersions.FirstOrDefault();
            var latestStandard = standardVersions.LastOrDefault();

            if (!string.IsNullOrWhiteSpace(version))
            {
                // specific version selected (from standversion view)
                var standardViewModel = new StandardVersionViewModel
                {
                    Id = id,
                    StandardReference = standardReference,
                    FromStandardsVersion = true
                };
                standardViewModel.SelectedStandard = (StandardVersion)standardVersions.FirstOrDefault(x => x.Version == version);
                standardViewModel.EarliestVersionEffectiveFrom = standardViewModel.SelectedStandard.VersionEarliestStartDate;
                standardViewModel.Results = new List<StandardVersion>() { standardViewModel.SelectedStandard };
                standardViewModel.ApplicationStatus = await ApplicationStandardStatus(application, standardReference, new List<string>() { version });
                return View("~/Views/Application/Standard/ConfirmStandard.cshtml", standardViewModel);
            }
            else
            {
                // no existing approved versions for this standard
                var standardViewModel = new StandardVersionViewModel
                {
                    Id = id,
                    StandardReference = standardReference,
                    FromStandardsVersion = false
                };
                standardViewModel.Results = standardVersions.Select(s => (StandardVersion)s).ToList();
                standardViewModel.SelectedStandard = (StandardVersion)latestStandard;
                standardViewModel.EarliestVersionEffectiveFrom = earliestStandard.VersionEarliestStartDate;
                if (standardVersions.Count() == 1)
                    standardViewModel.ApplicationStatus = await ApplicationStandardStatus(application, standardReference, new List<string>() { standardVersions.First().Version });
                return View("~/Views/Application/Standard/ConfirmStandard.cshtml", standardViewModel);
            }
        }

        [HttpPost("standard/{id}/confirm-standard/{standardReference}")]
        [HttpPost("standard/{id}/confirm-standard/{standardReference}/{version}")]
        public async Task<IActionResult> ConfirmStandard(StandardVersionViewModel model, Guid id, string standardReference, string version)
        {
            var application = await _applicationApiClient.GetApplication(id);
            if (!CanUpdateApplicationAsync(application))
            {
                return RedirectToAction("Applications", "Application");
            }

            var org = await _orgApiClient.GetEpaOrganisation(application.OrganisationId.ToString());
            var standardVersions = (await _orgApiClient.GetAppliedStandardVersionsForEPAO(org?.OrganisationId, standardReference))
                                        .OrderBy(s => s.Version);

            bool anyExistingVersions = standardVersions.Any(x => x.ApprovedStatus == ApprovedStatus.Approved || x.ApplicationStatus == ApplicationStatus.Submitted);

            AppliedStandardVersion selectedStandard = null;
            string applicationStatus = null;
            List<string> versions = null;

            if (string.IsNullOrWhiteSpace(version))
            {
                selectedStandard = standardVersions.LastOrDefault();
                versions = model.SelectedVersions ?? new List<string> { selectedStandard.Version };
                if (model.SelectedVersions != null)
                    applicationStatus = await ApplicationStandardStatus(application, standardReference, model.SelectedVersions);
            }
            else
            {
                selectedStandard = standardVersions.FirstOrDefault(x => x.Version == version);
                versions = new List<string> { selectedStandard.Version };
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
            else if (anyExistingVersions)
            {
                await _applicationApiClient.UpdateStandardData(id, selectedStandard.LarsCode, selectedStandard.IFateReferenceNumber, selectedStandard.Title, versions, StandardApplicationTypes.Version);

                // update QnA application data to include the version Application Type but remove the Organisation Type
                // as the QnA service does not include AND operations for NotRequiredConditions. The presence of
                // Organisation Type would remove some pages which should be shown in a standard version application
                // when the NotRequiredConditions are combined with an OR operation. The application data can be
                // updated here because a version application is always an additional standard and the update is being
                // done prior to displaying the application and collecting the answers to tagged questions
                var applicationData = await _qnaApiClient.GetApplicationData(application.ApplicationId);
                applicationData.ApplicationType = StandardApplicationTypes.Version;
                applicationData.OrganisationType = null;
                await _qnaApiClient.UpdateApplicationData(application.ApplicationId, applicationData);
            }
            else
            {
                // the QnA application data must not be updated here as this could be a full stage 2 standard application
                // where the tagged questions in stage 1 are required to approve the application, updating the application
                // data would overwrite the tagged questions
                await _applicationApiClient.UpdateStandardData(id, selectedStandard.LarsCode, selectedStandard.IFateReferenceNumber, selectedStandard.Title, versions, StandardApplicationTypes.Full);
            }

            return RedirectToAction("SequenceSignPost", "Application", new { Id = id });
        }

        [HttpGet("standard/opt-in/{referenceNumber}/{version}", Name = OptInStandardVersionRouteGet)]
        [PrivilegeAuthorize(Privileges.ApplyForStandard)]
        public async Task<IActionResult> OptInStandardVersion(string referenceNumber, string version)
        {
            if (string.IsNullOrEmpty(referenceNumber))
                throw new ArgumentException("Value cannot be null or empty", nameof(referenceNumber));

            if (string.IsNullOrEmpty(version))
                throw new ArgumentException("Value cannot be null or empty", nameof(version));

            var standards = await _standardVersionApiClient.GetStandardVersionsByIFateReferenceNumber(referenceNumber);
            var standardVersion = standards.FirstOrDefault(x => x.Version.Equals(version, StringComparison.InvariantCultureIgnoreCase));

            if (standardVersion == null)
                throw new NotFoundException($"The reference number {referenceNumber} version {version} cannot be found");

            var model = new OptInStandardVersionViewModel()
            {
                StandardReference = standardVersion.IFateReferenceNumber,
                StandardTitle = standardVersion.Title,
                Version = standardVersion.Version,
                EffectiveFrom = standardVersion.VersionEarliestStartDate ?? DateTime.Today,
                EffectiveTo = standardVersion.VersionLatestEndDate
            };

            return View(model);
        }

        [HttpPost("standard/opt-in", Name = OptInStandardVersionRoutePost)]
        [PrivilegeAuthorize(Privileges.ApplyForStandard)]
        public async Task<IActionResult> OptInStandardVersion(OptInStandardVersionViewModel model)
        {
            if (model == null)
                throw new ArgumentException("Value cannot be null or empty", nameof(model));

            if (string.IsNullOrEmpty(model.StandardReference))
                throw new ArgumentException($"Value of {nameof(model.StandardReference)} cannot be null or empty");

            if (string.IsNullOrEmpty(model.Version))
                throw new ArgumentException($"Value of {nameof(model.Version)} cannot be null or empty");

            var contactId = await GetUserId();
            var epaOrgId = GetEpaOrgIdFromClaim();

            var approvedVersions = await _standardVersionApiClient.GetEpaoRegisteredStandardVersions(epaOrgId, model.StandardReference);
            if (approvedVersions.FirstOrDefault(p => p.Version.Equals(model.Version, StringComparison.InvariantCultureIgnoreCase)) != null)
            {
                throw new AlreadyExistsException($"Unable to opt in to StandardReference {model.StandardReference} organisation {epaOrgId} already assesses this standard version");
            }

            await _orgApiClient.OrganisationStandardVersionOptIn(
                epaOrgId,
                model.StandardReference,
                model.Version,
                model.EffectiveFrom,
                model.EffectiveTo,
                contactId);

            return RedirectToRoute(OptInStandardVersionConfirmationRouteGet, new { referenceNumber = model.StandardReference, version = model.Version });
        }

        [HttpGet("standard/opt-in/{referenceNumber}/{version}/confirmation", Name = OptInStandardVersionConfirmationRouteGet)]
        [PrivilegeAuthorize(Privileges.ApplyForStandard)]
        public async Task<IActionResult> OptInStandardVersionConfirmation(string referenceNumber, string version)
        {
            if (string.IsNullOrEmpty(referenceNumber))
                throw new ArgumentException("Value cannot be null or empty", nameof(referenceNumber));

            if (string.IsNullOrEmpty(version))
                throw new ArgumentException("Value cannot be null or empty", nameof(version));

            var standardVersions = await _standardVersionApiClient.GetStandardVersionsByIFateReferenceNumber(referenceNumber);
            if (!standardVersions?.Any() ?? false)
                throw new NotFoundException($"The standard reference {referenceNumber} cannot be found");

            var model = new OptInStandardVersionConfirmationViewModel()
            {
                StandardTitle = standardVersions?.FirstOrDefault()?.Title,
                StandardReference = referenceNumber,
                Version = version,
                FeedbackUrl = _config.FeedbackUrl,
            };

            return View(model);
        }

        [HttpGet("standard/opt-out/{referenceNumber}/{version}", Name = OptOutStandardVersionRouteGet)]
        [PrivilegeAuthorize(Privileges.ApplyForStandard)]
        public async Task<IActionResult> OptOutStandardVersion(string referenceNumber, string version)
        {
            if (string.IsNullOrEmpty(referenceNumber))
                throw new ArgumentException("Value cannot be null or empty", nameof(referenceNumber));

            if (string.IsNullOrEmpty(version))
                throw new ArgumentException("Value cannot be null or empty", nameof(version));

            var standards = await _standardVersionApiClient.GetStandardVersionsByIFateReferenceNumber(referenceNumber);
            var standardVersion = standards.FirstOrDefault(x => x.Version.Equals(version, StringComparison.InvariantCultureIgnoreCase));

            if (standardVersion == null)
                throw new NotFoundException($"The reference number {referenceNumber} version {version} cannot be found");

            var model = new OptOutStandardVersionViewModel()
            {
                StandardReference = standardVersion.IFateReferenceNumber,
                StandardTitle = standardVersion.Title,
                Version = standardVersion.Version,
                EffectiveFrom = standardVersion.VersionEarliestStartDate ?? DateTime.Today,
                EffectiveTo = DateTime.Today
            };

            return View(model);
        }

        [HttpPost("standard/opt-out", Name = OptOutStandardVersionRoutePost)]
        [PrivilegeAuthorize(Privileges.ApplyForStandard)]
        public async Task<IActionResult> OptOutStandardVersion(OptOutStandardVersionViewModel model)
        {
            if (model == null)
                throw new ArgumentException("Value cannot be null or empty", nameof(model));

            if (string.IsNullOrEmpty(model.StandardReference))
                throw new ArgumentException($"Value of {nameof(model.StandardReference)} cannot be null or empty");

            if (string.IsNullOrEmpty(model.Version))
                throw new ArgumentException($"Value of {nameof(model.Version)} cannot be null or empty");

            var contactId = await GetUserId();
            var epaOrgId = GetEpaOrgIdFromClaim();

            var approvedVersions = await _standardVersionApiClient.GetEpaoRegisteredStandardVersions(epaOrgId, model.StandardReference);
            if (approvedVersions.FirstOrDefault(p => p.Version.Equals(model.Version, StringComparison.InvariantCultureIgnoreCase)) == null)
            {
                throw new NotFoundException($"Unable to opt out of StandardReference {model.StandardReference} organisation {epaOrgId} does not assesses this standard version");
            }

            await _orgApiClient.OrganisationStandardVersionOptOut(
                epaOrgId,
                model.StandardReference,
                model.Version,
                model.EffectiveFrom,
                model.EffectiveTo,
                contactId);

            return RedirectToRoute(OptOutStandardVersionConfirmationRouteGet, new { referenceNumber = model.StandardReference, version = model.Version });
        }

        [HttpGet("standard/opt-out/{referenceNumber}/{version}/confirmation", Name = OptOutStandardVersionConfirmationRouteGet)]
        [PrivilegeAuthorize(Privileges.ApplyForStandard)]
        public async Task<IActionResult> OptOutStandardVersionConfirmation(string referenceNumber, string version)
        {
            if (string.IsNullOrEmpty(referenceNumber))
                throw new ArgumentException("Value cannot be null or empty", nameof(referenceNumber));

            if (string.IsNullOrEmpty(version))
                throw new ArgumentException("Value cannot be null or empty", nameof(version));

            var standardVersions = await _standardVersionApiClient.GetStandardVersionsByIFateReferenceNumber(referenceNumber);
            if (!standardVersions?.Any() ?? false)
                throw new NotFoundException($"The standard reference {referenceNumber} cannot be found");

            var model = new OptOutStandardVersionConfirmationViewModel()
            {
                StandardTitle = standardVersions?.FirstOrDefault()?.Title,
                StandardReference = referenceNumber,
                Version = version,
                FeedbackUrl = _config.FeedbackUrl,
            };

            return View(model);
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

        private async Task<string> ApplicationStandardStatus(ApplicationResponse application, string iFateReferenceNumber, List<string> versions)
        {
            var validApplicationStatuses = new string[] { ApplicationStatus.InProgress };
            var validApplicationSequenceStatuses = new string[] { ApplicationSequenceStatus.Draft };

            var org = await _orgApiClient.GetEpaOrganisation(application.OrganisationId.ToString());
            var standards = await _orgApiClient.GetAppliedStandardVersionsForEPAO(org?.OrganisationId, iFateReferenceNumber);
            var matchingStandards = standards.Where(x => x.ApplicationId != application.Id &&
                                                            versions.Contains(x.Version));

            if (matchingStandards.Any(x => x.ApprovedStatus == ApprovedStatus.Approved))
                return ApplicationStatus.Approved;
            else
            {
                var inProgressApplications = matchingStandards.Where(x => x.ApprovedStatus == ApprovedStatus.ApplyInProgress &&
                                                validApplicationStatuses.Contains(x.ApplicationStatus));
                foreach (var app in inProgressApplications)
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
    }
}