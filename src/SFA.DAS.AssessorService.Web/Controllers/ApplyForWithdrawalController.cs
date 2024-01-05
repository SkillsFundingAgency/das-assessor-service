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
        private readonly IStandardVersionApiClient _standardVersionApiClient;
        private readonly IWebConfiguration _config;

        #region Routes
        public const string WithdrawalApplicationsRouteGet = nameof(WithdrawalApplicationsRouteGet);
        public const string TypeofWithdrawalRouteGet = nameof(TypeofWithdrawalRouteGet);
        public const string TypeofWithdrawalRoutePost = nameof(TypeofWithdrawalRoutePost);
        public const string ChooseStandardForWithdrawalRouteGet = nameof(ChooseStandardForWithdrawalRouteGet);
        public const string CheckWithdrawalRequestRouteGet = nameof(CheckWithdrawalRequestRouteGet);
        public const string CheckWithdrawalRequestRoutePost = nameof(CheckWithdrawalRequestRoutePost);
        #endregion

        public ApplyForWithdrawalController(IApplicationService applicationService, IOrganisationsApiClient orgApiClient, 
            IApplicationApiClient applicationApiClient, IContactsApiClient contactsApiClient, IHttpContextAccessor httpContextAccessor, 
            IStandardsApiClient standardsApiClient, IStandardVersionApiClient standardVersionApiClient, IWebConfiguration config)
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

        [HttpGet("withdrawal/requests", Name = WithdrawalApplicationsRouteGet)]
        public async Task<IActionResult> WithdrawalApplications()
        {
            var userId = await GetUserId();
            var applications = await _applicationApiClient.GetWithdrawalApplications(userId);
            return View(applications);
        }

        [HttpGet("withdrawal/type", Name = TypeofWithdrawalRouteGet)]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Dashboard })]
        public IActionResult TypeOfWithdrawal()
        {
            return View();
        }

        [HttpPost("withdrawal/type", Name = TypeofWithdrawalRoutePost)]
        [ModelStatePersist(ModelStatePersist.Store)]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Dashboard })]
        public IActionResult TypeOfWithdrawal(TypeOfWithdrawalViewModel viewModel)
        {
            if(!ModelState.IsValid)
            {
                return RedirectToRoute(TypeofWithdrawalRouteGet);
            }
            
            if (viewModel.TypeOfWithdrawal == ApplicationTypes.OrganisationWithdrawal)
            {
                return RedirectToRoute(CheckWithdrawalRequestRouteGet, new { backRouteName = TypeofWithdrawalRouteGet });
            }
            else if (viewModel.TypeOfWithdrawal == ApplicationTypes.StandardWithdrawal)
            {
                return RedirectToRoute(ChooseStandardForWithdrawalRouteGet);
            }

            return RedirectToRoute(DashboardController.DashboardIndexRouteGet);
        }

        [HttpGet("withdrawal/choose-standard", Name = ChooseStandardForWithdrawalRouteGet)]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Dashboard })]
        public async Task<IActionResult> ChooseStandardForWithdrawal(int? pageIndex)
        {
            var contact = await GetUserContact();
            var org = await _orgApiClient.GetEpaOrganisationById(contact.OrganisationId?.ToString());

            var applications = await GetWithdrawalApplications(contact.Id);
            var standards = await _standardsApiClient.GetEpaoRegisteredStandards(org.OrganisationId, pageIndex ?? 1, 10);

            var viewModel = new ChooseStandardForWithdrawalViewModel()
            {
                Standards = standards.Convert(x => new RegisteredStandardsViewModel()
                {
                    StandardName = x.StandardName,
                    Level = x.Level,
                    ReferenceNumber = x.ReferenceNumber,

                    // find a previous withdrawal application for the same reference number, if there is
                    // one then this is used to redirect the user to the previous withdrawal for the same
                    // standard this will prevent a withdrawal from the same standard twice i.e. it will
                    // prevent the withdrawal from a reapply for the same standard which most likely was
                    // not the intention see QF-1608
                    ApplicationId = applications.FirstOrDefault(a => !string.IsNullOrWhiteSpace(a.StandardReference) &&
                                                                    a.StandardReference.Equals(x.ReferenceNumber, StringComparison.InvariantCultureIgnoreCase) &&
                                                                    a.ApplyData.Apply.Versions == null)?.Id
                })
            };

            return View(viewModel);
        }

        [HttpGet("withdrawal/check/{ifateReferenceNumber?}", Name = CheckWithdrawalRequestRouteGet)]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Dashboard })]
        public async Task<IActionResult> CheckWithdrawalRequest(string ifateReferenceNumber)
        {
            var contact = await GetUserContact();
            var organisation = await _orgApiClient.GetOrganisationByUserId(contact.Id);

            if (string.IsNullOrWhiteSpace(ifateReferenceNumber))
            {
                return View(new CheckWithdrawalRequestViewModel()
                {
                    OrganisationName = organisation.EndPointAssessorName
                });
            }
            else
            {
                var standardVersions = await _standardVersionApiClient.GetEpaoRegisteredStandardVersions(organisation.EndPointAssessorOrganisationId, ifateReferenceNumber);
                var standard = standardVersions.First();

                return View(new CheckWithdrawalRequestViewModel()
                {
                    IfateReferenceNumber = ifateReferenceNumber,
                    Level = standard.Level,
                    StandardName = standard.Title
                });
            }
        }

        [HttpPost("withdrawal/check/{iFateReferenceNumber?}", Name = CheckWithdrawalRequestRoutePost)]
        [ModelStatePersist(ModelStatePersist.Store)]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Dashboard })]
        public async Task<IActionResult> CheckWithdrawalRequest(CheckWithdrawalRequestViewModel model)
        {
            var contact = await GetUserContact();
            var organisation = await _orgApiClient.GetOrganisationByUserId(contact.Id);

            if (!ModelState.IsValid)
            {
                return RedirectToRoute(CheckWithdrawalRequestRouteGet, new { ifateReferenceNumber = model.IfateReferenceNumber });
            }

            if (model.Continue.Equals("no", StringComparison.InvariantCultureIgnoreCase))
                return RedirectToRoute(WithdrawalApplicationsRouteGet);

            if (string.IsNullOrWhiteSpace(model.IfateReferenceNumber))
            {
                var id = await CreateOrganisationWithdrawalApplication(contact, organisation);

                return RedirectToRoute(ApplicationController.SequenceRouteGet,
                    new { Id = id, sequenceNo = ApplyConst.ORGANISATION_WITHDRAWAL_SEQUENCE_NO });
            }
            else
            {
                var standardVersions = await _standardVersionApiClient.GetEpaoRegisteredStandardVersions(organisation.EndPointAssessorOrganisationId, model.IfateReferenceNumber);
                var standard = standardVersions.First();

                var id = await CreateStandardWithdrawalApplication(contact, organisation,
                            standard.LarsCode,
                            model.IfateReferenceNumber,
                            standard.Title);

                return RedirectToRoute(ApplicationController.SequenceRouteGet,
                        new { Id = id, sequenceNo = ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO });
            }
        }

        private async Task<Guid> CreateStandardWithdrawalApplication(ContactResponse contact, OrganisationResponse organisation, int larsCode, string iFateReferenceNumber, string standardTitle)
        {
            var createApplicationRequest = await _applicationService.BuildStandardWithdrawalRequest(
                   contact,
                   organisation,
                   larsCode,
                   _config.ReferenceFormat);

            var id = await _applicationApiClient.CreateApplication(createApplicationRequest);
            await _applicationApiClient.UpdateStandardData(id, larsCode, iFateReferenceNumber, standardTitle, null, StandardApplicationTypes.StandardWithdrawal);

            return id;
        }

        private async Task<Guid> CreateOrganisationWithdrawalApplication(ContactResponse contact, OrganisationResponse organisation)
        {
            var createApplicationRequest = await _applicationService.BuildOrganisationWithdrawalRequest(
                        contact,
                        organisation,
                        _config.ReferenceFormat);

            return await _applicationApiClient.CreateApplication(createApplicationRequest);
        }

        private async Task<List<ApplicationResponse>> GetWithdrawalApplications(Guid contactId)
        {
            return (await _applicationApiClient.GetWithdrawalApplications(contactId))
                    .Where(x => x.IsStandardWithdrawalApplication &&
                                (x.ApplicationStatus == ApplicationStatus.InProgress ||
                                    x.ApplicationStatus == ApplicationStatus.Submitted ||
                                    x.ApplicationStatus == ApplicationStatus.FeedbackAdded ||
                                    x.ApplicationStatus == ApplicationStatus.Resubmitted ||
                                    x.ApplicationStatus == ApplicationStatus.Approved))
                    .ToList();

        }
    }
}