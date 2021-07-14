using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Apply;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Constants;
using SFA.DAS.AssessorService.Web.Controllers.Apply;
using SFA.DAS.AssessorService.Web.Extensions;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.StartupConfiguration;
using SFA.DAS.AssessorService.Web.ViewModels.ApplyForWithdrawal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Web.Controllers
{
    [PrivilegeAuthorize(Privileges.ApplyForStandard)]
    [Authorize]
    public class ApplyForWithdrawalController : AssessorController
    {
        private readonly IApplicationService _applicationService;
        private readonly IOrganisationsApiClient _orgApiClient;
        private readonly IStandardsApiClient _standardsApiClient;
        private readonly IStandardVersionClient _standardVersionApiClient;
        private readonly IWebConfiguration _config;

        public ApplyForWithdrawalController(IApplicationService applicationService, IOrganisationsApiClient orgApiClient, 
            IApplicationApiClient applicationApiClient, IContactsApiClient contactsApiClient, IHttpContextAccessor httpContextAccessor, 
            IStandardsApiClient standardsApiClient, IStandardVersionClient standardVersionApiClient, IWebConfiguration config)
            : base (applicationApiClient, contactsApiClient, httpContextAccessor)
        {
            _applicationService = applicationService;
            _orgApiClient = orgApiClient;
            _standardsApiClient = standardsApiClient;
            _standardVersionApiClient = standardVersionApiClient;
            _config = config;
        }

        [HttpGet]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Dashboard })]
        public async Task<IActionResult> Index()
        {
            var userId = await GetUserId();
            var applications = await _applicationApiClient.GetWithdrawalApplications(userId);

            return View(applications?.Count() == 0);
        }

        [HttpGet("/your-withdrawal-notifications")]
        public async Task<IActionResult> WithdrawalApplications()
        {
            var userId = await GetUserId();
            var applications = await _applicationApiClient.GetWithdrawalApplications(userId);
            return View(applications);
        }

        [HttpGet]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Dashboard })]
        public IActionResult TypeOfWithdrawal()
        {
            return View();
        }

        [HttpPost]
        [ModelStatePersist(ModelStatePersist.Store)]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Dashboard })]
        public async Task<IActionResult> TypeOfWithdrawal(TypeOfWithdrawalViewModel viewModel)
        {
            if(string.IsNullOrEmpty(viewModel.TypeOfWithdrawal))
            {
                ModelState.AddModelError(nameof(viewModel.TypeOfWithdrawal), "Select standard or register");
            }
            
            if (ModelState.IsValid)
            {
                if (viewModel.TypeOfWithdrawal == ApplicationTypes.OrganisationWithdrawal)
                {
                    var contact = await GetUserContact();
                    var organisation = await _orgApiClient.GetOrganisationByUserId(contact.Id);

                    var createApplicationRequest = await _applicationService.BuildOrganisationWithdrawalRequest(
                        contact, 
                        organisation, 
                        _config.ReferenceFormat);

                    var id = await _applicationApiClient.CreateApplication(createApplicationRequest);

                    return RedirectToAction(
                        nameof(ApplicationController.Sequence), 
                        nameof(ApplicationController).RemoveController(), 
                        new { Id = id, sequenceNo = ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO });
                }
                else if (viewModel.TypeOfWithdrawal == ApplicationTypes.StandardWithdrawal)
                {
                    return RedirectToAction(nameof(ChooseStandardForWithdrawal), nameof(ApplyForWithdrawalController).RemoveController());
                }
            }

            return RedirectToAction(nameof(TypeOfWithdrawal), nameof(ApplyForWithdrawalController).RemoveController());
        }

        [HttpGet]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Dashboard })]
        public async Task<IActionResult> ChooseStandardForWithdrawal(int? pageIndex)
        {
            var contact = await GetUserContact();
            var org = await _orgApiClient.GetEpaOrganisationById(contact.OrganisationId?.ToString());

            var modelStateSelectedStandardForWithdrawal =
                int.TryParse(
                    ModelState[nameof(ChooseStandardForWithdrawalViewModel.SelectedStandardForWithdrawal)]?.AttemptedValue,
                    out int result)
                ? result
                : (int?)null;

            var applications = await GetWithdrawalApplications(contact.Id);
            var standards = await _standardsApiClient.GetEpaoRegisteredStandards(org.OrganisationId, pageIndex ?? 1, 10);

            var viewModel = new ChooseStandardForWithdrawalViewModel()
            {
                SelectedStandardForWithdrawal = ModelState.IsValid ? null : modelStateSelectedStandardForWithdrawal,
                Standards = standards.Convert(x => new RegisteredStandardsViewModel()
                {
                    StandardName = x.StandardName,
                    Level = x.Level,
                    ReferenceNumber = x.ReferenceNumber,
                    NumberOfVersions = x.NumberOfVersions,
                    ApplicationId = applications.FirstOrDefault(a => !string.IsNullOrWhiteSpace(a.StandardReference) &&
                                                                    a.StandardReference.Equals(x.ReferenceNumber, StringComparison.InvariantCultureIgnoreCase) &&
                                                                    a.ApplyData.Apply.Versions == null)?.Id
                })
            };

            return View(viewModel);
        }

        [HttpGet("WholeStandardOrVersion/{iFateReferenceNumber}", Name = "WholeStandardOrVersion")]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Dashboard })]
        public IActionResult WholeStandardOrVersion(string iFateReferenceNumber)
        {
            return View();
        }

        [HttpPost("WholeStandardOrVersion/{iFateReferenceNumber}")]
        [ModelStatePersist(ModelStatePersist.Store)]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Dashboard })]
        public IActionResult WholeStandardOrVersion(string iFateReferenceNumber, WholeStandardOrVersionViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.WithdrawalType))
                ModelState.AddModelError(nameof(model.WithdrawalType), "Select whole standard or version(s)");

            if (ModelState.IsValid)
            {
                if (model.WithdrawalType == WithdrawalType.WholeStandard)
                    return RedirectToAction(
                        nameof(CheckWithdrawalRequest),
                        nameof(ApplyForWithdrawalController).RemoveController(),
                        new { iFateReferenceNumber = iFateReferenceNumber, backAction = nameof(WholeStandardOrVersion) });
                else
                    return RedirectToAction(
                        nameof(ReviewStandardVersions), 
                        nameof(ApplyForWithdrawalController).RemoveController(),
                        new { iFateReferenceNumber = iFateReferenceNumber });
            }
            else
                return View(model);
        }

        [HttpGet("ReviewStandardVersions/{iFateReferenceNumber}", Name = "ReviewStandardVersions")]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Dashboard })]
        public async Task<IActionResult> ReviewStandardVersions(string iFateReferenceNumber)
        {
            var contact = await GetUserContact();
            var organisation = await _orgApiClient.GetOrganisationByUserId(contact.Id);
            var applications = await GetWithdrawalApplications(contact.Id, iFateReferenceNumber);

            var versions = await _standardVersionApiClient.GetEpaoRegisteredStandardVersions(organisation.EndPointAssessorOrganisationId, iFateReferenceNumber);
            var standard = versions.First();

            return View(new ReviewStandardVersionsViewModel()
            {
                IFateReferenceNumber = iFateReferenceNumber,
                Level = standard.Level,
                StandardName = standard.Title,
                Versions = versions.Select(x => new ReviewStandardVersion()
                                        {
                                            Version = x.Version,
                                            AbleToWithdraw = !applications.Any(a => a.ApplyData.Apply.Versions != null &&
                                                                                    a.ApplyData.Apply.Versions.Contains(x.Version))
                                        })
                                        .OrderByDescending(x => x.Version)
                                        .ToList()
            });
        }

        [HttpGet("ChooseStandardVersionForWithdrawal/{iFateReferenceNumber}", Name = "ChooseStandardVersionForWithdrawal")]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Dashboard })]
        public async Task<IActionResult> ChooseStandardVersionForWithdrawal(string iFateReferenceNumber)
        {
            var contact = await GetUserContact();
            var organisation = await _orgApiClient.GetOrganisationByUserId(contact.Id);

            var applications = await GetWithdrawalApplications(contact.Id, iFateReferenceNumber);

            var versions = await _standardVersionApiClient.GetEpaoRegisteredStandardVersions(organisation.EndPointAssessorOrganisationId, iFateReferenceNumber);
            var standard = versions.First();

            var viewModel = new ChooseStandardVersionForWithdrawalViewModel()
            {
                IFateReferenceNumber = iFateReferenceNumber,
                Level = standard.Level,
                StandardName = standard.Title,
                Versions = versions.Where(v => !applications.Any(a => a.ApplyData.Apply.Versions != null && a.ApplyData.Apply.Versions.Contains(v.Version)))
                                    .OrderByDescending(x => x.Version)
                                    .ToList(),
            };

            return View(viewModel);
        }

        [HttpPost("ChooseStandardVersionForWithdrawal/{iFateReferenceNumber}")]
        [ModelStatePersist(ModelStatePersist.Store)]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Dashboard })]
        public async Task<IActionResult> ChooseStandardVersionForWithdrawal(string iFateReferenceNumber, ChooseStandardVersionForWithdrawalViewModel model)
        {
            var contact = await GetUserContact();
            var organisation = await _orgApiClient.GetOrganisationByUserId(contact.Id);
            var applications = await GetWithdrawalApplications(contact.Id, iFateReferenceNumber);

            var versions = (await _standardVersionApiClient.GetEpaoRegisteredStandardVersions(organisation.EndPointAssessorOrganisationId, iFateReferenceNumber))
                            .Where(v => !applications.Any(a => a.ApplyData.Apply.Versions != null && a.ApplyData.Apply.Versions.Contains(v.Version)))
                            .OrderByDescending(x => x.Version)
                            .ToList();

            var standard = versions.First();

            if (model.SelectedVersions == null || !model.SelectedVersions.Any())
                ModelState.AddModelError(nameof(model.SelectedVersions), "Select at least one version");
            else if (model.SelectedVersions.Count == versions.Count)
                ModelState.AddModelError(nameof(model.SelectedVersions), "Select less versions or go back and select whole standard");

            if (ModelState.IsValid)
            {
                var selectedVersions = model.SelectedVersions
                    .OrderBy(x => decimal.Parse(x));

                return RedirectToAction(
                nameof(CheckWithdrawalRequest),
                nameof(ApplyForWithdrawalController).RemoveController(),
                new { iFateReferenceNumber = iFateReferenceNumber, versionsToWithdrawal = string.Join(",", selectedVersions), backAction = nameof(ChooseStandardVersionForWithdrawal) });
            }
            else
            {
                model.IFateReferenceNumber = iFateReferenceNumber;
                model.Level = standard.Level;
                model.StandardName = standard.Title;
                model.Versions = versions;
                return View(model);
            }
        }

        [HttpGet("CheckWithdrawalRequest/{iFateReferenceNumber}/{versionsToWithdrawal?}", Name = "CheckWithdrawalRequest")]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Dashboard })]
        public async Task<IActionResult> CheckWithdrawalRequest(string iFateReferenceNumber, string versionsToWithdrawal, [FromQuery]string backAction)
        {
            var contact = await GetUserContact();
            var organisation = await _orgApiClient.GetOrganisationByUserId(contact.Id);
            var versions = await _standardVersionApiClient.GetEpaoRegisteredStandardVersions(organisation.EndPointAssessorOrganisationId, iFateReferenceNumber);
            var standard = versions.First();

            var applications = await GetInProgressWithdrawalVersionApplications(contact.Id, iFateReferenceNumber);

            return View(new CheckWithdrawalRequestViewModel()
            {
                IFateReferenceNumber = iFateReferenceNumber,
                Level = standard.Level,
                StandardName = standard.Title,
                Versions = versionsToWithdrawal,
                InProgressVersionWithdrawals = string.IsNullOrWhiteSpace(versionsToWithdrawal) && applications.Any(),
                BackAction = backAction
            });
        }

        [HttpPost("CheckWithdrawalRequest/{iFateReferenceNumber}/{versionsToWithdrawal?}")]
        [ModelStatePersist(ModelStatePersist.Store)]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Dashboard })]
        public async Task<IActionResult> CheckWithdrawalRequest(string iFateReferenceNumber, string versionsToWithdrawal, [FromQuery] string backAction, CheckWithdrawalRequestViewModel model)
        {
            var contact = await GetUserContact();
            var organisation = await _orgApiClient.GetOrganisationByUserId(contact.Id);
            var versions = await _standardVersionApiClient.GetEpaoRegisteredStandardVersions(organisation.EndPointAssessorOrganisationId, iFateReferenceNumber);
            var standard = versions.First();

            if (string.IsNullOrWhiteSpace(model.Continue))
                ModelState.AddModelError(nameof(model.Continue), "Select Yes or No");

            if (!ModelState.IsValid)
            {
                model.IFateReferenceNumber = iFateReferenceNumber;
                model.Level = standard.Level;
                model.StandardName = standard.Title;
                model.BackAction = backAction;
                return View(model);
            }

            if (model.Continue.Equals("yes", StringComparison.InvariantCultureIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(versionsToWithdrawal))
                {
                    var applications = await GetInProgressWithdrawalVersionApplications(contact.Id, iFateReferenceNumber);
                    if (applications.Any())
                    {
                        await _applicationApiClient.DeleteApplications(new DeleteApplicationsRequest()
                        {
                            ApplicationIds = applications.Select(x => x.Id),
                            DeletingContactId = null
                        });
                    }
                }

                var id = await CreateWithdrawalApplication(contact, organisation,
                            standard.LarsCode,
                            iFateReferenceNumber,
                            standard.Title,
                            string.IsNullOrWhiteSpace(versionsToWithdrawal) ? StandardOrVersion.Standard : StandardOrVersion.Version,
                            versionsToWithdrawal?.Split(","));

                return RedirectToAction(
                        nameof(ApplicationController.Sequence),
                        nameof(ApplicationController).RemoveController(),
                        new { Id = id, sequenceNo = ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO });
            }
            else
                return RedirectToAction(
                        nameof(WithdrawalApplications),
                        nameof(ApplyForWithdrawalController).RemoveController());
        }

        private async Task<Guid> CreateWithdrawalApplication(ContactResponse contact, OrganisationResponse organisation, int larsCode, string iFateReferenceNumber, string standardTitle, string standardOrVersion, IEnumerable<string> versions)
        {
            var createApplicationRequest = await _applicationService.BuildStandardWithdrawalRequest(
                   contact,
                   organisation,
                   larsCode,
                   _config.ReferenceFormat,
                   standardOrVersion);

            var id = await _applicationApiClient.CreateApplication(createApplicationRequest);

            await _applicationApiClient.UpdateStandardData(id, larsCode, iFateReferenceNumber, standardTitle, versions?.ToList(), ApplicationTypes.StandardWithdrawal);

            return id;
        }

        private async Task<List<ApplicationResponse>> GetWithdrawalApplications(Guid contactId, string iFateReferenceNumber = null)
        {
            return (await _applicationApiClient.GetWithdrawalApplications(contactId))
                    .Where(x => x.IsStandardWithdrawalApplication &&
                                (iFateReferenceNumber == null || 
                                    (!string.IsNullOrWhiteSpace(x.StandardReference) &&
                                        x.StandardReference.Equals(iFateReferenceNumber, StringComparison.InvariantCultureIgnoreCase))) &&
                                (x.ApplicationStatus == ApplicationStatus.InProgress ||
                                    x.ApplicationStatus == ApplicationStatus.Submitted ||
                                    x.ApplicationStatus == ApplicationStatus.FeedbackAdded ||
                                    x.ApplicationStatus == ApplicationStatus.Resubmitted ||
                                    x.ApplicationStatus == ApplicationStatus.Approved))
                    .ToList();

        }

        private async Task<List<ApplicationResponse>> GetInProgressWithdrawalVersionApplications(Guid contactId, string iFateReferenceNumber)
        {
            return (await GetWithdrawalApplications(contactId))
                    .Where(x => x.IsStandardWithdrawalApplication &&
                                !string.IsNullOrWhiteSpace(x.StandardReference) &&
                                x.StandardReference.Equals(iFateReferenceNumber, StringComparison.InvariantCultureIgnoreCase) &&
                                (x.ApplicationStatus == ApplicationStatus.InProgress ||
                                    x.ApplicationStatus == ApplicationStatus.Submitted ||
                                    x.ApplicationStatus == ApplicationStatus.FeedbackAdded ||
                                    x.ApplicationStatus == ApplicationStatus.Resubmitted) &&
                                x.ApplyData.Apply.Versions != null)
                    .ToList();
        }
    }
}