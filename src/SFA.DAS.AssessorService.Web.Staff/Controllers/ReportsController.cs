using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using SFA.DAS.AssessorService.Web.Staff.Infrastructure;
using SFA.DAS.AssessorService.Web.Staff.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Web.Staff.Domain;
using SFA.DAS.AssessorService.Api.Types.Models.AO;

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
                if (reportDetails.Type == "excel")
                {
                    foreach (var ws in reportDetails.Worksheets.OrderBy(w => w.Order))
                    {
                        var worksheetToAdd = package.Workbook.Worksheets.Add(ws.Worksheet);
                        var data = await _apiClient.GetDataFromStoredProcedure(ws.StoredProcedure);
                        worksheetToAdd.Cells.LoadFromDataTable(ToDataTable(data), true);
                    }

                    return File(package.GetAsByteArray(), "application/excel", $"{reportDetails.Name}.xlsx");
                }

                var ws1 = reportDetails.Worksheets.First();

                //var worksheetToAdd1 = package.Workbook.Worksheets.Add("x");
                var data1 = await _apiClient.GetDataFromStoredProcedure(ws1.StoredProcedure);
                //worksheetToAdd1.Cells.LoadFromDataTable(ToDataTable(data1), true);
                var dataTable = ToDataTable(data1);

                //var sw = new StringWriter();
                //var writer = new CsvWriter(sw);

                var csv = ToCsv(dataTable);

               // writer.WriteRecords(rows);
                var bytes = Encoding.UTF8.GetBytes(csv);
                //return File(bytes, "application/octet-stream", "text/csv");
                return File(bytes, "text/csv", $"{reportDetails.Name}.csv");
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


        public static string ToCsv(DataTable dataTable)
        {
            var listOfCharactersThatNeedQuoting = "\",";
            
            var sbData = new StringBuilder();
            if (dataTable.Columns.Count == 0)
                return null;

            foreach (var col in dataTable.Columns)
            {
                if (col == null)
                    sbData.Append(",");
                else
                {
                    var detailToAppend = col.ToString().Replace("\"", "\"\"");
                    if (detailToAppend.IndexOfAny(listOfCharactersThatNeedQuoting.ToCharArray()) != -1)
                        detailToAppend = $"\"{detailToAppend}\"";

                    sbData.Append($"{detailToAppend},");
                }
            }

            sbData.Replace(",", System.Environment.NewLine, sbData.Length - 1, 1);

            foreach (DataRow dr in dataTable.Rows)
            {
                foreach (var column in dr.ItemArray)
                {
                    if (column == null)
                        sbData.Append(",");
                    else
                    {
                        var detailToAppend = column.ToString().Replace("\"", "\"\"");

                        if (detailToAppend.IndexOfAny(listOfCharactersThatNeedQuoting.ToCharArray()) != -1)
                            detailToAppend = $"\"{detailToAppend}\"";

                        sbData.Append($"{detailToAppend},");
                    }
                }
            
                sbData.Replace(",", System.Environment.NewLine, sbData.Length - 1, 1);
            }

            return sbData.ToString();
        }
    }
}