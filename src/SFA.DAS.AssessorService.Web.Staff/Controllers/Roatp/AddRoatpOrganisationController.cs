using System.Net.Http;
using SFA.DAS.AssessorService.Application.Api.Services;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers.Roatp
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Staff.Domain;
    using Infrastructure;
    using Validators.Roatp;
    using ViewModels.Roatp;
    using System;
    using System.Threading.Tasks;
    using Resources;
    using SFA.DAS.AssessorService.Application.Api.Client.Clients;
    using SFA.DAS.AssessorService.Api.Types.Models.Roatp;
    using SFA.DAS.AssessorService.Api.Types.Models.UKRLP;

    [Authorize]
    public class AddRoatpOrganisationController : Controller
    {
        private readonly IRoatpApiClient _apiClient;
        private readonly IRoatpSessionService _sessionService;
        private readonly IUkrlpApiClient _ukrlpClient;
        private ILogger<AddRoatpOrganisationController> _logger;

        public AddRoatpOrganisationController(IRoatpApiClient apiClient, IRoatpSessionService sessionService, 
            IUkrlpApiClient ukrlpClient, ILogger<AddRoatpOrganisationController> logger)
        {
            _apiClient = apiClient;
            _sessionService = sessionService;
            _ukrlpClient = ukrlpClient;
            _logger = logger;
        }
        

        [Route("organisations-ukprn")]
        public async Task<IActionResult> EnterUkprn()
        { 
            ModelState.Clear();
            var model = new AddOrganisationViaUkprnViewModel();
            return View("~/Views/Roatp/EnterUkprn.cshtml", model);
        }

        [Route("ukprn-not-found")]
        public async Task<IActionResult> UkprnNotFound()
        {
            ModelState.Clear();
            return View("~/Views/Roatp/UkprnNotFound.cshtml");
        }

        [Route("organisations-details")]
        public async Task<IActionResult> UkprnPreview(AddOrganisationViaUkprnViewModel model)
        {
            if (!IsRedirectFromConfirmationPage() && !ModelState.IsValid)
            {
                model.ProviderTypes = await _apiClient.GetProviderTypes();
                return View("~/Views/Roatp/EnterUkprn.cshtml", model);
            }

            UkrlpProviderDetails details;

            try
            {
                details = await _ukrlpClient.Get(model.UKPRN);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex,$"Failed to gather organisation details from ukrlp for UKPRN:[{model?.UKPRN}]");
                var notFoundModel = new UkrlpNotFoundViewModel {NextAction = "wait"};
                return RedirectToAction("UklrpIsUnavailable", notFoundModel);
            }

            if (string.IsNullOrEmpty(details.LegalName))
            {
                return Redirect("/ukprn-not-found");
            }

            var vm = new AddOrganisationProviderTypeViewModel
            {
                UKPRN = model.UKPRN,
                LegalName = details.LegalName,
                TradingName = details.TradingName,
                CompanyNumber = details.CompanyNumber,
                CharityNumber = details.CharityNumber
            };

            return View("~/Views/Roatp/UkprnPreview.cshtml", vm);
        }

        [Route("ukrlp-unavailable")]
        public async Task<IActionResult> UklrpIsUnavailable(UkrlpNotFoundViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Roatp/UkprnIsUnavailable.cshtml",model);
            }

            if (model?.NextAction == "wait" || model?.NextAction == "AddManually")
            {
                return View("~/Views/Roatp/UkprnIsUnavailable.cshtml");
            }

            return RedirectToAction("Index", "RoatpHome");
        }

        [Route("new-training-provider")]
        public async Task<IActionResult> AddOrganisation(AddOrganisationProviderTypeViewModel model)
        {
            if (model == null)
            {
                model = new AddOrganisationProviderTypeViewModel();     
            }

            model.ProviderTypes = await _apiClient.GetProviderTypes();

            ModelState.Clear();

            return View("~/Views/Roatp/AddOrganisation.cshtml", model);
        }

        [Route("enter-details")]
        public async Task<IActionResult> AddOrganisationDetails(AddOrganisationProviderTypeViewModel model)
        {
            if (!IsRedirectFromConfirmationPage() && !ModelState.IsValid)
            {
                model.ProviderTypes = await _apiClient.GetProviderTypes();
                return View("~/Views/Roatp/AddOrganisation.cshtml", model);
            }

            var addOrganisationModel = _sessionService.GetAddOrganisationDetails();
            if (addOrganisationModel == null)
            {
                addOrganisationModel = new AddOrganisationViewModel
                {
                    OrganisationId = model.OrganisationId,
                    ProviderTypeId = model.ProviderTypeId
                };
            }
            else
            {
                if (model.OrganisationId != Guid.Empty)
                {
                    addOrganisationModel.OrganisationId = model.OrganisationId;
                }

                if (model.ProviderTypeId > 0)
                {
                    addOrganisationModel.ProviderTypeId = model.ProviderTypeId;
                }
            }

            addOrganisationModel.OrganisationTypes = await _apiClient.GetOrganisationTypes(addOrganisationModel.ProviderTypeId);
            
            _sessionService.SetAddOrganisationDetails(addOrganisationModel);

            ModelState.Clear();

            return View("~/Views/Roatp/AddOrganisationDetails.cshtml", addOrganisationModel);
        }

        [Route("confirm-details")]
        public async Task<IActionResult> AddOrganisationPreview(AddOrganisationViewModel model)
        {
            model.OrganisationTypes = await _apiClient.GetOrganisationTypes(model.ProviderTypeId);
            model.ProviderTypes = await _apiClient.GetProviderTypes();
            model.LegalName = TextSanitiser.SanitiseText(model?.LegalName);
            model.TradingName = TextSanitiser.SanitiseText(model?.TradingName);
            if (!ModelState.IsValid)
            {
                model.ProviderTypes = await _apiClient.GetProviderTypes();
                return View("~/Views/Roatp/AddOrganisationDetails.cshtml", model);
            }

            model.LegalName = model.LegalName.ToUpper();
  
            _sessionService.SetAddOrganisationDetails(model);

            return View("~/Views/Roatp/AddOrganisationPreview.cshtml", model);
        }

        [Route("successfully-added")]
        public async Task<IActionResult> CreateOrganisation(AddOrganisationViewModel model)
        {
            model.LegalName = TextSanitiser.SanitiseText(model?.LegalName);
            model.TradingName = TextSanitiser.SanitiseText(model?.TradingName);

            var request = CreateAddOrganisationRequestFromModel(model);

            var success = await _apiClient.CreateOrganisation(request);

            if (!success)
            {
                return RedirectToAction("Error", "Home");
            }
            
            string bannerMessage = string.Format(RoatpConfirmationMessages.AddOrganisationConfirmation,
                                                 model.LegalName.ToUpper());

            var bannerModel = new OrganisationSearchViewModel { BannerMessage = bannerMessage };
            _sessionService.ClearAddOrganisationDetails();
            return View("~/Views/Roatp/Index.cshtml", bannerModel);
        }

        [Route("back")]
        public async Task<IActionResult> Back(string action, Guid organisationId)
        {
            var model = _sessionService.GetAddOrganisationDetails();

            return RedirectToAction(action);
        }

        private CreateOrganisationRequest CreateAddOrganisationRequestFromModel(AddOrganisationViewModel model)
        {
            var request = new CreateOrganisationRequest
            {
                CharityNumber = model.CharityNumber,
                CompanyNumber = model.CompanyNumber,
                FinancialTrackRecord = true,
                LegalName = model?.LegalName?.ToUpper(),
                NonLevyContract = false,
                OrganisationTypeId = model.OrganisationTypeId,
                ParentCompanyGuarantee = false,
                ProviderTypeId = model.ProviderTypeId,
                StatusDate = DateTime.Now,
                Ukprn = model.UKPRN,
                TradingName = model?.TradingName,
                Username = HttpContext.User.OperatorName()
            };
            return request;
        }

        private bool IsRedirectFromConfirmationPage()
        {
            var refererHeaders = ControllerContext.HttpContext.Request.Headers["Referer"];
            if (refererHeaders.Count == 0)
            {
                return false;
            }
            var referer = refererHeaders[0];

            if (referer.Contains("confirm-details"))
            {
                return true;
            }

            var request = ControllerContext.HttpContext.Request;
            if (request.Method == "GET" && request.Path.ToString().Contains("enter-details"))
            {
                return true;
            }

            return false;
        }
    }
}
