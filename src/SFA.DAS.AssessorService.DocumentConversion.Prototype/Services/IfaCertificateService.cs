using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SFA.DAS.AssessorService.DocumentConversion.Prototype.Data;

namespace SFA.DAS.AssessorService.DocumentConversion.Prototype.Services
{
    public class IFACertificateService
    {
        private readonly IConfiguration _configuration;
        private readonly CertificatesRepository _certificatesRepository;

        public IFACertificateService(IConfiguration configuration,
            CertificatesRepository certificatesRepository)
        {
            _configuration = configuration;
            _certificatesRepository = certificatesRepository;
        }

        public void Create()
        {
            var directoryName = @"C:\\OutputDirectory\\Excel";
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            Guid uuid = Guid.NewGuid();
            var fileName = directoryName + $"\\output-{uuid}.xlsx";

            FileInfo file = new FileInfo(fileName);

            using (ExcelPackage package = new ExcelPackage(file))
            {

                var workbook = package.Workbook;
                workbook.Protection.LockWindows = true;
                workbook.Protection.LockStructure = true;
                //workbook.View.SetWindowSize(150, 525, 14500, 6000);
                workbook.View.ShowHorizontalScrollBar = true;
                workbook.View.ShowVerticalScrollBar = true;
                workbook.View.ShowSheetTabs = true;

                var worksheet = package.Workbook.Worksheets.Add("DEC 2017");
                worksheet.Cells.Style.Font.Name = "Calibri";


                //worksheet.PrinterSettings.PaperSize = ePaperSize.A3;
                //worksheet.PrinterSettings.Orientation = eOrientation.Landscape;
                //worksheet.PrinterSettings.FitToPage = true;
                //worksheet.PrinterSettings.FitToHeight = 1;
                //worksheet.PrinterSettings.FooterMargin = .05M;
                //worksheet.PrinterSettings.TopMargin = .05M;
                //worksheet.PrinterSettings.LeftMargin = .05M;
                //worksheet.PrinterSettings.RightMargin = .05M;

                //worksheet.Column(30).PageBreak = true;
                //worksheet.Row(30).PageBreak = true;


                //worksheet.Cells["A1:M20"].AutoFitColumns();
                //Add the headers


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
                worksheet.Cells[2, 9].Value = "Ch8ir Name";
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

                worksheet.Cells.AutoFitColumns(0);  //Autofit columns for all cell            

                // lets set the header text 
                worksheet.HeaderFooter.FirstHeader.LeftAlignedText = "&24&\"Arial,Regular Bold\" December 2017 Print Data";
                // add the page number to the footer plus the total number of pages
                worksheet.HeaderFooter.OddFooter.RightAlignedText =
                    string.Format("Page {0} of {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
                // add the sheet name to the footer
                worksheet.HeaderFooter.OddFooter.CenteredText = ExcelHeaderFooter.SheetName;
                // add the file path to the footer
                worksheet.HeaderFooter.OddFooter.LeftAlignedText = ExcelHeaderFooter.FilePath + ExcelHeaderFooter.FileName;

                worksheet.View.PageLayoutView = false;

                // set some document properties
                package.Workbook.Properties.Title = "Invertory";
                package.Workbook.Properties.Author = "Jan Källman";
                package.Workbook.Properties.Comments = "This sample demonstrates how to create an Excel 2007 workbook using EPPlus";

                // set some extended property values
                package.Workbook.Properties.Company = "AdventureWorks Inc.";

                // set some custom property values
                package.Workbook.Properties.SetCustomPropertyValue("Checked by", "Jan Källman");
                package.Workbook.Properties.SetCustomPropertyValue("AssemblyName", "EPPlus");
                // save our new workbook and we are done!

                //Add some items...
                //worksheet.Cells["A2"].Value = 12001;
                //worksheet.Cells["B2"].Value = "Nails";
                //worksheet.Cells["C2"].Value = 37;
                //worksheet.Cells["D2"].Value = 3.99;

                //worksheet.Cells["A3"].Value = 12002;
                //worksheet.Cells["B3"].Value = "Hammer";
                //worksheet.Cells["C3"].Value = 5;
                //worksheet.Cells["D3"].Value = 12.10;

                //worksheet.Cells["A4"].Value = 12003;
                //worksheet.Cells["B4"].Value = "Saw";
                //worksheet.Cells["C4"].Value = 12;
                //worksheet.Cells["D4"].Value = 15.37;

                ////Add a formula for the value-column
                //worksheet.Cells["E2:E4"].Formula = "C2*D2";

                ////Ok now format the values;
                //using (var range = worksheet.Cells[1, 1, 1, 5])
                //{
                //    range.Style.Font.Bold = true;
                //    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                //    range.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                //    range.Style.Font.Color.SetColor(Color.White);
                //}

                //worksheet.Cells["A5:E5"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                //worksheet.Cells["A5:E5"].Style.Font.Bold = true;

                //worksheet.Cells[5, 3, 5, 5].Formula = string.Format("SUBTOTAL(9,{0})", new ExcelAddress(2, 3, 4, 3).Address);
                //worksheet.Cells["C2:C5"].Style.Numberformat.Format = "#,##0";
                //worksheet.Cells["D2:E5"].Style.Numberformat.Format = "#,##0.00";

                ////Create an autofilter for the range
                //worksheet.Cells["A1:E4"].AutoFilter = true;

                //worksheet.Cells["A2:A4"].Style.Numberformat.Format = "@";   //Format as text

                ////There is actually no need to calculate, Excel will do it for you, but in some cases it might be useful. 
                ////For example if you link to this workbook from another workbook or you will open the workbook in a program that hasn't a calculation engine or 
                ////you want to use the result of a formula in your program.
                //worksheet.Calculate();

                //worksheet.Cells.AutoFitColumns(0);  //Autofit columns for all cells

                //// lets set the header text 
                //worksheet.HeaderFooter.OddHeader.CenteredText = "&24&U&\"Arial,Regular Bold\" Inventory";
                //// add the page number to the footer plus the total number of pages
                //worksheet.HeaderFooter.OddFooter.RightAlignedText =
                //    string.Format("Page {0} of {1}", ExcelHeaderFooter.PageNumber, ExcelHeaderFooter.NumberOfPages);
                //// add the sheet name to the footer
                //worksheet.HeaderFooter.OddFooter.CenteredText = ExcelHeaderFooter.SheetName;
                //// add the file path to the footer
                //worksheet.HeaderFooter.OddFooter.LeftAlignedText = ExcelHeaderFooter.FilePath + ExcelHeaderFooter.FileName;

                //worksheet.PrinterSettings.RepeatRows = worksheet.Cells["1:2"];
                //worksheet.PrinterSettings.RepeatColumns = worksheet.Cells["A:G"];

                //// Change the sheet view to show it in page layout mode
                //worksheet.View.PageLayoutView = true;

                //// set some document properties
                //package.Workbook.Properties.Title = "Invertory";
                //package.Workbook.Properties.Author = "Jan Källman";
                //package.Workbook.Properties.Comments = "This sample demonstrates how to create an Excel 2007 workbook using EPPlus";

                //// set some extended property values
                //package.Workbook.Properties.Company = "AdventureWorks Inc.";

                //// set some custom property values
                //package.Workbook.Properties.SetCustomPropertyValue("Checked by", "Jan Källman");
                //package.Workbook.Properties.SetCustomPropertyValue("AssemblyName", "EPPlus");
                // save our new workbook and we are done!
                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;
                bool bHeaderRow = true;

                var certificates = _certificatesRepository.GetData().ToList();

                int row = 3;

                foreach (var certificate in certificates)
                {
                    var certificateData = JsonConvert.DeserializeObject<Domain.JsonData.CertificateData>(certificate.CertificateData);
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

                    Console.WriteLine($"Processing Certificate For Spread Sheet - {certificate.Id}");

                    row++;
                }

                package.Save();
            }
        }
    }
}
