using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Settings;
using SFA.DAS.AssessorService.Web.Constants;
using SFA.DAS.AssessorService.Web.Controllers.Apply;
using SFA.DAS.AssessorService.Web.Extensions;
using SFA.DAS.AssessorService.Web.Infrastructure;
using SFA.DAS.AssessorService.Web.StartupConfiguration;
using SFA.DAS.AssessorService.Web.ViewModels.ApplyForWithdrawal;
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
        private readonly IWebConfiguration _config;

        public ApplyForWithdrawalController(IApplicationService applicationService, IOrganisationsApiClient orgApiClient, 
            IApplicationApiClient applicationApiClient, IContactsApiClient contactsApiClient, IHttpContextAccessor httpContextAccessor, 
            IStandardsApiClient standardsApiClient, IWebConfiguration config)
            : base (applicationApiClient, contactsApiClient, httpContextAccessor)
        {
            _applicationService = applicationService;
            _orgApiClient = orgApiClient;
            _standardsApiClient = standardsApiClient;
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
                ModelState.AddModelError(nameof(viewModel.TypeOfWithdrawal), "Select if you’re withdrawing from a standard or the register");
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
        public async Task<IActionResult> ChooseStandardForWithdrawal()
        {
            var contact = await GetUserContact();
            var org = await _orgApiClient.GetEpaOrganisationById(contact.OrganisationId?.ToString());

            var modelStateSelectedStandardForWithdrawal =
                int.TryParse(
                    ModelState[nameof(ChooseStandardForWithdrawalViewModel.SelectedStandardForWithdrawal)]?.AttemptedValue,
                    out int result)
                ? result
                : (int?)null;

            var viewModel = new ChooseStandardForWithdrawalViewModel()
            {
                SelectedStandardForWithdrawal = ModelState.IsValid ? null : modelStateSelectedStandardForWithdrawal,
                Standards = await _standardsApiClient.GetEpaoRegisteredStandards(org.OrganisationId, 1, int.MaxValue)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ModelStatePersist(ModelStatePersist.Store)]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Dashboard })]
        public async Task<IActionResult> ChooseStandardForWithdrawal(ChooseStandardForWithdrawalViewModel viewModel)
        {
            if (!viewModel.SelectedStandardForWithdrawal.HasValue)
            {
                ModelState.AddModelError(nameof(viewModel.SelectedStandardForWithdrawal), "Select the standard you want to withdraw from assessing");
            }

            if (ModelState.IsValid)
            {
                var contact = await GetUserContact();
                var organisation = await _orgApiClient.GetOrganisationByUserId(contact.Id);

                var createApplicationRequest = await _applicationService.BuildStandardWithdrawalRequest(
                    contact, 
                    organisation, 
                    viewModel.SelectedStandardForWithdrawal.Value, 
                    _config.ReferenceFormat);
                
                var id = await _applicationApiClient.CreateApplication(createApplicationRequest);

                var standards = await _applicationApiClient.GetStandards();
                var selectedStandard = standards.FirstOrDefault(s => s.StandardId == viewModel.SelectedStandardForWithdrawal);
                
                await _applicationApiClient.UpdateStandardData(id, selectedStandard.Id, selectedStandard.ReferenceNumber, selectedStandard.Title, null);

                return RedirectToAction(
                    nameof(ApplicationController.Sequence),
                    nameof(ApplicationController).RemoveController(),
                    new { Id = id, sequenceNo = ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO });
            }

            return RedirectToAction(nameof(ChooseStandardForWithdrawal), nameof(ApplyForWithdrawalController).RemoveController());
        }
    }
}