namespace SFA.DAS.AssessorService.Web.Staff.Controllers.Roatp
{
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
    using System.Threading.Tasks;
    using ViewModels.Roatp;
    using SFA.DAS.AssessorService.Web.Staff.Domain;
    using SFA.DAS.AssessorService.Api.Types.Models.Roatp;
    using System.Collections.Generic;

    public class UpdateRoatpOrganisationController : RoatpSearchResultsControllerBase
    {
        private ILogger<UpdateRoatpOrganisationController> _logger;

        public UpdateRoatpOrganisationController(ILogger<UpdateRoatpOrganisationController> logger, IRoatpApiClient apiClient,
            IRoatpSessionService sessionService)
        {
            _logger = logger;
            _apiClient = apiClient;
            _sessionService = sessionService;
        }

        [Route("change-legal-name")]
        public async Task<IActionResult> UpdateOrganisationLegalName()
        {
            var searchModel = _sessionService.GetSearchResults();

            var model = new UpdateOrganisationLegalNameViewModel
            {
                CurrentLegalName = searchModel.SelectedResult.LegalName,
                LegalName = searchModel.SelectedResult.LegalName,
                OrganisationId = searchModel.SelectedResult.Id
            };

            return View("~/Views/Roatp/UpdateOrganisationLegalName.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLegalName(UpdateOrganisationLegalNameViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Roatp/UpdateOrganisationLegalName.cshtml", model);
            }

            model.UpdatedBy = HttpContext.User.OperatorName();

            var result = await _apiClient.UpdateOrganisationLegalName(CreateUpdateLegalNameRequest(model));

            if (result)
            {
                return await RefreshSearchResults();
            }

            return View("~/Views/Roatp/UpdateOrganisationLegalName.cshtml", model);
        }

        [Route("change-status")]
        public async Task<IActionResult> UpdateOrganisationStatus()
        {
            var searchModel = _sessionService.GetSearchResults();

            var organisationStatuses = _apiClient.GetOrganisationStatuses().Result.OrderBy(x => x.Status);
            var removedReasons = _apiClient.GetRemovedReasons().Result.OrderBy(x => x.Id);
            
            var model = new UpdateOrganisationStatusViewModel
            {
                LegalName = searchModel.SelectedResult.LegalName,
                OrganisationId = searchModel.SelectedResult.Id,
                OrganisationStatusId = searchModel.SelectedResult.OrganisationStatus.Id,
                OrganisationStatuses = organisationStatuses,
                RemovedReasons = removedReasons
            };
            if (model.OrganisationStatusId == 0) // Removed
            {
                model.RemovedReasonId = searchModel.SelectedResult.OrganisationData.RemovedReason.Id;
            }
            return View("~/Views/Roatp/UpdateOrganisationStatus.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(UpdateOrganisationStatusViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Roatp/UpdateOrganisationStatus.cshtml", model);
            }

            model.UpdatedBy = HttpContext.User.OperatorName();

            var result = await _apiClient.UpdateOrganisationStatus(CreateUpdateOrganisationStatusRequest(model));

            if (result)
            {
                return await RefreshSearchResults();
            }

