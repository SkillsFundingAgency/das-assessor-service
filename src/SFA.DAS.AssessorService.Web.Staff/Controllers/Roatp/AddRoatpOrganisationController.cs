
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.AssessorService.Application.Api.Services;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers.Roatp
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using SFA.DAS.AssessorService.Api.Types.Models.Roatp;
    using SFA.DAS.AssessorService.Web.Staff.Domain;
    using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
    using SFA.DAS.AssessorService.Web.Staff.Validators.Roatp;
    using SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp;
    using System;
    using System.Threading.Tasks;
    using Resources;
    using SFA.DAS.AssessorService.Application.Api.Client.Clients;

    [Authorize]
    public class AddRoatpOrganisationController : Controller
    {
        private IRoatpApiClient _apiClient;
        private ILogger<AddRoatpOrganisationController> _logger;
        private IAddOrganisationValidator _validator;
        private IRoatpSessionService _sessionService;
        private IUkrlpApiClient _ukrlpClient;

        private const string CompleteRegisterWorksheetName = "Providers";
        private const string AuditHistoryWorksheetName = "Provider history";
        private const string ExcelFileName = "_RegisterOfApprenticeshipTrainingProviders.xlsx";

        public AddRoatpOrganisationController(IRoatpApiClient apiClient, ILogger<AddRoatpOrganisationController> logger, 
            IAddOrganisationValidator validator, IRoatpSessionService sessionService, IUkrlpApiClient ukrlpClient)
        {
            _apiClient = apiClient;
            _logger = logger;
            _validator = validator;
            _sessionService = sessionService;
            _ukrlpClient = ukrlpClient;
        }
        

        [Route("enter-ukprn")]
        public async Task<IActionResult> EnterUkprn()
        { 
            ModelState.Clear();
            var model = new AddOrganisationProviderTypeViewModel();
            return View("~/Views/Roatp/EnterUkprn.cshtml", model);
        }

        [Route("ukprn-preview")]
        public async Task<IActionResult> UkprnPreview(AddOrganisationProviderTypeViewModel model)
        {
            if (string.IsNullOrEmpty(model.UKPRN))
            {
                Redirect("enter-ukprn");
            }

            var details = await _ukrlpClient.Get(model.UKPRN);


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



        [Route("add-provider-type")]
        public async Task<IActionResult> AddProviderType(AddOrganisationProviderTypeViewModel model)
        {
            model.ProviderTypes = await _apiClient.GetProviderTypes();
            ModelState.Clear();
            return View("~/Views/Roatp/AddProviderType.cshtml", model);
        }

        // MFCMFC this may be decommissioned
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

        [Route("add-organisation-type")]
        public async Task<IActionResult> AddOrganisationType(AddOrganisationProviderTypeViewModel model)
        {
            if (!IsRedirectFromConfirmationPage() && !ModelState.IsValid)
            {
                model.ProviderTypes = await _apiClient.GetProviderTypes();
                return View("~/Views/Roatp/AddProviderType.cshtml", model);
            }

            var addOrganisationModel = _sessionService.GetAddOrganisationDetails();
            //if (addOrganisationModel == null)
            //{
            //    addOrganisationModel = new AddOrganisationViewModel
            //    {
            //        OrganisationId = model.OrganisationId,
            //        ProviderTypeId = model.ProviderTypeId,
            //        LegalName = model.LegalName,
            //        TradingName = model.TradingName,
            //        CharityNumber = model.CharityNumber,
            //        CompanyNumber =  model.CompanyNumber
            //    };
            //}

            //if (addOrganisationModel.ProviderTypeId == 0) addOrganisationModel.ProviderTypeId = model.ProviderTypeId;
            if (string.IsNullOrEmpty(addOrganisationModel.LegalName)) addOrganisationModel.LegalName = model.LegalName;
            if (string.IsNullOrEmpty(addOrganisationModel.TradingName)) addOrganisationModel.TradingName = model.TradingName;
            if (string.IsNullOrEmpty(addOrganisationModel.CompanyNumber)) addOrganisationModel.CompanyNumber = model.CompanyNumber;
            if (string.IsNullOrEmpty(addOrganisationModel.CharityNumber)) addOrganisationModel.CharityNumber = model.CharityNumber;
            if (string.IsNullOrEmpty(addOrganisationModel.UKPRN)) addOrganisationModel.UKPRN = model.UKPRN;

            if (model.OrganisationId != Guid.Empty)
            {
                addOrganisationModel.OrganisationId = model.OrganisationId;
            }

            if (model.ProviderTypeId > 0)
            {
                addOrganisationModel.ProviderTypeId = model.ProviderTypeId;
            }

            var organisationTypes = await _apiClient.GetOrganisationTypes(addOrganisationModel.ProviderTypeId);


            addOrganisationModel.OrganisationTypes = organisationTypes.Where(x=>x.Id!=0).ToList().OrderBy(x=>x.Type);

            _sessionService.SetAddOrganisationDetails(addOrganisationModel);

            ModelState.Clear();

            var vm = MapOrganisationVMToOrganisationTypeVM(addOrganisationModel);

            return View("~/Views/Roatp/AddOrganisationType.cshtml", vm);
        }

        private static AddOrganisationTypeViewModel MapOrganisationVMToOrganisationTypeVM(AddOrganisationViewModel addOrganisationModel)
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

        [Route("confirm-organisation-details")]
        public async Task<IActionResult> ConfirmOrganisationDetails(AddOrganisationTypeViewModel model)
        {
            var organisationVm = _sessionService.GetAddOrganisationDetails();
            var vm = MapOrganisationVMToOrganisationTypeVM(organisationVm);

            if (!IsRedirectFromConfirmationPage() && !ModelState.IsValid)
            {
                //model.ProviderTypes = await _apiClient.GetProviderTypes();
                return View("~/Views/Roatp/AddOrganisationType.cshtml", vm);
            }

            vm.OrganisationTypeId = model.OrganisationTypeId;
            _sessionService.SetAddOrganisationDetails(vm);

            return View("~/Views/Roatp/ConfirmDetails.cshtml", vm);
        }

        
        //MFCMFC this may be decommissioned
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
