using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Web.Staff.Domain;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{
    [Authorize(Roles = Roles.OperationsTeam + "," + Roles.CertificationTeam + "," + Roles.AssessmentDeliveryTeam)]
    public class ReportsController : Controller
    {
        private readonly ILogger<ReportsController> _logger;
        private readonly ApiClient _apiClient;

        public ReportsController(ILogger<ReportsController> logger, ApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task<IActionResult> Index()
        {
            var reports = await _apiClient.GetReportList();

            var vm = new ReportViewModel {Reports = reports};

            return View(vm);
        }

        public async Task<IActionResult> Run(Guid reportId)
        {
            if (!ModelState.IsValid || reportId == Guid.Empty)
            {
                return RedirectToAction("Index");
            }
            else
            {
                var reports = await _apiClient.GetReportList();
                var reportType = await _apiClient.GetReportTypeFromId(reportId);
                if (reportType == ReportType.ViewOnScreen)
                {
                    var data = await _apiClient.GetReport(reportId);
                    var vm = new ReportViewModel {Reports = reports, ReportId = reportId, SelectedReportData = data};
                    return View(vm);
                }
                var certCount = new Guid("0B188AC5-0A59-4B16-B63B-1031CC985728");
                //var fileToDownload = await Export(certCount);
                return RedirectToAction("DirectDownload", new {reportId = reportId});
            }
        }

        public async Task<FileContentResult> DirectDownload(Guid reportId)
        {
            //var data = await _apiClient.GetReport(reportId);

            var reportDetails = await _apiClient.GetReportDetailsFromId(reportId);
           // ValueTask reportDetails = 
            using (var package = new ExcelPackage())
            {
                foreach (var ws in reportDetails.Worksheets.OrderBy(w => w.Order))
                {
                    var worksheetToAdd = package.Workbook.Worksheets.Add(ws.Worksheet);
                    var data = await _apiClient.GetDataFromStoredProcedure(ws.StoredProcedure);
                    worksheetToAdd.Cells.LoadFromDataTable(ToDataTable(data), true);
                }
                if (reportDetails.Type=="excel")
                    return File(package.GetAsByteArray(), "application/excel", $"{reportDetails.Name}.xlsx");
                else
                    return null;

            }
        }

        public async Task<FileContentResult> Export(Guid reportId)
        {
            var data = await _apiClient.GetReport(reportId);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells.LoadFromDataTable(ToDataTable(data), true);

                return File(package.GetAsByteArray(), "application/excel", $"report.xlsx");
            }
        }

        private static DataTable ToDataTable(IEnumerable<IDictionary<string, object>> list)
        {
            var dataTable = new DataTable();

            if (list != null || list.Any())
            {
                var columnNames = list.SelectMany(dict => dict.Keys).Distinct();
                dataTable.Columns.AddRange(columnNames.Select(col => new DataColumn(col)).ToArray());

                foreach (var item in list)
                {
                    var row = dataTable.NewRow();
                    foreach (var key in item.Keys)
                    {
                        row[key] = item[key];
                    }

                    dataTable.Rows.Add(row);
                }
            }

            return dataTable;
        }
    }
}