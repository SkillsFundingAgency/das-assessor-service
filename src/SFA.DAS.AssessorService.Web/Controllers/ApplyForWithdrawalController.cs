using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
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

            // remove any standards where there is an existing withdrawal application
            var applications = await _applicationApiClient.GetWithdrawalApplications(contact.Id);
            var standards = (await _standardsApiClient.GetEpaoRegisteredStandards(org.OrganisationId, 1, int.MaxValue)).Items;

            var filteredStandards = standards.Where(s => !applications.Any(a => a.ApplyData.Apply.StandardReference.Equals(s.ReferenceNumber, StringComparison.InvariantCultureIgnoreCase)
                                                                                        && a.ApplyData.Apply.Versions == null));

            var viewModel = new ChooseStandardForWithdrawalViewModel()
            {
                SelectedStandardForWithdrawal = ModelState.IsValid ? null : modelStateSelectedStandardForWithdrawal,
                Standards = filteredStandards.ToList()
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
                ModelState.AddModelError(nameof(viewModel.SelectedStandardForWithdrawal), "Select a standard");
            }

            if (ModelState.IsValid)
            {
                var contact = await GetUserContact();
                var organisation = await _orgApiClient.GetOrganisationByUserId(contact.Id);

                var standards = await _applicationApiClient.GetStandards();
                var selectedStandard = standards.FirstOrDefault(s => s.StandardId == viewModel.SelectedStandardForWithdrawal);
                var versions = await _standardVersionApiClient.GetEpaoRegisteredStandardVersions(organisation.EndPointAssessorOrganisationId, selectedStandard.ReferenceNumber);

                if (versions.Count() == 1)
                {
                    var id = await CreateWithdrawalApplication(contact, organisation, selectedStandard.StandardId.Value, selectedStandard.ReferenceNumber,
                    selectedStandard.Title,  StandardOrVersion.Standard,  null);

                    return RedirectToAction(
                    nameof(ApplicationController.Sequence),
                    nameof(ApplicationController).RemoveController(),
                    new { Id = id, sequenceNo = ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO });
                }
                else
                    return RedirectToAction(nameof(ChooseStandardVersionForWithdrawal), nameof(ApplyForWithdrawalController).RemoveController(), new { iFateReferenceNumber = selectedStandard.ReferenceNumber });
            }

            return RedirectToAction(nameof(ChooseStandardForWithdrawal), nameof(ApplyForWithdrawalController).RemoveController());
        }

        [HttpGet("ChooseStandardVersionForWithdrawal/{iFateReferenceNumber}", Name = "ChooseStandardVersionForWithdrawal")]
        [ModelStatePersist(ModelStatePersist.RestoreEntry)]
        [TypeFilter(typeof(MenuFilter), Arguments = new object[] { Pages.Dashboard })]
        public async Task<IActionResult> ChooseStandardVersionForWithdrawal(string iFateReferenceNumber)
        {
            var contact = await GetUserContact();
            var organisation = await _orgApiClient.GetOrganisationByUserId(contact.Id);
            
            var applications = (await _applicationApiClient.GetWithdrawalApplications(contact.Id))
                                    .Where(x => x.ApplyData.Apply.StandardReference.Equals(iFateReferenceNumber, StringComparison.InvariantCultureIgnoreCase))
                                    .ToList();

            var versions = await _standardVersionApiClient.GetEpaoRegisteredStandardVersions(organisation.EndPointAssessorOrganisationId, iFateReferenceNumber);

            var viewModel = new ChooseStandardVersionForWithdrawalViewModel()
            {
                Versions = versions.Where(v => !applications.Any(a => a.ApplyData.Apply.Versions != null && a.ApplyData.Apply.Versions.Contains(v.Version)))
                                    .OrderBy(x => x.Version)
                                    .ToList(),
                WholeStandardDisabled = applications.Any()
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
            var versions = await _standardVersionApiClient.GetEpaoRegisteredStandardVersions(organisation.EndPointAssessorOrganisationId, iFateReferenceNumber);

            if (string.IsNullOrWhiteSpace(model.WithdrawalType))
                ModelState.AddModelError(nameof(model.WithdrawalType), "Select whole standard or version(s)");
            else if (model.WithdrawalType == WithdrawalType.SpecificVersions)
            {
                if (model.SelectedVersions == null || !model.SelectedVersions.Any())
                    ModelState.AddModelError(nameof(model.SelectedVersions), "Select at least one version");
                else if (model.SelectedVersions.Count == versions.Count())
                    ModelState.AddModelError(nameof(model.SelectedVersions), "Select less versions or go back and select whole standard");
            }
           
            if (ModelState.IsValid)
            {
                var firstVersion = versions.First();
                var id = await CreateWithdrawalApplication(contact, organisation, 
                    firstVersion.LarsCode, 
                    firstVersion.IFateReferenceNumber, 
                    firstVersion.Title,
                    (model.WithdrawalType == WithdrawalType.SpecificVersions)? StandardOrVersion.Version : StandardOrVersion.Standard,
                    (model.WithdrawalType == WithdrawalType.SpecificVersions) ? model.SelectedVersions : null);

                return RedirectToAction(
                nameof(ApplicationController.Sequence),
                nameof(ApplicationController).RemoveController(),
                new { Id = id, sequenceNo = ApplyConst.STANDARD_WITHDRAWAL_SEQUENCE_NO });
            }
            else
            {
                model.Versions = versions.OrderBy(x => x.Version).ToList();
                return View(model);
            }
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
    }
}