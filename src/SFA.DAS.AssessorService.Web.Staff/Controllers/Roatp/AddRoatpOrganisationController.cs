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
    using System.Linq;

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
                var notFoundModel = new UkrlpNotFoundViewModel {FirstEntry = "true"};
                return RedirectToAction("UklrpIsUnavailable", notFoundModel);
            }

            if (string.IsNullOrEmpty(details.LegalName))
            {
                return Redirect("/ukprn-not-found");
            }

            var vm = new AddOrganisationViewModel
            {
                UKPRN = model.UKPRN,
                LegalName = details.LegalName,
                TradingName = details.TradingName,
                CompanyNumber = details.CompanyNumber,
                CharityNumber = details.CharityNumber
            };

            _sessionService.SetAddOrganisationDetails(vm);

            return RedirectToAction("AddProviderType");
        }

        [Route("provider-type")]
        public async Task<IActionResult> AddProviderType()
        {
            var addOrganisationModel = _sessionService.GetAddOrganisationDetails();

            if (string.IsNullOrEmpty(addOrganisationModel?.LegalName))
            {
                return Redirect("organisations-details");
            }

            var model = MapOrganisationVmToProviderTypeVm(addOrganisationModel);

          
            model.ProviderTypes = await _apiClient.GetProviderTypes();
            ModelState.Clear();
            return View("~/Views/Roatp/AddProviderType.cshtml", model);
        }

        [Route("type-organisation")]
        public async Task<IActionResult> AddOrganisationType(AddOrganisationProviderTypeViewModel model)
        {
            if (!IsRedirectFromConfirmationPage() && !ModelState.IsValid)
            {
                model.ProviderTypes = await _apiClient.GetProviderTypes();
                return View("~/Views/Roatp/AddProviderType.cshtml", model);
            }

            var addOrganisationModel = _sessionService.GetAddOrganisationDetails();

            if (string.IsNullOrEmpty(addOrganisationModel?.LegalName))
            {
                return Redirect("organisations-details");
            }

            UpdateAddOrganisationModelFromProviderTypeModel(addOrganisationModel,model);

            var organisationTypes = await _apiClient.GetOrganisationTypes(addOrganisationModel.ProviderTypeId);

            addOrganisationModel.OrganisationTypes = organisationTypes.ToList().OrderBy(x => x.Type);

            _sessionService.SetAddOrganisationDetails(addOrganisationModel);

            ModelState.Clear();

            var vm = MapOrganisationVmToOrganisationTypeVm(addOrganisationModel);

            return View("~/Views/Roatp/AddOrganisationType.cshtml", vm);
        }


        [Route("confirm-details")]
        public async Task<IActionResult> ConfirmOrganisationDetails(AddOrganisationTypeViewModel model)
        {
            var organisationVm = _sessionService.GetAddOrganisationDetails();
            var vm = MapOrganisationVmToOrganisationTypeVm(organisationVm);
            if (!IsRedirectFromConfirmationPage() && !ModelState.IsValid)
            {
                return View("~/Views/Roatp/AddOrganisationType.cshtml",vm);
            }

            vm.OrganisationTypeId = model.OrganisationTypeId;
            vm.LegalName = vm.LegalName.ToUpper();
            _sessionService.SetAddOrganisationDetails(vm);

            //return View("~/Views/Roatp/ConfirmDetails.cshtml");



            model.OrganisationTypes = await _apiClient.GetOrganisationTypes(vm.ProviderTypeId);
            model.ProviderTypes = await _apiClient.GetProviderTypes();
            model.ProviderTypeId = vm.ProviderTypeId;
            model.LegalName = TextSanitiser.SanitiseText(vm.LegalName).ToUpper();
            model.TradingName = TextSanitiser.SanitiseText(vm.TradingName);
            model.UKPRN = vm.UKPRN;

            return View("~/Views/Roatp/AddOrganisationPreview.cshtml", model);
        }

    private static void UpdateAddOrganisationModelFromProviderTypeModel(AddOrganisationViewModel addOrganisationModel, AddOrganisationProviderTypeViewModel model)
        {
            if (string.IsNullOrEmpty(addOrganisationModel.LegalName)) addOrganisationModel.LegalName = model.LegalName;
            if (string.IsNullOrEmpty(addOrganisationModel.TradingName)) addOrganisationModel.TradingName = model.TradingName;
            if (string.IsNullOrEmpty(addOrganisationModel.CompanyNumber))
                addOrganisationModel.CompanyNumber = model.CompanyNumber;
            if (string.IsNullOrEmpty(addOrganisationModel.CharityNumber))
                addOrganisationModel.CharityNumber = model.CharityNumber;
            if (string.IsNullOrEmpty(addOrganisationModel.UKPRN)) addOrganisationModel.UKPRN = model.UKPRN;

            if (model.OrganisationId != Guid.Empty)
            {
                addOrganisationModel.OrganisationId = model.OrganisationId;
            }

            if (model.ProviderTypeId > 0)
            {
                addOrganisationModel.ProviderTypeId = model.ProviderTypeId;
            }
        }

        private static AddOrganisationTypeViewModel MapOrganisationVmToOrganisationTypeVm(AddOrganisationViewModel addOrganisationModel)
        {
            return new AddOrganisationTypeViewModel
            {
                CharityNumber = addOrganisationModel.CharityNumber,
                CompanyNumber = addOrganisationModel.CompanyNumber,
                LegalName = addOrganisationModel.LegalName,
                OrganisationId = addOrganisationModel.OrganisationId,
                OrganisationTypeId = addOrganisationModel.OrganisationTypeId,
                OrganisationTypes = addOrganisationModel.OrganisationTypes,
                ProviderTypeId = addOrganisationModel.ProviderTypeId,
                ProviderTypes = addOrganisationModel.ProviderTypes,
                TradingName = addOrganisationModel.TradingName,
                UKPRN = addOrganisationModel.UKPRN
            };
        }


        private static AddOrganisationProviderTypeViewModel MapOrganisationVmToProviderTypeVm(AddOrganisationViewModel addOrganisationModel)
        {
            return new AddOrganisationProviderTypeViewModel
            {
                CharityNumber = addOrganisationModel.CharityNumber,
                CompanyNumber = addOrganisationModel.CompanyNumber,
                LegalName = addOrganisationModel.LegalName,
                OrganisationId = addOrganisationModel.OrganisationId,
                OrganisationTypeId = addOrganisationModel.OrganisationTypeId,
                OrganisationTypes = addOrganisationModel.OrganisationTypes,
                ProviderTypeId = addOrganisationModel.ProviderTypeId,
                ProviderTypes = addOrganisationModel.ProviderTypes,
                TradingName = addOrganisationModel.TradingName,
                UKPRN = addOrganisationModel.UKPRN
            };
        }




        [Route("ukrlp-not-found")]
        public async Task<IActionResult> UklrpIsUnavailable(UkrlpNotFoundViewModel model)
        {
   
            if (!ModelState.IsValid || !string.IsNullOrEmpty(model?.FirstEntry))
            {
                return View("~/Views/Roatp/UkprnIsUnavailable.cshtml", model);
            }

            if (model?.NextAction == "AddManually")
            {
                var notFoundModel = new UkrlpNotFoundViewModel { FirstEntry = "true" };
                return RedirectToAction("UklrpIsUnavailable", notFoundModel);
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

        // TODO: REMOVEREMOVE once established there is no totally manual entry process
        //[Route("confirm-details-original")]
        //public async Task<IActionResult> AddOrganisationPreview(AddOrganisationViewModel model)
        //{
        //    model.OrganisationTypes = await _apiClient.GetOrganisationTypes(model.ProviderTypeId);
        //    model.ProviderTypes = await _apiClient.GetProviderTypes();
        //    model.LegalName = TextSanitiser.SanitiseText(model?.LegalName);
        //    model.TradingName = TextSanitiser.SanitiseText(model?.TradingName);
        //    if (!ModelState.IsValid)
        //    {
        //        model.ProviderTypes = await _apiClient.GetProviderTypes();
        //        return View("~/Views/Roatp/AddOrganisationDetails.cshtml", model);
        //    }

        //    model.LegalName = model.LegalName.ToUpper();
  
        //    _sessionService.SetAddOrganisationDetails(model);

        //    return View("~/Views/Roatp/AddOrganisationPreview.cshtml", model);
        //}

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
