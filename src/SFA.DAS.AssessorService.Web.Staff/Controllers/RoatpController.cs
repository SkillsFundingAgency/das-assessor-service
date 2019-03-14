namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{
    using Domain;
    using Helpers;
    using Infrastructure;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore.Internal;
    using Microsoft.Extensions.Logging;
    using OfficeOpenXml;
    using SFA.DAS.AssessorService.Api.Types.Models.Roatp;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Validators.Roatp;
    using ViewModels.Roatp;

    [Authorize]
    public class RoatpController : Controller
    {
        private IRoatpApiClient _apiClient;
        private IDataTableHelper _dataTableHelper;
        private ILogger<RoatpController> _logger;
        private IAddOrganisationValidator _validator;
        private IRoatpSessionService _sessionService;

        private const string CompleteRegisterWorksheetName = "Providers";
        private const string AuditHistoryWorksheetName = "Provider history";
        private const string ExcelFileName = "_RegisterOfApprenticeshipTrainingProviders.xlsx";
        
        public RoatpController(IRoatpApiClient apiClient, IDataTableHelper dataTableHelper, 
                               ILogger<RoatpController> logger, IAddOrganisationValidator validator,
                               IRoatpSessionService sessionService)
        {
            _apiClient = apiClient;
            _dataTableHelper = dataTableHelper;
            _logger = logger;
            _validator = validator;
            _sessionService = sessionService;
        } 
        
        [Route("manage-apprenticeship-training-providers")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("download-register")]
        public async Task<IActionResult> Download()
        {
            using (var package = new ExcelPackage())
            {
                var completeRegisterWorkSheet = package.Workbook.Worksheets.Add(CompleteRegisterWorksheetName);
                var registerData = await _apiClient.GetCompleteRegister();
                if (registerData != null && registerData.Any())
                {
                    completeRegisterWorkSheet.Cells.LoadFromDataTable(_dataTableHelper.ToDataTable(registerData), true);
                }
                else
                {
                    _logger.LogError("Unable to retrieve register data from RoATP API");
                }

                var auditHistoryWorksheet = package.Workbook.Worksheets.Add(AuditHistoryWorksheetName);
                var auditHistoryData = await _apiClient.GetAuditHistory();
                if (auditHistoryData != null && auditHistoryData.Any())
                {
                    auditHistoryWorksheet.Cells.LoadFromDataTable(_dataTableHelper.ToDataTable(auditHistoryData), true);
                }
                else
                {
                    _logger.LogError("Unable to retrieve audit history data from RoATP API");
                }

                return File(package.GetAsByteArray(), "application/excel", $"{DateTime.Now.ToString("yyyyMMdd")}{ExcelFileName}");
            }
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

            return View(model);
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
                return View("AddOrganisation", model);
            }

            model.OrganisationTypes = await _apiClient.GetOrganisationTypes(model.ProviderTypeId);

            var sessionModel = _sessionService.GetAddOrganisationDetails(model.OrganisationId);
            if (sessionModel.ProviderTypeId != model.ProviderTypeId)
            {
                sessionModel.OrganisationTypeId = 0;
            }

            _sessionService.SetAddOrganisationDetails(model);

            return View(model);
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
                return View("AddOrganisationDetails", model);
            }

            _sessionService.SetAddOrganisationDetails(model);

            return View(model);
        }

        [Route("successfully-added")]
        public async Task<IActionResult> CreateOrganisation(AddOrganisationViewModel model)
        {
            var request = CreateAddOrganisationRequestFromModel(model);

            await _apiClient.CreateOrganisation(request);

            var bannerModel = new BannerViewModel {CreateOrganisationCompanyName = model.LegalName};

            return View("Index", bannerModel);
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

