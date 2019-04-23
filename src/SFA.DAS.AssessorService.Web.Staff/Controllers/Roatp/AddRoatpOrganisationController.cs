
using Microsoft.AspNetCore.Authorization;

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

    [Authorize]
    public class AddRoatpOrganisationController : Controller
    {
        private IRoatpApiClient _apiClient;
        private ILogger<AddRoatpOrganisationController> _logger;
        private IAddOrganisationValidator _validator;
        private IRoatpSessionService _sessionService;

        private const string CompleteRegisterWorksheetName = "Providers";
        private const string AuditHistoryWorksheetName = "Provider history";
        private const string ExcelFileName = "_RegisterOfApprenticeshipTrainingProviders.xlsx";

        public AddRoatpOrganisationController(IRoatpApiClient apiClient, ILogger<AddRoatpOrganisationController> logger, 
            IAddOrganisationValidator validator, IRoatpSessionService sessionService)
        {
            _apiClient = apiClient;
            _logger = logger;
            _validator = validator;
            _sessionService = sessionService;
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
                LegalName = model.LegalName.ToUpper(),
                NonLevyContract = false,
                OrganisationTypeId = model.OrganisationTypeId,
                ParentCompanyGuarantee = false,
                ProviderTypeId = model.ProviderTypeId,
                StatusDate = DateTime.Now,
                Ukprn = model.UKPRN,
                TradingName = model.TradingName,
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
