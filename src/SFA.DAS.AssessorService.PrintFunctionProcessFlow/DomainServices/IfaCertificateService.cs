using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.AzureStorage;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Data;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Logger;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Sftp;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.DomainServices
{
    public class IFACertificateService
    {
        private readonly InitialiseContainer _initialiseContainer;
        private readonly IAggregateLogger _aggregateLogger;
        private readonly FileTransferClient _fileTransferClient;
        private readonly CertificatesRepository _certificatesRepository;

        public IFACertificateService(
            InitialiseContainer initialiseContainer,
            IAggregateLogger aggregateLogger,
            FileTransferClient fileTransferClient,
            CertificatesRepository certificatesRepository)
        {
            _initialiseContainer = initialiseContainer;
            _aggregateLogger = aggregateLogger;
            _fileTransferClient = fileTransferClient;
            _certificatesRepository = certificatesRepository;
        }

        public async Task Create()
        {
            _aggregateLogger.LogInfo("Created Excdel Spreadsheet ....");
            var memoryStream = new MemoryStream();

            var uuid = Guid.NewGuid();
            var fileName = $"output-{uuid}.xlsx";

            using (var package = new ExcelPackage(memoryStream))
            {
                CreateWorkBook(package);
                CreateWorkSheet(package);

                package.Save();

                await _fileTransferClient.Send(memoryStream, fileName);
                await WriteCopyOfMergedDocumentToBlob(fileName, memoryStream);

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

        private void CreateWorkSheet(ExcelPackage package)
        {
            var monthYear = GetMonthYear("MMM");

            var worksheet = package.Workbook.Worksheets.Add(monthYear);

            CreateWorksheetDefaults(worksheet);
            CreateWorkbookProperties(package);

            CreateWorksheetHeader(worksheet);
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
            package.Workbook.Properties.Title = "PrintFLow Prototype";
            package.Workbook.Properties.Author = "Alan Burns";
            package.Workbook.Properties.Comments =
                "This sample demonstrates how to create an Excel 2007 workbook for Printer Output Prototype";
        }

        private static void CreateWorksheetTableHeader(ExcelWorksheet worksheet)
        {
            worksheet.Cells["K1:Q1"].Merge = true;
            worksheet.Cells["K1:Q1"].Value = "EmployerAddress Details";


            worksheet.Cells[2, 1].Value = "Achievement Date";
            worksheet.Cells[2, 2].Value = "Apprentice Name";
            worksheet.Cells[2, 3].Value = "Standard Title";
            worksheet.Cells[2, 4].Value = "Option";
            worksheet.Cells[2, 5].Value = "Level";
            worksheet.Cells[2, 6].Value = "acheiving a";
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

        private static void CreateWorksheetHeader(ExcelWorksheet worksheet)
        {
            using (var range = worksheet.Cells[1, 1, 1, 18])
            {
                range.Style.Font.Bold = true;
                //range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                //range.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                range.Style.Font.Color.SetColor(Color.Red);
                range.Style.Font.Size = 20;
            }

            var monthYear = GetMonthYear("MMMM");
            worksheet.Cells["A1:J1"].Merge = true;
            worksheet.Cells["A1:J1"].Value = monthYear + " Print Data";
        }

        private void CreateWorksheetData(ExcelWorksheet worksheet)
        {
            var certificates = _certificatesRepository.GetData().ToList();

            var row = 3;

            foreach (var certificate in certificates)
            {
                var certificateData =
                    JsonConvert.DeserializeObject<Domain.JsonData.CertificateData>(certificate.CertificateData);
                if (certificateData.AchievementDate != null)
                    worksheet.Cells[row, 1].Value = certificateData.AchievementDate.ToString();

                var learnerName = $"{certificateData.LearnerGivenNames} {certificateData.LearnerFamilyName}";
                if (certificateData.ContactName != null)
                    worksheet.Cells[row, 2].Value = learnerName;

                if (certificateData.StandardName != null)
                    worksheet.Cells[row, 3].Value = certificateData.StandardName;

                if (certificateData.CourseOption != null)
                    worksheet.Cells[row, 4].Value = certificateData.CourseOption;

                worksheet.Cells[row, 5].Value = $"Level{certificateData.StandardLevel}";

                if (certificateData.AchievementOutcome != null)
                    worksheet.Cells[row, 6].Value = certificateData.AchievementOutcome;

                if (certificateData.OverallGrade != null)
                    worksheet.Cells[row, 7].Value = certificateData.OverallGrade;

                if (certificate.CertificateReference != null)
                    worksheet.Cells[row, 8].Value = certificate.CertificateReference;

                worksheet.Cells[row, 9].Value = Environment.GetEnvironmentVariable("ChairName", EnvironmentVariableTarget.Process);
                worksheet.Cells[row, 10].Value = Environment.GetEnvironmentVariable("ChairTitle", EnvironmentVariableTarget.Process);

                if (certificateData.ContactOrganisation != null)
                    worksheet.Cells[row, 11].Value = certificateData.ContactOrganisation;

                if (certificateData.ContactName != null)
                    worksheet.Cells[row, 12].Value = certificateData.ContactName;

                if (certificateData.ContactOrganisation != null)
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

                _aggregateLogger.LogInfo($"Processing Certificate For IFA Certificate - {certificate.Id}");

                row++;
            }
        }

        private static string GetMonthYear(string monthFormat)
        {
            var month = DateTime.Today.ToString(monthFormat, new CultureInfo("en-GB"));

            var year = DateTime.Now.Year;
            var monthYear = month + " " + year;
            return monthYear;
        }

        private async Task WriteCopyOfMergedDocumentToBlob(string mergedFileName, MemoryStream memoryStream)
        {
            //var memoryStream = new MemoryStream();
            //document.SaveToStream(memoryStream, FileFormat.Docx);

            memoryStream.Position = 0;

            var containerName = "mergeddocuments";
            var container = await _initialiseContainer.Execute(containerName);

            var blob = container.GetBlockBlobReference(mergedFileName);
            blob.UploadFromStream(memoryStream);

            memoryStream.Position = 0;
            //await _fileTransferClient.Send(memoryStream, mergedFileName);
        }
    }
}
