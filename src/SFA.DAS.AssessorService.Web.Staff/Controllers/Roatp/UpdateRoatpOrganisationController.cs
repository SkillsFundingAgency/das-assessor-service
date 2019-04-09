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

    //MFCMFC
    //[Authorize]
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
            const int OrganisationStatusIdRemoved = 0;
            var searchModel = _sessionService.GetSearchResults();

            var organisationStatuses = _apiClient.GetOrganisationStatuses(searchModel.SelectedResult?.ProviderType?.Id).Result.OrderBy(x => x.Status);
            var removedReasons = _apiClient.GetRemovedReasons().Result.OrderBy(x => x.Id);
            
            var model = new UpdateOrganisationStatusViewModel
            {
                LegalName = searchModel.SelectedResult.LegalName,
                OrganisationId = searchModel.SelectedResult.Id,
                OrganisationStatusId = searchModel.SelectedResult.OrganisationStatus.Id,
                OrganisationStatuses = organisationStatuses,
                RemovedReasons = removedReasons,
                ProviderTypeId = searchModel.SelectedResult.ProviderType.Id
            };
            if (model.OrganisationStatusId == OrganisationStatusIdRemoved)
            {
                model.RemovedReasonId = searchModel.SelectedResult.OrganisationData.RemovedReason.Id;
            }
            return View("~/Views/Roatp/UpdateOrganisationStatus.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(UpdateOrganisationStatusViewModel model)
        {
            model.OrganisationStatuses = _apiClient.GetOrganisationStatuses(model.ProviderTypeId).Result.OrderBy(x => x.Status);
            model.RemovedReasons = _apiClient.GetRemovedReasons().Result.OrderBy(x => x.Id);

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
    }
}
