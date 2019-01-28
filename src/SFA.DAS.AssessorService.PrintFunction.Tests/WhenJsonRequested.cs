using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using OfficeOpenXml;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.JsonData.Printing;
using SFA.DAS.AssessorService.EpaoImporter.DomainServices;
using SFA.DAS.AssessorService.EpaoImporter.Logger;
using SFA.DAS.AssessorService.EpaoImporter.Sftp;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.PrintFunction.Tests
        
{
    [TestFixture]
    public class WhenJsonRequested
    {


        [Test]
        public void ThenSingleItemReturnsPostalContact1TotalCertificate1()
        {
            var certificateSummary = GenerateJson(new List<CertificateResponse>()
            {
                DefaultCertificateResponse("address line 1", "123")
                
            });

            certificateSummary.Batch.PostalContactCount.Should().Be(1);
            certificateSummary.Batch.TotalCertificateCount.Should().Be(1);
        }


        [Test]
        public void ThenTwoItemsWithSameAddressReturnsPostalContact1TotalCertificate2()
        {
            var certificateSummary = GenerateJson(new List<CertificateResponse>()
            {
                DefaultCertificateResponse("address line 1", "123"),
                DefaultCertificateResponse("address line 1", "456")

            });

            certificateSummary.Batch.PostalContactCount.Should().Be(1);
            certificateSummary.Batch.TotalCertificateCount.Should().Be(2);
        }


        [Test]
        public void ThenThreeItemsWithTwoSameAddressReturnsPostalContact2TotalCertificate3()
        {
            var certificateSummary = GenerateJson(new List<CertificateResponse>()
            {
                DefaultCertificateResponse("address line 1", "123"),
                DefaultCertificateResponse("address line 1", "456"),
                DefaultCertificateResponse("address line 1b", "789")

            });

            certificateSummary.Batch.PostalContactCount.Should().Be(2);
            certificateSummary.Batch.TotalCertificateCount.Should().Be(3);
        }


        private CertificateResponse DefaultCertificateResponse(string addressLine1, string certificateReference)
        {
            return new CertificateResponse()
            {
                CertificateData = new CertificateDataResponse()
                {
                    AchievementDate = new DateTime(2018, 11, 21),
                    StandardName = "Plumbing",
                    CourseOption = "pipes",
                    StandardLevel = 3,
                    OverallGrade = "pass",
                    ContactName = "Mr\tJones",
                    ContactOrganisation = "MegaCorp",
                    Department = "HR",
                    ContactAddLine1 = addressLine1,
                    ContactAddLine2 = "Add2",
                    ContactAddLine3 = "Add3",
                    ContactAddLine4 = "Add4",
                    ContactPostCode = "AddPostcode",
                },
                CertificateReference = certificateReference
            };
        }

        private static PrintOutput GenerateJson(List<CertificateResponse> certificates)
        {
            var aggregrateLogger = new Mock<IAggregateLogger>();
            var fileTransferClient = new Mock<IFileTransferClient>();
            var webConfig = new WebConfiguration()
                { CertificateDetails = new CertificateDetails() { ChairName = "Dave", ChairTitle = "Lord" } };

            var spreadsheetCreator =
                new PrintingJsonCreator(aggregrateLogger.Object, fileTransferClient.Object, webConfig);

            MemoryStream spreadsheetStream = null;
            fileTransferClient.Setup(h => h.Send(It.IsAny<MemoryStream>(), It.IsAny<string>()))
                .Callback<MemoryStream, string>((stream, filename) => { spreadsheetStream = stream; });

            spreadsheetCreator.Create(1, certificates, "filename.json");

            spreadsheetStream.Should().NotBeNull();

            var newStream = new MemoryStream(spreadsheetStream.ToArray());
            //var serializedPrintOutput = JsonConvert.DeserializeObject(newStream.Read());
            return JsonConvert.DeserializeObject<PrintOutput>(System.Text.Encoding.Default.GetString(newStream.ToArray()));
        }
    }
}

