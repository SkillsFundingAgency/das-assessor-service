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
		
        private UpdateOrganisationLegalNameRequest CreateUpdateLegalNameRequest(UpdateOrganisationLegalNameViewModel model)
        {
            return new UpdateOrganisationLegalNameRequest
            {
                LegalName = model.LegalName.ToUpper(),
                OrganisationId = model.OrganisationId,
                UpdatedBy = model.UpdatedBy
            };
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
    
    }
}