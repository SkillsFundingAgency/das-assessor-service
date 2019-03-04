
namespace SFA.DAS.AssessorService.Web.Staff.Controllers.Roatp
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using SFA.DAS.AssessorService.Api.Types.Models.Roatp;
    using SFA.DAS.AssessorService.Web.Staff.Domain;
    using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
    using SFA.DAS.AssessorService.Web.Staff.Validators.Roatp;
    using SFA.DAS.AssessorService.Web.Staff.ViewModels.Roatp;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

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
        public async Task<IActionResult> AddOrganisation(AddOrganisationViewModel model)
        {
            if (model == null)
            {
                model = new AddOrganisationViewModel();
            }

            model.ProviderTypes = await _apiClient.GetProviderTypes();

            _sessionService.SetAddOrganisationDetails(model);

            return View("~/Views/Roatp/AddOrganisation.cshtml", model);
        }

        [Route("enter-details")]
        public async Task<IActionResult> AddOrganisationDetails(AddOrganisationViewModel model)
        {
            var validationMessages = _validator.ValidateProviderType(model.ProviderTypeId);
            if (validationMessages.Any())
            {
                model.ValidationErrors = new List<string>();
                model.ValidationErrors.AddRange(validationMessages);
                model.ProviderTypes = await _apiClient.GetProviderTypes();
                return View("~/Views/Roatp/AddOrganisation.cshtml", model);
            }

            model.OrganisationTypes = await _apiClient.GetOrganisationTypes(model.ProviderTypeId);

            var sessionModel = _sessionService.GetAddOrganisationDetails(model.OrganisationId);
            if (sessionModel.ProviderTypeId != model.ProviderTypeId)
            {
                sessionModel.OrganisationTypeId = 0;
            }

            _sessionService.SetAddOrganisationDetails(model);

            return View("~/Views/Roatp/AddOrganisationDetails.cshtml", model);
        }

        [Route("confirm-details")]
        public async Task<IActionResult> AddOrganisationPreview(AddOrganisationViewModel model)
        {
            model.OrganisationTypes = await _apiClient.GetOrganisationTypes(model.ProviderTypeId);
            model.ProviderTypes = await _apiClient.GetProviderTypes();

            var validationMessages = _validator.ValidateOrganisationDetails(model);
            if (validationMessages.Any())
            {
                model.ValidationErrors = new List<string>();
                model.ValidationErrors.AddRange(validationMessages);
                model.ProviderTypes = await _apiClient.GetProviderTypes();
                return View("~/Views/Roatp/AddOrganisationDetails.cshtml", model);
            }

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
                model.OrganisationTypes = await _apiClient.GetOrganisationTypes(model.ProviderTypeId);
                model.ProviderTypes = await _apiClient.GetProviderTypes();
                model.ValidationErrors = new List<string>
                {
                    $"An error occurred when adding the organisation '{model.LegalName}'.<br/> Please try again later."
                };
                return View("~/Views/Roatp/AddOrganisationPreview.cshtml", model);
            }

            var bannerModel = new BannerViewModel { CreateOrganisationCompanyName = model.LegalName };

            return View("~/Views/Roatp/Index.cshtml", bannerModel);
        }

        [Route("back")]
        public async Task<IActionResult> Back(string action, Guid organisationId)
        {
            var model = _sessionService.GetAddOrganisationDetails(organisationId);

            return RedirectToAction(action, model);
        }

        private CreateOrganisationRequest CreateAddOrganisationRequestFromModel(AddOrganisationViewModel model)
        {
            var request = new CreateOrganisationRequest
            {
                Username = HttpContext.User.OperatorName(),
                Organisation = CreateOrganisationFromModel(model)
            };
            return request;
        }

        private Organisation CreateOrganisationFromModel(AddOrganisationViewModel model)
        {
            var organisation = new Organisation
            {
                Id = Guid.NewGuid(),
                LegalName = model.LegalName,
                TradingName = model.TradingName,
                OrganisationData = new OrganisationData
                {
                    CharityNumber = model.CharityNumber,
                    CompanyNumber = model.CompanyNumber,
                    FinancialTrackRecord = true,
                    NonLevyContract = false,
                    ParentCompanyGuarantee = false
                },
                UKPRN = Convert.ToInt64(model.UKPRN),
                OrganisationStatus = new OrganisationStatus { Id = 1 }, // Active
                StatusDate = DateTime.Now,
                OrganisationType = new OrganisationType { Id = model.OrganisationTypeId },
                ProviderType = new ProviderType { Id = model.ProviderTypeId }
            };

            return organisation;
        }
    }
}
