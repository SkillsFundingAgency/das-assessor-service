using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.Consts;
using SFA.DAS.AssessorService.Domain.Extensions;
using SFA.DAS.AssessorService.EpaoImporter.Interfaces;
using SFA.DAS.AssessorService.EpaoImporter.Logger;
using SFA.DAS.AssessorService.EpaoImporter.Sftp;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.EpaoImporter.DomainServices
{
    public class PrintingSpreadsheetCreator : IPrintingSpreadsheetCreator
    {
        private readonly IAggregateLogger _aggregateLogger;
        private readonly IFileTransferClient _fileTransferClient;
        private readonly IWebConfiguration _webConfiguration;
        private IEnumerable<CertificateResponse> _certificates;

        public PrintingSpreadsheetCreator(
            IAggregateLogger aggregateLogger,
            IFileTransferClient fileTransferClient,
            IWebConfiguration webConfiguration)
        {
            _aggregateLogger = aggregateLogger;
            _fileTransferClient = fileTransferClient;
            _webConfiguration = webConfiguration;
        }

        public void Create(int batchNumber, IEnumerable<CertificateResponse> certificates)
        {
            _aggregateLogger.LogInfo("Creating Excel Spreadsheet ....");

            var memoryStream = new MemoryStream();

            var certificateResponses = certificates as CertificateResponse[] ?? certificates.ToArray();
            _certificates = certificateResponses;

            var utcNow = DateTime.UtcNow;
            var gmtNow = utcNow.UtcToTimeZoneTime(TimezoneNames.GmtStandardTimeZone);
            var fileName = $"IFA-Certificate-{gmtNow:MMyy}-{batchNumber.ToString().PadLeft(3, '0')}.xlsx";

            using (var package = new ExcelPackage(memoryStream))
            {
                CreateWorkBook(package);
                CreateWorkSheet(batchNumber, package, certificateResponses);

                package.Save();

                _fileTransferClient.Send(memoryStream, fileName);

                memoryStream.Close();
            }

            _aggregateLogger.LogInfo("Completed Excel Spreadsheet ....");
        }

        private static void CreateWorkBook(ExcelPackage package)
        {
            var workbook = package.Workbook;
            workbook.Protection.LockWindows = true;
            workbook.Protection.LockStructure = true;
            workbook.View.ShowHorizontalScrollBar = true;
            workbook.View.ShowVerticalScrollBar = true;
            workbook.View.ShowSheetTabs = true;
        }

        private void CreateWorkSheet(int batchNumber, ExcelPackage package,
            IEnumerable<CertificateResponse> certificates)
        {
            var utcNow = DateTime.UtcNow;
            var gmtNow = utcNow.UtcToTimeZoneTime(TimezoneNames.GmtStandardTimeZone);

            var monthYear = gmtNow.ToString("MMM yyyy");
            var worksheet = package.Workbook.Worksheets.Add(monthYear);

            CreateWorksheetDefaults(worksheet);
            CreateWorkbookProperties(package);

            CreateWorksheetHeader(batchNumber, worksheet);
            CreateWorksheetTableHeader(worksheet);

            CreateWorksheetData(worksheet);

            ResetColumnWidth(worksheet);
        }

        private static void ResetColumnWidth(ExcelWorksheet worksheet)
        {
            worksheet.Cells.AutoFitColumns(0); //Autofit columns for all cell   
        }

        private static void CreateWorksheetDefaults(ExcelWorksheet worksheet)
        {
            worksheet.Cells.Style.Font.Name = "Calibri";
            worksheet.View.PageLayoutView = false;
        }

        private static void CreateWorkbookProperties(ExcelPackage package)
        {
            package.Workbook.Properties.Title = "PrintFlow Prototype";
            package.Workbook.Properties.Author = "SFA";
            package.Workbook.Properties.Comments =
                "Printed Certificates information";
        }

        private static void CreateWorksheetTableHeader(ExcelWorksheet worksheet)
        {
            worksheet.Cells["K1:Q1"].Merge = true;
            worksheet.Cells["K1:Q1"].Value = "Employer Address Details";


            worksheet.Cells[2, 1].Value = "Achievement Date";
            worksheet.Cells[2, 2].Value = "Apprentice Name";
            worksheet.Cells[2, 3].Value = "Standard Title";
            worksheet.Cells[2, 4].Value = "Option";
            worksheet.Cells[2, 5].Value = "Level";
            worksheet.Cells[2, 6].Value = "achieving a";
            worksheet.Cells[2, 7].Value = "Grade";
            worksheet.Cells[2, 8].Value = "Certificate Number";
            worksheet.Cells[2, 9].Value = "Chair Name";
            worksheet.Cells[2, 10].Value = "Chair Title";
            worksheet.Cells[2, 11].Value = "Employer Contact";
            worksheet.Cells[2, 12].Value = "Employer Name";
            worksheet.Cells[2, 13].Value = "Department";
            worksheet.Cells[2, 14].Value = "Address Line 1";
            worksheet.Cells[2, 15].Value = "Address Line 2";
            worksheet.Cells[2, 16].Value = "Address Line 3";
            worksheet.Cells[2, 17].Value = "Address Line 4";
            worksheet.Cells[2, 18].Value = "Post Code";

            using (var range = worksheet.Cells[2, 1, 2, 18])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                range.Style.Font.Color.SetColor(Color.White);
                range.Style.Font.Size = 16;
            }
        }

        private static void CreateWorksheetHeader(int batchNumber, ExcelWorksheet worksheet)
        {
            using (var range = worksheet.Cells[1, 1, 1, 18])
            {
                range.Style.Font.Bold = true;
                range.Style.Font.Color.SetColor(Color.Red);
                range.Style.Font.Size = 20;
            }

            var monthYear = DateTime.Today.ToString("MMMM yyyy");

            worksheet.Cells["A1:J1"].Merge = true;
            worksheet.Cells["A1:J1"].Value = monthYear + " Print Data - Batch " + batchNumber.ToString();
        }

        private void CreateWorksheetData(ExcelWorksheet worksheet)
        {
            var row = 3;

            foreach (var certificate in _certificates)
            {
                var certificateData = certificate.CertificateData;
                if (certificateData.AchievementDate.HasValue)
                    worksheet.Cells[row, 1].Value = certificateData.AchievementDate.Value.ToString("dd MMMM yyyy");

                var learnerName = $"{certificateData.LearnerGivenNames} {certificateData.LearnerFamilyName}";
                var resultName = String.Empty;
                if (!string.IsNullOrEmpty(certificateData.FullName))
                {
                    resultName = (certificateData.FullName.ToLower());
                }
                else
                {
                    resultName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(learnerName.ToLower());
                }

                worksheet.Cells[row, 2].Value = resultName;

                if (certificateData.StandardName != null)
                    worksheet.Cells[row, 3].Value = certificateData.StandardName.ToUpper();

                if (certificateData.CourseOption != null)
                    worksheet.Cells[row, 4].Value = "(" + certificateData.CourseOption.ToUpper() + "):";

                worksheet.Cells[row, 5].Value = $"Level {certificateData.StandardLevel}".ToUpper();

                if (certificateData.OverallGrade != null &&
                    !certificateData.OverallGrade.ToLower().Contains("no grade awarded"))
                    worksheet.Cells[row, 6].Value = "Achieved grade ";

                if (certificateData.OverallGrade != null &&
                    !certificateData.OverallGrade.ToLower().Contains("no grade awarded"))
                    worksheet.Cells[row, 7].Value = certificateData.OverallGrade.ToUpper();

                if (certificate.CertificateReference != null)
                    worksheet.Cells[row, 8].Value = certificate.CertificateReference.PadLeft(8, '0');

                worksheet.Cells[row, 9].Value = _webConfiguration.CertificateDetails.ChairName;
                worksheet.Cells[row, 10].Value = _webConfiguration.CertificateDetails.ChairTitle;

                if (certificateData.ContactName != null)
                    worksheet.Cells[row, 11].Value = certificateData.ContactName.Replace("\t", " ");

                if (certificateData.ContactOrganisation != null)
                    worksheet.Cells[row, 12].Value = certificateData.ContactOrganisation;

                if (certificateData.Department != null)
                    worksheet.Cells[row, 13].Value = certificateData.Department;

                if (certificateData.ContactAddLine1 != null)
                    worksheet.Cells[row, 14].Value = certificateData.ContactAddLine1;

                if (certificateData.ContactAddLine2 != null)
                    worksheet.Cells[row, 15].Value = certificateData.ContactAddLine2;

                if (certificateData.ContactAddLine3 != null)
                    worksheet.Cells[row, 16].Value = certificateData.ContactAddLine3;

                if (certificateData.ContactAddLine4 != null)
                    worksheet.Cells[row, 17].Value = certificateData.ContactAddLine4;

                if (certificateData.ContactPostCode != null)
                    worksheet.Cells[row, 18].Value = certificateData.ContactPostCode;

                _aggregateLogger.LogInfo(
                    $"Processing Certificate For IFA Certificate - {certificate.CertificateReference}");

                row++;
            }
        }
    }
}