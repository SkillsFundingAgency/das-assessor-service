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

    [Authorize]
    public class AddRoatpOrganisationController : Controller
    {
        private readonly IRoatpApiClient _apiClient;
        private readonly IRoatpSessionService _sessionService;
        private readonly IUkrlpApiClient _ukrlpClient;

        public AddRoatpOrganisationController(IRoatpApiClient apiClient, IRoatpSessionService sessionService, IUkrlpApiClient ukrlpClient)
        {
            _apiClient = apiClient;
            _sessionService = sessionService;
            _ukrlpClient = ukrlpClient;
        }
        

        [Route("enter-ukprn")]
        public async Task<IActionResult> EnterUkprn()
        { 
            ModelState.Clear();
            var model = new AddOrganisatioViaUkprnViewModel();
            return View("~/Views/Roatp/EnterUkprn.cshtml", model);
        }

        [Route("ukprn-not-found")]
        public async Task<IActionResult> UkprnNotFound()
        {
            ModelState.Clear();
            return View("~/Views/Roatp/UkprnNotFound.cshtml");
        }

        [Route("ukprn-preview")]
        public async Task<IActionResult> UkprnPreview(AddOrganisatioViaUkprnViewModel model)
        {
            if (!IsRedirectFromConfirmationPage() && !ModelState.IsValid)
            {
                model.ProviderTypes = await _apiClient.GetProviderTypes();
                return View("~/Views/Roatp/EnterUkprn.cshtml", model);
            }
            var details = await _ukrlpClient.Get(model.UKPRN);


            if (string.IsNullOrEmpty(details.LegalName))
            {
                return View("~/Views/Roatp/UkprnNotFound.cshtml");
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

            return false;
        }
    }
}
