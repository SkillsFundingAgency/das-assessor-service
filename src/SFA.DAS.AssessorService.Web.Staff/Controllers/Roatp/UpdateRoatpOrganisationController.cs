namespace SFA.DAS.AssessorService.Web.Staff.Controllers.Roatp
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
    using System.Threading.Tasks;
    using ViewModels.Roatp;
    using SFA.DAS.AssessorService.Web.Staff.Domain;
    using SFA.DAS.AssessorService.Api.Types.Models.Roatp;
    
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

        private UpdateOrganisationLegalNameRequest CreateUpdateLegalNameRequest(UpdateOrganisationLegalNameViewModel model)
        {
            return new UpdateOrganisationLegalNameRequest
            {
                LegalName = model.LegalName.ToUpper(),
                OrganisationId = model.OrganisationId,
                UpdatedBy = model.UpdatedBy
            };
        }









































        [Route("change-parent-company-guarantee")]
        public async Task<IActionResult> UpdateOrganisationParentCompanyGuarantee()
        {
            var searchModel = _sessionService.GetSearchResults();

            var model = new UpdateOrganisationParentCompanyGuaranteeViewModel
            {
                CurrentParentCompanyGuarantee = searchModel.SelectedResult.OrganisationData.ParentCompanyGuarantee,
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

            return View("~/Views/Roatp/UpdateOrganisationParentCompanyGuarantree.cshtml", model);
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
    }
}
