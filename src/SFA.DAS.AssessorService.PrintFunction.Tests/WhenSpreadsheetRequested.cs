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
        [TestCase("JAMES", "SMITH", "James Smith")]
        [TestCase("JAMES", "O'SMITH", "James O'Smith")]
        [TestCase("James", "O'Smith", "James O'Smith")]
        [TestCase("OSCAR", "de la Hoya", "Oscar de la Hoya")]
        [TestCase("abram", "aramot", "Abram Aramot")]
        [TestCase("Abram", "ARAMOT", "Abram Aramot")]
        [TestCase("JAMES", "OSMITH'", "James Osmith'")]
        [TestCase("Aarhus-TERRY", "o'Sullivan", "Aarhus-Terry O'Sullivan")]
        [TestCase("abigantus-liam'", "o'hara", "Abigantus-Liam' O'Hara")]
        [TestCase("Bartley", "Mac o'Donnell", "Bartley Mac O'Donnell")]
        [TestCase("Bartley", "Mac o’donnell", "Bartley Mac O'Donnell")]
        [TestCase("Del", "Stanley", "Del Stanley")]
        [TestCase("Ben", "Bridgehouse", "Ben Bridgehouse")]
        [TestCase("Keith", "KEITH", "Keith Keith")]
        [TestCase("Katie", "Leigh-WILLIAMS", "Katie Leigh-Williams")]
        [TestCase("ADARIA", "Mccarthy", "Adaria McCarthy")]
        [TestCase("Alastair", "MACHIN", "Alastair Machin")]
        [TestCase("ALEC", "Machlin", "Alec Machlin")]
        [TestCase("Angus", "Machar", "Angus Machar")]
        [TestCase("Andrew", "mackle", "Andrew Mackle")]
        [TestCase("ANNabel", "Macklin", "Annabel Macklin")]
        [TestCase("Ansely", "Mackie", "Ansely Mackie")]
        [TestCase("Archibald", "MACquarie", "Archibald Macquarie")]
        [TestCase("Allister", "Machado", "Allister Machado")]
        [TestCase("Alistaire", "Macevicius", "Alistaire Macevicius")]
        [TestCase("Black", "Maciulis", "Black Maciulis")]
        [TestCase("Bonny-LEE", "Macias", "Bonny-Lee Macias")]
        [TestCase("Brodric", "MacMurdo", "Brodric MacMurdo")]
        [TestCase("Camron", "O'Callaghan", "Camron O'Callaghan")]
        [TestCase("Carmichael", "St. John", "Carmichael St. John")]
        [TestCase("Moritz", "von Streit", "Moritz von Streit")]
        [TestCase("MILAN", "van Dyke", "Milan van Dyke")]
        [TestCase("Van", "De-Ville", "Van De-Ville")]
        [TestCase("Gwendolyn", "ap Llwyd Dafydd", "Gwendolyn ap Llwyd Dafydd")]
        [TestCase("Nayef", "al Fahd", "Nayef al Fahd")]
        [TestCase("Al", "GIORDANO", "Al Giordano")]
        [TestCase("Pablo", "el Grecco", "Pablo el Grecco")]
        [TestCase("Doménikos", "Theotokópoulos", "Doménikos Theotokópoulos")]
        [TestCase("David", "ben Gurion", "David ben Gurion")]
        [TestCase("David", "Ben-Gurion", "David Ben-Gurion")]
        [TestCase("Ben", "Disraeli", "Ben Disraeli")]
        [TestCase("Leo", "da Vinci", "Leo da Vinci")]
        [TestCase("Leonardo", "di Caprio", "Leonardo di Caprio")]
        [TestCase("Pierre", "du Pont", "Pierre du Pont")]
        [TestCase("Anthony", "De Legate", "Anthony de Legate")]
        [TestCase("Alessandro", "Del Crond", "Alessandro del Crond")]
        [TestCase("Jan", "der Sind", "Jan der Sind")]
        [TestCase("Arthur", "van Der Post", "Arthur van der Post")]
        [TestCase("Michael", "van den Thillart", "Michael van den Thillart")]
        [TestCase("Max", "VON Trapp", "Max von Trapp")]
        [TestCase("Carl", "la Poisson", "Carl la Poisson")]
        [TestCase("Shamus", "le Figaro", "Shamus le Figaro")]
        [TestCase("Peter", "Mack Knife", "Peter Mack Knife")]
        [TestCase("Alex", "Dougal MacDonald", "Alex Dougal MacDonald")]
        [TestCase("Simon", "Ruiz y Picasso", "Simon Ruiz y Picasso")]
        [TestCase("Javier", "Dato e Iradier", "Javier Dato e Iradier")]
        [TestCase("Ark", "Mas I Gavarró", "Ark Mas i Gavarró")]
        [TestCase("His Majesty", "Henry VIII", "His Majesty Henry VIII")]
        [TestCase("Louis III", "PAR la grâce de Dieu", "Louis III par la grâce de Dieu")]
        [TestCase("Louis XIV", "par la grâce de DIEU", "Louis XIV par la grâce de Dieu")]
        [TestCase("His Majesty","Charles II", "His Majesty Charles II")]
        [TestCase("HER Highness", "Fredrika XLIX", "Her Highness Fredrika XLIX")]
        [TestCase("Yang Amat Berbahagia tun Haji", "Yusof BIN Ishak", "Yang Amat Berbahagia Tun Haji Yusof bin Ishak")]
        public void ThenLearnerNameCasesAreStandardised(string givenNames, string familyName, string expected)
        {
            var worksheet = GenerateWorksheet(new List<CertificateResponse>()
            {
                new CertificateResponse()
                {
                    CertificateData = new CertificateDataResponse()
                    {
                        LearnerGivenNames = givenNames,
                        LearnerFamilyName = familyName,
                        FullName = $"{givenNames} {familyName}"
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