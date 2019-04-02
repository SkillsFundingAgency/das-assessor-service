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




































































































































        [Route("change-ukprn")]
        public async Task<IActionResult> UpdateOrganisationUkprn()
        {
            var searchModel = _sessionService.GetSearchResults();

            var model = new UpdateOrganisationUkprnViewModel
            {
                Ukprn = searchModel.SelectedResult?.UKPRN.ToString(),
                LegalName = searchModel.SelectedResult.LegalName,
                OrganisationId = searchModel.SelectedResult.Id
            };

            return View("~/Views/Roatp/UpdateOrganisationUkprn.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUkprn(UpdateOrganisationUkprnViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Roatp/UpdateOrganisationUkprn.cshtml", model);
            }

            model.UpdatedBy = HttpContext.User.OperatorName();

            var result = await _apiClient.UpdateOrganisationUkprn(CreateUpdateUkprnRequest(model));

            if (result)
            {
                return await RefreshSearchResults();
            }

            return View("~/Views/Roatp/UpdateOrganisationUkprn.cshtml", model);
        }

        private UpdateOrganisationUkprnRequest CreateUpdateUkprnRequest(UpdateOrganisationUkprnViewModel model)
        {
            return new UpdateOrganisationUkprnRequest
            {
                Ukprn = model.Ukprn,
                OrganisationId = model.OrganisationId,
                UpdatedBy = model.UpdatedBy
            };
        }

    }
}
