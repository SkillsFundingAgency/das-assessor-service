using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Api.Types.Models.Printing;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.JsonData;
using SFA.DAS.AssessorService.Domain.JsonData.Printing;
using SFA.DAS.AssessorService.EpaoImporter.Interfaces;
using SFA.DAS.AssessorService.EpaoImporter.Logger;
using SFA.DAS.AssessorService.EpaoImporter.Sftp;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.EpaoImporter.DomainServices
{
    public class PrintingJsonCreator:IPrintingJsonCreator
    {
        private readonly IAggregateLogger _aggregateLogger;
        private readonly IFileTransferClient _fileTransferClient;
        private readonly IWebConfiguration _webConfiguration;

        public PrintingJsonCreator(
            IAggregateLogger aggregateLogger,
            IFileTransferClient fileTransferClient,
            IWebConfiguration webConfiguration)
        {
            _aggregateLogger = aggregateLogger;
            _fileTransferClient = fileTransferClient;
            _webConfiguration = webConfiguration;
        }

        public void Create(int batchNumber, List<CertificateResponse> certificates)
        {
            var printOutput = new PrintOutput
            {
                Batch = new BatchDetails()
                {
                    BatchNumber = 1,
                    BatchDate = DateTime.UtcNow
                },
                PrintData = new List<PrintData>()
            };
  
            printOutput.Batch.TotalCertificateCount = certificates.Count;         

            var groupedByRecipient = certificates.GroupBy(c =>
                new
                {
                    c.CertificateData.ContactName,
                    c.CertificateData.ContactOrganisation,
                    c.CertificateData.Department,
                    c.CertificateData.ContactAddLine1,
                    c.CertificateData.ContactAddLine2,
                    c.CertificateData.ContactAddLine3,
                    c.CertificateData.ContactAddLine4,
                    c.CertificateData.ContactPostCode
                }).ToList();


            printOutput.Batch.PostalContactCount = groupedByRecipient.Count;

            groupedByRecipient.ForEach(g =>
            {
                var printData = new PrintData
                {
                    PostalContact = new PostalContact
                    {
                        Name = g.Key.ContactName,
                        Department = g.Key.Department,
                        EmployerName = g.Key.ContactOrganisation,
                        AddressLine1 = g.Key.ContactAddLine1,
                        AddressLine2 = g.Key.ContactAddLine2,
                        AddressLine3 = g.Key.ContactAddLine3,
                        AddressLine4 = g.Key.ContactAddLine4,
                        Postcode = g.Key.ContactPostCode,
                        CertificateCount = g.Count()
                    },
                    CoverLetter = new CoverLetter
                    {
                        ChairName = "The Chair",
                        ChairTitle = "Chair of the board"
                    },
                    Certificates = new List<PrintCertificate>()
                };

                g.ToList().ForEach(c =>
                {
                    printData.Certificates.Add(new PrintCertificate
                    {
                        CertificateNumber = c.CertificateReference,
                        ApprenticeName =
                            $"{c.CertificateData.LearnerGivenNames} {c.CertificateData.LearnerFamilyName}",
                        LearningDetails = new LearningDetails()
                        {
                            StandardTitle = c.CertificateData.StandardName,
                            Level = $"LEVEL {c.CertificateData.StandardLevel}",
                            Option = c.CertificateData.CourseOption,
                            GradeText = string.IsNullOrWhiteSpace(c.CertificateData.OverallGrade) ? null : "achieving a",
                            Grade = c.CertificateData.OverallGrade,
                            AchievementDate = $"{c.CertificateData.AchievementDate.Value:dd MMM, yyyy}",
                        }
                    });
                });

                printOutput.PrintData.Add(printData);
            });
        

        var serializedPrintOutput = JsonConvert.SerializeObject(printOutput);
            byte[] array = Encoding.ASCII.GetBytes(serializedPrintOutput);
            using (var mystream = new MemoryStream(array))
            {
                _fileTransferClient.Send(mystream, "test.json");
            }

                var x = 1;
            //File.WriteAllText("CertificatesPrintOutput.json", serializedPrintOutput); 

        }
    }

}
