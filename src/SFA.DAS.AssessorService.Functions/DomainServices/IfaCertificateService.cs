﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Functions.InfrastructureServices;
using SFA.DAS.AssessorService.Functions.Logger;
using SFA.DAS.AssessorService.Functions.Settings;
using SFA.DAS.AssessorService.Functions.Sftp;

namespace SFA.DAS.AssessorService.Functions.DomainServices
{
    public class IFACertificateService
    {
        private readonly BlobContainerHelper _initialiseContainer;
        private readonly IAggregateLogger _aggregateLogger;
        private readonly FileTransferClient _fileTransferClient;
        private readonly IWebConfiguration _webConfiguration;
        private IEnumerable<CertificateResponse> _certificates;

        public IFACertificateService(
            BlobContainerHelper initialiseContainer,
            IAggregateLogger aggregateLogger,
            FileTransferClient fileTransferClient,
            IWebConfiguration webConfiguration)
        {
            _initialiseContainer = initialiseContainer;
            _aggregateLogger = aggregateLogger;
            _fileTransferClient = fileTransferClient;
            _webConfiguration = webConfiguration;
        }

        public async Task Create(int batchNumber, IEnumerable<CertificateResponse> certificates)
        {
            _aggregateLogger.LogInfo("Created Excel Spreadsheet ....");

            var memoryStream = new MemoryStream();

            var certificateResponses = certificates as CertificateResponse[] ?? certificates.ToArray();
            _certificates = certificateResponses;

            var uuid = Guid.NewGuid();
            var fileName = $"IFA-Certificate-{GetMonthYear()}-{batchNumber.ToString().PadLeft(9, '0')}.xlsx";

            using (var package = new ExcelPackage(memoryStream))
            {
                CreateWorkBook(package);
                CreateWorkSheet(package, certificateResponses);

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

        private void CreateWorkSheet(ExcelPackage package,
            IEnumerable<CertificateResponse> certificates)
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
            var row = 3;

            foreach (var certificate in _certificates)
            {
                var certificateData = certificate.CertificateData;
                if (certificateData.AchievementDate != null)
                    worksheet.Cells[row, 1].Value = certificateData.AchievementDate.ToString();

                var learnerName = $"{certificateData.LearnerGivenNames} {certificateData.LearnerFamilyName}";
                if (certificateData.ContactName != null)
                    worksheet.Cells[row, 2].Value = learnerName.ToUpper();

                if (certificateData.StandardName != null)
                    worksheet.Cells[row, 3].Value = certificateData.StandardName.ToUpper();

                if (certificateData.CourseOption != null)
                    worksheet.Cells[row, 4].Value = certificateData.CourseOption.ToUpper();

                worksheet.Cells[row, 5].Value = $"Level{certificateData.StandardLevel}".ToUpper();

                if (certificateData.OverallGrade != null)
                    worksheet.Cells[row, 6].Value = "achieving a " + certificateData.OverallGrade.ToUpper();

                if (certificateData.OverallGrade != null)
                    worksheet.Cells[row, 7].Value = certificateData.OverallGrade.ToUpper();

                if (certificate.CertificateReference != null)
                    worksheet.Cells[row, 8].Value = certificate.CertificateReference.PadLeft(8, '0');

                worksheet.Cells[row, 9].Value = _webConfiguration.CertificateDetails.ChairName;
                worksheet.Cells[row, 10].Value = _webConfiguration.CertificateDetails.ChairTitle;

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

                _aggregateLogger.LogInfo($"Processing Certificate For IFA Certificate - {certificate.CertificateReference}");

                row++;
            }
        }

        private async Task WriteCopyOfMergedDocumentToBlob(string mergedFileName, MemoryStream memoryStream)
        {
            memoryStream.Position = 0;

            var containerName = "mergeddocuments";
            var container = await _initialiseContainer.GetContainer(containerName);

            var blob = container.GetBlockBlobReference(mergedFileName);
            blob.UploadFromStream(memoryStream);

            memoryStream.Position = 0;
        }

        private static string GetMonthYear(string monthFormat)
        {
            var month = DateTime.Today.ToString(monthFormat, new CultureInfo("en-GB"));

            var year = DateTime.Now.Year;
            var monthYear = month + " " + year;
            return monthYear;
        }

        private static string GetMonthYear()
        {
            var month = DateTime.Today.Month.ToString().PadLeft(2, '0');

            var year = DateTime.Now.Year;
            var monthYear = month + "-" + year;
            return monthYear;
        }
    }
}
