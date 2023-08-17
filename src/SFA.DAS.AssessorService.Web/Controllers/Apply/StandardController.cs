using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Api.Types.Models.OrganisationStandards;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Api.Client.QnA;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Exceptions;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Extensions;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.StartupConfiguration;
using SFA.DAS.AssessorService.Web.ViewModels.Standard;

namespace SFA.DAS.AssessorService.Web.Controllers.Apply
{
    [Authorize]
    public class StandardController : AssessorController
    {
        private readonly IOrganisationsApiClient _orgApiClient;
        private readonly IQnaApiClient _qnaApiClient;
        private readonly IStandardVersionClient _standardVersionApiClient;
        private readonly IWebConfiguration _config;

        #region Routes
        public const string ApplyStandardSearchRouteGet = nameof(ApplyStandardSearchRouteGet);
        public const string ApplyStandardSearchRoutePost = nameof(ApplyStandardSearchRoutePost);
        public const string ApplyStandardSearchResultsRouteGet = nameof(ApplyStandardSearchResultsRouteGet);
        public const string ApplyStandardConfirmOfqualRouteGet = nameof(ApplyStandardConfirmOfqualRouteGet);
        public const string ApplyStandardConfirmRouteGet = nameof(ApplyStandardConfirmRouteGet);
        public const string ApplyStandardConfirmRoutePost = nameof(ApplyStandardConfirmRoutePost);
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
            IStandardVersionClient standardVersionApiClient, IHttpContextAccessor httpContextAccessor, IWebConfiguration config)
            : base(apiClient, contactsApiClient, httpContextAccessor)
        {
            _orgApiClient = orgApiClient;
            _qnaApiClient = qnaApiClient;
            _standardVersionApiClient = standardVersionApiClient;
            _config = config;
        }

        [PrivilegeAuthorize(Privileges.ApplyForStandard)]
        [HttpGet("standard/add-standard/{search?}", Name = AddStandardSearchRouteGet)]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        [NonAction]
        public IActionResult AddStandardSearch(string search)
        {
            var standardViewModel = new AddStandardSearchViewModel()
            {
                Search = ModelState
                    .GetAttemptedValueWhenInvalid(nameof(AddStandardSearchViewModel.Search), search)
            };

            return View(standardViewModel);
        }

        [PrivilegeAuthorize(Privileges.ApplyForStandard)]
        [HttpPost("standard/add-standard", Name = AddStandardSearchRoutePost)]
        [ModelStatePersist(ModelStatePersist.Store)]
        [NonAction]
        public IActionResult AddStandardSearch(AddStandardSearchViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToRoute(AddStandardSearchRouteGet, new { search = model.Search });
            }