            return View("~/Views/Roatp/UpdateOrganisationStatus.cshtml", model);
        }
				
		[Route("change-trading-name")]
        public async Task<IActionResult> UpdateOrganisationTradingName()
        {
            var searchModel = _sessionService.GetSearchResults();

            var model = new UpdateOrganisationTradingNameViewModel
            {
                LegalName = searchModel.SelectedResult.LegalName,
                TradingName = searchModel.SelectedResult.TradingName,
                OrganisationId = searchModel.SelectedResult.Id
            };

            return View("~/Views/Roatp/UpdateOrganisationTradingName.cshtml", model);
        }

		[HttpPost]
        public async Task<IActionResult> UpdateTradingName(UpdateOrganisationTradingNameViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Roatp/UpdateOrganisationTradingName.cshtml", model);
            }

            model.UpdatedBy = HttpContext.User.OperatorName();

            var result = await _apiClient.UpdateOrganisationTradingName(CreateUpdateTradingNameRequest(model));

            if (result)
            {
                return await RefreshSearchResults();
            }

            return View("~/Views/Roatp/UpdateOrganisationTradingName.cshtml", model);
        }
        
        [Route("change-parent-company-guarantee")]
        public async Task<IActionResult> UpdateOrganisationParentCompanyGuarantee()
        {
            var searchModel = _sessionService.GetSearchResults();

            var model = new UpdateOrganisationParentCompanyGuaranteeViewModel
            {
                ParentCompanyGuarantee = searchModel.SelectedResult.OrganisationData.ParentCompanyGuarantee,
                OrganisationId = searchModel.SelectedResult.Id,
                LegalName = searchModel.SelectedResult.LegalName
            };

            return View("~/Views/Roatp/UpdateOrganisationParentCompanyGuarantee.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateParentCompanyGuarantee(UpdateOrganisationParentCompanyGuaranteeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Roatp/UpdateOrganisationParentCompanyGuarantee.cshtml", model);
            }

            model.UpdatedBy = HttpContext.User.OperatorName();

            var result = await _apiClient.UpdateOrganisationParentCompanyGuarantee(CreateUpdateParentCompanyGuaranteeRequest(model));

            if (result)
            {
                return await RefreshSearchResults();
            }

            return View("~/Views/Roatp/UpdateOrganisationParentCompanyGuarantee.cshtml", model);
        }
        
        [Route("change-financial-track-record")]
        public async Task<IActionResult> UpdateOrganisationFinancialTrackRecord()
        {
            var searchModel = _sessionService.GetSearchResults();

            var model = new UpdateOrganisationFinancialTrackRecordViewModel
            {
                FinancialTrackRecord = searchModel.SelectedResult.OrganisationData.FinancialTrackRecord,
                OrganisationId = searchModel.SelectedResult.Id,
                LegalName = searchModel.SelectedResult.LegalName
            };

            return View("~/Views/Roatp/UpdateOrganisationFinancialTrackRecord.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFinancialTrackRecord(UpdateOrganisationFinancialTrackRecordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Roatp/UpdateOrganisationFinancialTrackRecord.cshtml", model);
            }

            model.UpdatedBy = HttpContext.User.OperatorName();

            var result = await _apiClient.UpdateOrganisationFinancialTrackRecord(CreateUpdateFinancialTrackRecordRequest(model));

            if (result)
            {
                return await RefreshSearchResults();
            }

            return View("~/Views/Roatp/UpdateOrganisationFinancialTrackRecord.cshtml", model);
        }
        
        [Route("change-provider")]
        public async Task<IActionResult> UpdateOrganisationProviderType()
        {
            var searchModel = _sessionService.GetSearchResults();

            var model = new UpdateOrganisationProviderTypeViewModel
            {
                LegalName = searchModel.SelectedResult.LegalName,
                OrganisationId = searchModel.SelectedResult.Id,
                ProviderTypeId = searchModel.SelectedResult.ProviderType.Id,
                OrganisationTypeId = searchModel.SelectedResult.OrganisationType.Id
            };

            var providerTypes = await _apiClient.GetProviderTypes();
            model.ProviderTypes = providerTypes;

            var organisationTypes = new Dictionary<int, IEnumerable<OrganisationType>>();
            foreach (var providerType in providerTypes)
            {
                var organisationTypesByProvider = await _apiClient.GetOrganisationTypes(providerType.Id);
                organisationTypes.Add(providerType.Id, organisationTypesByProvider);
            }

            model.OrganisationTypesByProviderType = organisationTypes;

            return View("~/Views/Roatp/UpdateOrganisationProviderType.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProviderType(UpdateOrganisationProviderTypeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Roatp/UpdateOrganisationProviderType.cshtml", model);
            }

            model.UpdatedBy = HttpContext.User.OperatorName();

            if (model.OrganisationTypeId == 0) // if not applicable to change organisation type then use existing value
            {
                var searchModel = _sessionService.GetSearchResults();
                model.OrganisationTypeId = searchModel.SelectedResult.OrganisationType.Id;
            }

            var result = await _apiClient.UpdateOrganisationProviderType(CreateUpdateOrganisationProviderTypeRequest(model));

            if (result)
            {
                return await RefreshSearchResults();
            }

            return View("~/Views/Roatp/UpdateOrganisationProviderType.cshtml", model);
        }

        private UpdateOrganisationLegalNameRequest CreateUpdateLegalNameRequest(UpdateOrganisationLegalNameViewModel model)
        {
            return new UpdateOrganisationLegalNameRequest
            {
                LegalName = model.LegalName.ToUpper(),
                OrganisationId = model.OrganisationId,
                UpdatedBy = model.UpdatedBy
            };
        }

        private UpdateOrganisationStatusRequest CreateUpdateOrganisationStatusRequest(UpdateOrganisationStatusViewModel model)
        {
            var request = new UpdateOrganisationStatusRequest
            {
                RemovedReasonId = null,
                OrganisationStatusId = model.OrganisationStatusId,
                OrganisationId = model.OrganisationId,
                UpdatedBy = model.UpdatedBy
            };

            if (model.OrganisationStatusId == 0)
            {
                request.RemovedReasonId = model.RemovedReasonId;
            }

            return request;
        }

        private UpdateOrganisationTradingNameRequest CreateUpdateTradingNameRequest(UpdateOrganisationTradingNameViewModel model)
        {
            return new UpdateOrganisationTradingNameRequest
            {
                TradingName = model.TradingName,
                OrganisationId = model.OrganisationId,
                UpdatedBy = model.UpdatedBy
            };
        }

        private UpdateOrganisationProviderTypeRequest CreateUpdateOrganisationProviderTypeRequest(UpdateOrganisationProviderTypeViewModel model)
        {

            return new UpdateOrganisationProviderTypeRequest
            {
                OrganisationId = model.OrganisationId,
                OrganisationTypeId = model.OrganisationTypeId,
                ProviderTypeId = model.ProviderTypeId,
                UpdatedBy = model.UpdatedBy
            };
        }

        private UpdateOrganisationParentCompanyGuaranteeRequest CreateUpdateParentCompanyGuaranteeRequest(UpdateOrganisationParentCompanyGuaranteeViewModel model)
        {
            return new UpdateOrganisationParentCompanyGuaranteeRequest
            {
                ParentCompanyGuarantee = model.ParentCompanyGuarantee,
                OrganisationId = model.OrganisationId,
                UpdatedBy = model.UpdatedBy
            };
        }
        
        private UpdateOrganisationFinancialTrackRecordRequest CreateUpdateFinancialTrackRecordRequest(UpdateOrganisationFinancialTrackRecordViewModel model)
        {
            return new UpdateOrganisationFinancialTrackRecordRequest
            {
                FinancialTrackRecord = model.FinancialTrackRecord,
                OrganisationId = model.OrganisationId,
                UpdatedBy = model.UpdatedBy
            };
        }

    }
}