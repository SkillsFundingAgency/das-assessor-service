using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using OfficeOpenXml;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.EpaoImporter.DomainServices;
using SFA.DAS.AssessorService.EpaoImporter.Logger;
using SFA.DAS.AssessorService.EpaoImporter.Sftp;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.PrintFunction.Tests
{
    [TestFixture]
    public class WhenSpreadsheetRequested
    {
        [TestCase("JAMES SMITH", "James Smith")]
        [TestCase("JAMES O'SMITH", "James O'Smith")]
        [TestCase("James O'Smith", "James O'Smith")]
        [TestCase("Oscar de la Hoya", "Oscar de la Hoya")]
        [TestCase("abram aramot", "Abram Aramot")]
        [TestCase("Abram ARAMOT", "Abram Aramot")]
        [TestCase("JAMES OSMITH'", "James Osmith'")]
        [TestCase("Aarhus-terry o'Sullivan", "Aarhus-Terry O'Sullivan")]
        [TestCase("abigantus-liam' o'hara", "Abigantus-Liam' O'Hara")]
        [TestCase("Bartley Mac o'Donnell", "Bartley Mac O'Donnell")]
        [TestCase("Bartley Mac o’donnell", "Bartley Mac O'Donnell")]
        public void ThenLearnerNameCasesAreStandardised(string inputName, string expected)
        {
            var worksheet = GenerateWorksheet(new List<CertificateResponse>()
            {
                new CertificateResponse()
                {
                    CertificateData = new CertificateDataResponse()
                    {
                        FullName = inputName
                    }
                }
            });

            worksheet.Cells[3, 2].Value.Should().Be(expected);
        }

        [TestCase(3,1,"21 November 2018")]
        [TestCase(3,3,"PLUMBING")]
        [TestCase(3,4,"(PIPES):")]
        [TestCase(3,5,"LEVEL 3")]
        [TestCase(3,6,"Achieved grade ")]
        [TestCase(3,7,"PASS")]
        [TestCase(3,8,"00000123")]
        [TestCase(3,9,"Dave")]
        [TestCase(3,10,"Lord")]
        [TestCase(3,11,"Mr Jones")]
        [TestCase(3,12,"MegaCorp")]
        [TestCase(3,13,"HR")]
        [TestCase(3,14,"Add1")]
        [TestCase(3,15,"Add2")]
        [TestCase(3,16,"Add3")]
        [TestCase(3,17,"Add4")]
        [TestCase(3,18,"AddPostcode")]
        public void ThenCellsHaveCorrectlyFormattedValues(int row, int col, string expectedValue)
        {
            var worksheet = GenerateWorksheet(new List<CertificateResponse>()
            {
                new CertificateResponse()
                {
                    CertificateData = new CertificateDataResponse()
                    {
                        AchievementDate = new DateTime(2018,11,21),
                        StandardName = "Plumbing",
                        CourseOption = "pipes",
                        StandardLevel = 3,
                        OverallGrade = "pass",
                        ContactName = "Mr\tJones",
                        ContactOrganisation = "MegaCorp",
                        Department = "HR",
                        ContactAddLine1 = "Add1",
                        ContactAddLine2 = "Add2",
                        ContactAddLine3 = "Add3",
                        ContactAddLine4 = "Add4",
                        ContactPostCode = "AddPostcode",
                    },
                    CertificateReference = "123"
                }
            });

            worksheet.Cells[row, col].Value.Should().Be(expectedValue);
        }
        
        [Test]
        public void ThenNoGradeHasNoGradeAwardedValue()
        {
            var worksheet = GenerateWorksheet(new List<CertificateResponse>()
            {
                new CertificateResponse()
                {
                    CertificateData = new CertificateDataResponse()
                    {
                        OverallGrade = "no grade awarded"
                    }
                }
            });

            worksheet.Cells[3, 6].Value.Should().BeNull();
            worksheet.Cells[3, 7].Value.Should().BeNull();
        }

        private static ExcelWorksheet GenerateWorksheet(List<CertificateResponse> certificates)
        {
            var aggregrateLogger = new Mock<IAggregateLogger>();
            var fileTransferClient = new Mock<IFileTransferClient>();
            var webConfig = new WebConfiguration()
                {CertificateDetails = new CertificateDetails() {ChairName = "Dave", ChairTitle = "Lord"}};

            var spreadsheetCreator =
                new PrintingSpreadsheetCreator(aggregrateLogger.Object, fileTransferClient.Object, webConfig);

            MemoryStream spreadsheetStream = null;
            fileTransferClient.Setup(h => h.Send(It.IsAny<MemoryStream>(), It.IsAny<string>()))
                .Callback<MemoryStream, string>((stream, filename) => { spreadsheetStream = stream; });

            spreadsheetCreator.Create(1, certificates);

            spreadsheetStream.Should().NotBeNull();

            var newStream = new MemoryStream(spreadsheetStream.ToArray());

            var excelPackage = new ExcelPackage(newStream);

            var worksheet = excelPackage.Workbook.Worksheets.Single();
            return worksheet;
        }
    }
}