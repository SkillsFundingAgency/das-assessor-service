using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SFA.DAS.AssessorService.DocumentConversion.Prototype.Data;
using SFA.DAS.AssessorService.DocumentConversion.Prototype.Sftp;

namespace SFA.DAS.AssessorService.DocumentConversion.Prototype.Services
{
    public class IFACertificateService
    {
        private readonly IConfiguration _configuration;
        private readonly FileTransferClient _fileTransferClient;
        private readonly CertificatesRepository _certificatesRepository;

        public IFACertificateService(IConfiguration configuration,
            FileTransferClient fileTransferClient,
            CertificatesRepository certificatesRepository)
        {
            _configuration = configuration;
            _fileTransferClient = fileTransferClient;
            _certificatesRepository = certificatesRepository;
        }

        public async Task Create()
        {           
            var memoryStream = new MemoryStream();

            var uuid = Guid.NewGuid();
            var fileName = $"output-{uuid}.xlsx";

            using (var package = new ExcelPackage(memoryStream))
            {
                CreateWorkBook(package);
                CreateWorkSheet(package);

                package.Save();

                memoryStream.Position = 0;
                await _fileTransferClient.Send(memoryStream, fileName);

                CreateOutputFile(memoryStream, fileName);

                memoryStream.Close();
            }            
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
            var worksheet = package.Workbook.Worksheets.Add("DEC 2017");

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
            worksheet.Cells[2, 13].Value = "Address Line 1";
            worksheet.Cells[2, 14].Value = "Address Line 2";
            worksheet.Cells[2, 15].Value = "Address Line 3";
            worksheet.Cells[2, 16].Value = "Address Line 4";
            worksheet.Cells[2, 17].Value = "Post Code";

            using (var range = worksheet.Cells[2, 1, 2, 17])
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
            using (var range = worksheet.Cells[1, 1, 1, 17])
            {
                range.Style.Font.Bold = true;
                //range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                //range.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                range.Style.Font.Color.SetColor(Color.Red);
                range.Style.Font.Size = 20;
            }

            worksheet.Cells["A1:J1"].Merge = true;
            worksheet.Cells["A1:J1"].Value = "December 2017 Print Data";
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

                worksheet.Cells[row, 9].Value = _configuration["ChairDetails:ChairName"];
                worksheet.Cells[row, 10].Value = _configuration["ChairDetails:ChairTitle"];

                if (certificateData.ContactName != null)
                    worksheet.Cells[row, 11].Value = certificateData.ContactName;

                if (certificateData.ContactOrganisation != null)
                    worksheet.Cells[row, 12].Value = certificateData.ContactOrganisation;

                if (certificateData.ContactAddLine1 != null)
                    worksheet.Cells[row, 13].Value = certificateData.ContactAddLine1;

                if (certificateData.ContactAddLine2 != null)
                    worksheet.Cells[row, 14].Value = certificateData.ContactAddLine2;

                if (certificateData.ContactAddLine3 != null)
                    worksheet.Cells[row, 15].Value = certificateData.ContactAddLine3;

                if (certificateData.ContactAddLine4 != null)
                    worksheet.Cells[row, 16].Value = certificateData.ContactAddLine4;

                if (certificateData.ContactPostCode != null)
                    worksheet.Cells[row, 17].Value = certificateData.ContactPostCode;

                Console.WriteLine($"Processing Certificate For IFA Certificate - {certificate.Id}");

                row++;
            }
        }

        private void CreateOutputFile(MemoryStream memoryStream, string fileName)
        {
            var directoryName = _configuration["OutputDirectory"] + "\\Excel\\";
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
         
            var fullFileName = directoryName + fileName;

            using (FileStream file = new FileStream(fullFileName, FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite))
            {
                var bytes = new byte[memoryStream.Length];
                memoryStream.Read(bytes, 0, (int)memoryStream.Length);
                file.Write(bytes, 0, bytes.Length);        
            }
        }
    }
}
