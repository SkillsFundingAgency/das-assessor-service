namespace SFA.DAS.AssessorService.Web.Staff.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Infrastructure;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using OfficeOpenXml;

    [Authorize]
    public class RoatpController : ExcelAwareController
    {
        private IRoatpApiClient _apiClient;

        private const string CompleteRegisterWorksheetName = "Providers";
        private const string AuditHistoryWorksheetName = "Provider history";

        public RoatpController(IRoatpApiClient apiClient)
        {
            _apiClient = apiClient;
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
                completeRegisterWorkSheet.Cells.LoadFromDataTable(ToDataTable(registerData), true);

                var auditHistoryWorksheet = package.Workbook.Worksheets.Add(AuditHistoryWorksheetName);
                var auditHistoryData = await _apiClient.GetAuditHistory();
                auditHistoryWorksheet.Cells.LoadFromDataTable(ToDataTable(auditHistoryData), true);

                return File(package.GetAsByteArray(), "application/excel", $"{DateTime.Now.ToString("yyyyMMdd")}_RegisterOfApprenticeshipTrainingProviders.xlsx");
            }
        }
    }
}

