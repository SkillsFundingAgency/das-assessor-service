namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Infrastructure;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore.Internal;
    using Microsoft.Extensions.Logging;
    using OfficeOpenXml;

    [Authorize]
    public class RoatpController : Controller
    {
        private IRoatpApiClient _apiClient;
        private IDataTableHelper _dataTableHelper;
        private ILogger<RoatpController> _logger;
    
        private const string CompleteRegisterWorksheetName = "Providers";
        private const string AuditHistoryWorksheetName = "Provider history";
        private const string ExcelFileName = "_RegisterOfApprenticeshipTrainingProviders.xlsx";
        
        public RoatpController(IRoatpApiClient apiClient, IDataTableHelper dataTableHelper, ILogger<RoatpController> logger)
        {
            _apiClient = apiClient;
            _dataTableHelper = dataTableHelper;
            _logger = logger;
        } 
        
        public IActionResult Index()
        {
            return View();
        }

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
    }
}

