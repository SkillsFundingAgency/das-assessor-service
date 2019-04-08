namespace SFA.DAS.AssessorService.Web.Staff.Controllers.Roatp
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using OfficeOpenXml;
    using SFA.DAS.AssessorService.Web.Staff.Helpers;
    using SFA.DAS.AssessorService.Web.Staff.Infrastructure;

    [Authorize]
    public class DownloadRoatpController : Controller
    {
        private IRoatpApiClient _apiClient;
        private IDataTableHelper _dataTableHelper;
        private ILogger<DownloadRoatpController> _logger;

        private const string CompleteRegisterWorksheetName = "Providers";
        private const string AuditHistoryWorksheetName = "Provider history";
        private const string ExcelFileName = "_RegisterOfApprenticeshipTrainingProviders.xlsx";
        private const string FatFileName = "roatp {0}.xlsx";
        private const string FatWorksheetName = "roatp";

        public DownloadRoatpController(IRoatpApiClient apiClient, IDataTableHelper dataTableHelper,
            ILogger<DownloadRoatpController> logger)
        {
            _apiClient = apiClient;
            _dataTableHelper = dataTableHelper;
            _logger = logger;
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

                return File(package.GetAsByteArray(), "application/excel",
                    $"{DateTime.Now.ToString("yyyyMMdd")}{ExcelFileName}");
            }
        }

        [Route("download-fat-file")]
        public async Task<IActionResult> DownloadFatFile()
        {
            using (var package = new ExcelPackage())
            {
                var roatpSummaryWorksheet = package.Workbook.Worksheets.Add(FatWorksheetName);
                var roatpData = await _apiClient.GetRoatpSummary();
                if (roatpData != null && roatpData.Any())
                {
                    roatpSummaryWorksheet.Cells.LoadFromDataTable(_dataTableHelper.ToDataTable(roatpData), true);
                }
                else
                {
                    _logger.LogError("Unable to retrieve summary data from RoATP API");
                }

                var fatSpreadsheetFilename = string.Format(FatFileName, DateTime.Now.ToString("dd.MM.yyyy"));

                return File(package.GetAsByteArray(), "application/excel", fatSpreadsheetFilename);
            }
        }
    }
}
