using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Web.Staff.Domain;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Web.Staff.Helpers;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{
    [Authorize(Roles = Roles.OperationsTeam + "," + Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
    public class ReportsController : Controller
    {
        private readonly ILogger<ReportsController> _logger;
        private readonly ApiClient _apiClient;
        private readonly IDataTableHelper _dataTableHelper;

        public ReportsController(ILogger<ReportsController> logger, ApiClient apiClient, IDataTableHelper dataTableHelper)
        {
            _logger = logger;
            _apiClient = apiClient;
            _dataTableHelper = dataTableHelper;
        }

        public async Task<IActionResult> Index()
        {
            var reports = await _apiClient.GetReportList();
            _apiClient.GatherAndCollateStandards();

            var vm = new ReportViewModel {Reports = reports};

            return View(vm);
        }

        public async Task<IActionResult> Run(Guid reportId)
        {
            if (!ModelState.IsValid || reportId == Guid.Empty)
            {
                return RedirectToAction("Index");
            }

            var reports = await _apiClient.GetReportList();
            var reportType = await _apiClient.GetReportTypeFromId(reportId);

            if (reportType == ReportType.Download)
                return RedirectToAction("DirectDownload", new {reportId});


            var data = await _apiClient.GetReport(reportId);
            var vm = new ReportViewModel {Reports = reports, ReportId = reportId, SelectedReportData = data};
            return View(vm);

        }

        public async Task<FileContentResult> DirectDownload(Guid reportId)
        {
            _logger.LogInformation($"Standard Collation initiated");
            await _apiClient.GatherAndCollateStandards();

            _logger.LogInformation($"Standard Collation completed");
            var reportDetails = await _apiClient.GetReportDetailsFromId(reportId);

            using (var package = new ExcelPackage())
            {

                    foreach (var ws in reportDetails.Worksheets.OrderBy(w => w.Order))
                    {
                        var worksheetToAdd = package.Workbook.Worksheets.Add(ws.Worksheet);
                        var data = await _apiClient.GetDataFromStoredProcedure(ws.StoredProcedure);
                        worksheetToAdd.Cells.LoadFromDataTable(_dataTableHelper.ToDataTable(data), true);
                    }

                    return File(package.GetAsByteArray(), "application/excel", $"{reportDetails.Name}.xlsx");
              
            }
        }
    
        public async Task<FileContentResult> Export(Guid reportId)
        {
            var data = await _apiClient.GetReport(reportId);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells.LoadFromDataTable(_dataTableHelper.ToDataTable(data), true);

                return File(package.GetAsByteArray(), "application/excel", $"report.xlsx");
            }
        }
    }
}