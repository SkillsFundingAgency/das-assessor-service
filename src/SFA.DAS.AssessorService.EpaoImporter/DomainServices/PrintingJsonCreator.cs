﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.Domain.JsonData.Printing;
using SFA.DAS.AssessorService.EpaoImporter.Extensions;
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

        public void Create(int batchNumber, List<CertificateResponse> certificates, string fileName)
        {
            var printOutput = new PrintOutput
            {
                Batch = new BatchData()
                {
                    BatchNumber = batchNumber,
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
                var contactName = string.Empty;
                if (g.Key.ContactName != null)
                    contactName = g.Key.ContactName.Replace("\t", " ");

                var printData = new PrintData
                {
                    PostalContact = new PostalContact
                    {
                        Name = contactName,
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
                    var learnerName  = 
                        !string.IsNullOrEmpty(c.CertificateData.FullName)
                            ? c.CertificateData.FullName
                            : $"{c.CertificateData.LearnerGivenNames} {c.CertificateData.LearnerFamilyName}";

                    var gradeText = string.Empty;
                    var grade = string.Empty;

                    if (!string.IsNullOrWhiteSpace(c.CertificateData.OverallGrade) && c.CertificateData.OverallGrade!= "No grade awarded")
                    {
                        gradeText = "Achieved grade ";
                        grade = c.CertificateData.OverallGrade;
                    }

                    printData.Certificates.Add(new PrintCertificate
                    {
                        CertificateNumber = c.CertificateReference,
                        ApprenticeName =
                            learnerName.NameCase(),
                        LearningDetails = new LearningDetails()
                        {
                            StandardTitle = c.CertificateData.StandardName,
                            Level = $"LEVEL {c.CertificateData.StandardLevel}",
                            Option = string.IsNullOrWhiteSpace(c.CertificateData?.CourseOption) ? string.Empty: $"({c.CertificateData.CourseOption}):",
                            GradeText = gradeText,
                            Grade = grade,
                            AchievementDate = $"{c.CertificateData.AchievementDate.Value:dd MMMM yyyy}",
                        }
                    });
                });

                printOutput.PrintData.Add(printData);
            });

            _aggregateLogger.LogInfo("Completed Certificates to print Json ....");
            var serializedPrintOutput = JsonConvert.SerializeObject(printOutput);
            byte[] array = Encoding.ASCII.GetBytes(serializedPrintOutput);
            using (var mystream = new MemoryStream(array))
            {
                _aggregateLogger.LogInfo("Sending Certificates to print Json ....");
                _fileTransferClient.Send(mystream, fileName);
            }
        }
    }
}