            return RedirectToRoute(AddStandardSearchResultsRouteGet, new { search = model.Search });
        }

        [PrivilegeAuthorize(Privileges.ApplyForStandard)]
        [HttpGet("standard/add-standard/{search}/results", Name = AddStandardSearchResultsRouteGet)]
        [NonAction]
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
                Search = search
            };

            return View(model);
        }

        [PrivilegeAuthorize(Privileges.ApplyForStandard)]
        [HttpGet("standard/add-standard/{search}/{referenceNumber}/choose-versions", Name = AddStandardChooseVersionsRouteGet)]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        [NonAction]
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
        [NonAction]
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
        [NonAction]
        public async Task<IActionResult> AddStandardConfirm(string search, string referenceNumber, [FromQuery] List<string> versions)
        {
            if (string.IsNullOrEmpty(search))
                throw new ArgumentException("Value cannot be null or empty", nameof(search));

            if (string.IsNullOrEmpty(referenceNumber))
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
        [NonAction]
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

            return RedirectToRoute(AddStandardConfirmationRouteGet, new { referenceNumber = model.StandardReference });
        }

        [PrivilegeAuthorize(Privileges.ApplyForStandard)]
        [HttpGet("standard/add-standard/{referenceNumber}/confirmation", Name = AddStandardConfirmationRouteGet)]
        [NonAction]
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
                ApprovedVersions = approvedVersions.ToList()
            };

            return View(model);
        }

        [PrivilegeAuthorize(Privileges.ApplyForStandard)]
        [HttpGet("standard/{id}/apply-standard/{search?}", Name = ApplyStandardSearchRouteGet)]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        public IActionResult ApplyStandardSearch(Guid id, string search)
        {
            if (id == Guid.Empty)
                throw new ArgumentException($"Value of {nameof(id)} cannot be empty");

            var viewModel = new ApplyStandardSearchViewModel
            {
                Id = id,
                Search = ModelState
                    .GetAttemptedValueWhenInvalid(nameof(ApplyStandardSearchViewModel.Search), search)
            };

            return View(viewModel);
        }

        [PrivilegeAuthorize(Privileges.ApplyForStandard)]
        [HttpPost("standard/{id}/apply-standard", Name = ApplyStandardSearchRoutePost)]
        [ModelStatePersist(ModelStatePersist.Store)]
        public IActionResult ApplyStandardSearch(ApplyStandardSearchViewModel model)
        {
            if (model == null)
                throw new ArgumentException("Value cannot be null or empty", nameof(model));

            if (model.Id == Guid.Empty)
                throw new ArgumentException($"Value of {nameof(model.Id)} cannot be empty");

            if (!ModelState.IsValid)
            {
                return RedirectToRoute(ApplyStandardSearchRouteGet, new { id = model.Id, search = model.Search });
            }

            return RedirectToRoute(ApplyStandardSearchResultsRouteGet, new { id = model.Id, search = model.Search });
        }

        [PrivilegeAuthorize(Privileges.ApplyForStandard)]
        [HttpGet("standard/{id}/apply-standard/{search}/results", Name = ApplyStandardSearchResultsRouteGet)]
        public Task<IActionResult> ApplyStandardSearchResults(Guid id, string search)
        {
            if (id == Guid.Empty)
                throw new ArgumentException($"Value of {nameof(id)} cannot be empty");

            if (string.IsNullOrEmpty(search))
                throw new ArgumentException($"Value of {nameof(search)} cannot be null or empty");

            return ApplyStandardSearchResultsInnerAsync(id, search);
        }

        private async Task<IActionResult> ApplyStandardSearchResultsInnerAsync(Guid id, string search)
        {
            var standards = await _standardVersionApiClient.GetLatestStandardVersions();
            var results = standards
                    .Where(s => s.Title.Contains(search, StringComparison.InvariantCultureIgnoreCase))
                    .ToList();

            var viewModel = new ApplyStandardSearchViewModel
            {
                Id = id,
                Search = search,
                Results = results
            };

            return View(viewModel);
        }

        [PrivilegeAuthorize(Privileges.ApplyForStandard)]
        [HttpGet("standard/{id}/apply-standard/{search}/{referenceNumber}/confirm-ofqual", Name = ApplyStandardConfirmOfqualRouteGet)]
        [ApplicationAuthorize(routeId: "Id")]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        public Task<IActionResult> ApplyStandardConfirmOfqual(Guid id, string search, string referenceNumber)
        {
            if (id == Guid.Empty)
                throw new ArgumentException($"Value of {nameof(id)} cannot be empty");

            if (string.IsNullOrEmpty(search))
                throw new ArgumentException($"Value of {nameof(search)} cannot be null or empty");

            if (string.IsNullOrEmpty(referenceNumber))
                throw new ArgumentException($"Value of {nameof(referenceNumber)} cannot be null or empty");

            return ApplyStandardConfirmOfqualInnerAsync(id, search, referenceNumber);
        }

        private async Task<IActionResult> ApplyStandardConfirmOfqualInnerAsync(Guid id, string search, string referenceNumber)
        {
            var application = await _applicationApiClient.GetApplication(id);
            if (!CanUpdateApplicationAsync(application))
            {
                return RedirectToAction("Applications", "Application");
            }

            var org = await _orgApiClient.GetEpaOrganisation(application.OrganisationId.ToString());
            var standardVersions = await _orgApiClient.GetAppliedStandardVersionsForEPAO(org?.OrganisationId, referenceNumber);

            if (!standardVersions?.Any() ?? true)
            {
                return NotFound($"The standard '{referenceNumber}' was not found");
            }

            var latestStandard = standardVersions.OrderBy(s => s.Version).Last();

            var viewModel = new ApplyStandardConfirmOfqualViewModel
            {
                Id = id,
                Search = search,
                SelectedStandard = latestStandard
            };

            return View(viewModel);
        }

        [PrivilegeAuthorize(Privileges.ApplyForStandard)]
        [HttpGet("standard/{id}/apply-standard/{search}/{referenceNumber}/confirm", Name = ApplyStandardConfirmRouteGet)]
        [ApplicationAuthorize(routeId: "Id")]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        public Task<IActionResult> ApplyStandardConfirm(Guid id, string search, string referenceNumber)
        {
            if (id == Guid.Empty)
                throw new ArgumentException($"Value of {nameof(id)} cannot be empty");

            if (string.IsNullOrEmpty(search))
                throw new ArgumentException($"Value of {nameof(search)} cannot be null or empty");

            if (string.IsNullOrEmpty(referenceNumber))
                throw new ArgumentException($"Value of {nameof(referenceNumber)} cannot be null or empty");

            return ApplyStandardConfirmInnerAsync(id, search, referenceNumber);
        }

        private async Task<IActionResult> ApplyStandardConfirmInnerAsync(Guid id, string search, string referenceNumber)
        {
            var application = await _applicationApiClient.GetApplication(id);
            if (!CanUpdateApplicationAsync(application))
            {
                return RedirectToAction("Applications", "Application");
            }

            var org = await _orgApiClient.GetEpaOrganisation(application.OrganisationId.ToString());
            var standardVersions = await _orgApiClient.GetAppliedStandardVersionsForEPAO(org?.OrganisationId, referenceNumber);

            if (!standardVersions?.Any() ?? true)
            {
                return NotFound($"The standard '{referenceNumber}' was not found");
            }

            var earliestStandard = standardVersions.OrderBy(s => s.Version).First();
            var latestStandard = standardVersions.OrderBy(s => s.Version).Last();

            if (standardVersions?.Any(x => x.ApprovedStatus == ApprovedStatus.Approved) ?? false)
            {
                return RedirectToRoute(StandardDetailsRouteGet, new { referenceNumber });
            }
            else if (latestStandard.EqaProviderName == "Ofqual")
            {
                return RedirectToRoute(ApplyStandardConfirmOfqualRouteGet, new { id, search, referenceNumber });
            }

            var viewModel = new ApplyStandardConfirmViewModel
            {
                Id = id,
                Search = search,
                StandardReference = referenceNumber,
                Results = standardVersions.Select(s => (StandardVersion)s).ToList(),
                SelectedStandard = (StandardVersion)latestStandard,
                EarliestVersionEffectiveFrom = earliestStandard.VersionEarliestStartDate,
                ApplicationStatus = await ApplicationStandardStatus(application, referenceNumber, new List<string>() { standardVersions.First().Version }),
                IsConfirmed = ModelState
                    .GetAttemptedValueWhenInvalid(nameof(AddStandardConfirmViewModel.IsConfirmed), false),
                SelectedVersions = ModelState
                    .GetAttemptedValueListWhenInvalid(nameof(AddStandardConfirmViewModel.SelectedVersions), new List<string>(), ',')
            };

            return View(viewModel);
        }

        [PrivilegeAuthorize(Privileges.ApplyForStandard)]
        [HttpPost("standard/{id}/apply-standard/{search}/{referenceNumber}/confirm", Name = ApplyStandardConfirmRoutePost)]
        [ModelStatePersist(ModelStatePersist.Store)]
        public Task<IActionResult> ApplyStandardConfirm(ApplyStandardConfirmViewModel model)
        {
            if (model == null)
                throw new ArgumentException("Value cannot be null or empty", nameof(model));

            if (model.Id == Guid.Empty)
                throw new ArgumentException($"Value of {nameof(model.Id)} cannot be empty");

            if (string.IsNullOrEmpty(model.Search))
                throw new ArgumentException($"Value of {nameof(model.Search)} cannot be null or empty");

            if (string.IsNullOrEmpty(model.StandardReference))
                throw new ArgumentException($"Value of {nameof(model.StandardReference)} cannot be null or empty");

            return ApplyStandardConfirmInnerAsync(model);
        }

        private async Task<IActionResult> ApplyStandardConfirmInnerAsync(ApplyStandardConfirmViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToRoute(ApplyStandardConfirmRouteGet, new { id = model.Id, search = model.Search, referenceNumber = model.StandardReference });
            }

            var application = await _applicationApiClient.GetApplication(model.Id);
            if (!CanUpdateApplicationAsync(application))
            {
                return RedirectToAction("Applications", "Application");
            }

            var org = await _orgApiClient.GetEpaOrganisation(application.OrganisationId.ToString());
            var standardVersions = await _orgApiClient.GetAppliedStandardVersionsForEPAO(org?.OrganisationId, model.StandardReference);

            if (!standardVersions?.Any() ?? true)
            {
                return NotFound($"The standard '{model.StandardReference}' was not found");
            }

            var latestStandard = standardVersions.OrderBy(s => s.Version).Last();

            // the application data is being updated to include the EqaProviderName from the selected standard,
            // this is required for the configuration of the questions via the NotRequired attributes and
            // is done using a dynamic dictionary to preserve any answers to tagged questions which may exist
            // in a multi-sequence application (e.g. the Stage1 + Stage2 organisation application)
            var applicationData = await _qnaApiClient.GetApplicationDataDictionary(application.ApplicationId);
            applicationData[nameof(ApplicationData.Eqap)] = latestStandard.EqaProviderName;
            await _qnaApiClient.UpdateApplicationDataDictionary(application.ApplicationId, applicationData);

            await _applicationApiClient.UpdateStandardData(model.Id, latestStandard.LarsCode, latestStandard.IFateReferenceNumber,
                latestStandard.Title, model.SelectedVersions, StandardApplicationTypes.Full);

            return RedirectToAction("SequenceSignPost", "Application", new { model.Id });
        }

        [HttpGet("standard/opt-in/{referenceNumber}/{version}", Name = OptInStandardVersionRouteGet)]
        [PrivilegeAuthorize(Privileges.ApplyForStandard)]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
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
        [ModelStatePersist(ModelStatePersist.Store)]
        public async Task<IActionResult> OptInStandardVersion(OptInStandardVersionViewModel model)
        {
            if (string.IsNullOrEmpty(model?.StandardReference))
                throw new ArgumentException($"Value of {nameof(model.StandardReference)} cannot be null or empty");

            if (string.IsNullOrEmpty(model?.Version))
                throw new ArgumentException($"Value of {nameof(model.Version)} cannot be null or empty");

            if (!ModelState.IsValid)
            {
                return RedirectToRoute(OptInStandardVersionRouteGet, new { referenceNumber = model.StandardReference, version = model.Version });
            }

            var epaOrgId = GetEpaOrgIdFromClaim();
            var contactId = await GetUserId();

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
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
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
                EffectiveTo = DateTime.Today,
            };

            return View(model);
        }

        [HttpPost("standard/opt-out", Name = OptOutStandardVersionRoutePost)]
        [PrivilegeAuthorize(Privileges.ApplyForStandard)]
        [ModelStatePersist(ModelStatePersist.Store)]
        public async Task<IActionResult> OptOutStandardVersion(OptOutStandardVersionViewModel model)
        {
            if (string.IsNullOrEmpty(model?.StandardReference))
                throw new ArgumentException($"Value of {nameof(model.StandardReference)} cannot be null or empty");

            if (string.IsNullOrEmpty(model?.Version))
                throw new ArgumentException($"Value of {nameof(model.Version)} cannot be null or empty");

            if (!ModelState.IsValid)
            {
                return RedirectToRoute(OptOutStandardVersionRouteGet, new { referenceNumber = model.StandardReference, version = model.Version });
            }

            var contactId = await GetUserId();
            var epaOrgId = GetEpaOrgIdFromClaim();

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
