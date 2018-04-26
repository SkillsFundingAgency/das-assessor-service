using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;
using SFA.DAS.AssessorService.EpaoImporter.Data;
using SFA.DAS.AssessorService.EpaoImporter.InfrastructureServices;
using SFA.DAS.AssessorService.EpaoImporter.Logger;
using SFA.DAS.AssessorService.EpaoImporter.Sftp;
using Spire.Doc;

namespace SFA.DAS.AssessorService.EpaoImporter.DomainServices
{
    public class CoverLetterService
    {
        private readonly IAggregateLogger _aggregateLogger;
        private readonly IFileTransferClient _fileTransferClient;
        private readonly DocumentTemplateDataStream _documentTemplateDataStream;
        private readonly BlobContainerHelper _initialiseContainer;

        public CoverLetterService(
            IAggregateLogger aggregateLogger,
            IFileTransferClient fileTransferClient,
            DocumentTemplateDataStream documentTemplateDataStream,
            BlobContainerHelper initialiseContainer)
        {
            _aggregateLogger = aggregateLogger;
            _fileTransferClient = fileTransferClient;
            _documentTemplateDataStream = documentTemplateDataStream;
            _initialiseContainer = initialiseContainer;
        }

        public async Task<List<string>> Create(int batchNumber, IEnumerable<CertificateResponse> certificates)
        {
            var documentTemplateDataStream = await _documentTemplateDataStream.Get();

            await CleanMergedDocumentContainer();

            var certificateResponses = certificates as CertificateResponse[] ?? certificates.ToArray();
            var groupedCertificates = certificateResponses.ToArray().GroupBy(
                x => new
                {
                    x.CertificateData.ContactOrganisation,
                    x.CertificateData.Department,
                    x.CertificateData.ContactName,
                    x.CertificateData.ContactPostCode
                },
                (key, group) => new
                {
                    key1 = key.ContactOrganisation,
                    key2 = key.Department,
                    key3 = key.ContactName,
                    key4 = key.ContactPostCode,
                    Result = group.ToList()
                });

            var sequenceNumber = 0;
            var coverLetterFileNames = new List<string>();

            foreach (var groupedCertificate in groupedCertificates)
            {
                sequenceNumber++;

                var certificate = groupedCertificate.Result[0];
                var wordDocumentFileName = $"IFA-Certificate-{GetMonthYear()}-{batchNumber.ToString().PadLeft(3, '0')}-{certificate.CertificateData.ContactOrganisation}-{sequenceNumber}.docx";
                coverLetterFileNames.Add(wordDocumentFileName);

                _aggregateLogger.LogInfo($"Processing Certificate for Cover Letter - {certificate.CertificateReference} - {wordDocumentFileName}");
                var wordStream = await CreateWordDocumentStream(wordDocumentFileName, certificate.CertificateData, documentTemplateDataStream);

                _aggregateLogger.LogInfo($"converted certifcate data - Contact Name = {certificate.CertificateData.ContactName}");

                await _fileTransferClient.Send(wordStream, wordDocumentFileName);

                wordStream.Close();
            }

            documentTemplateDataStream.Close();

            return coverLetterFileNames;
        }

        private async Task<MemoryStream> CreateWordDocumentStream(string mergedFileName, CertificateDataResponse certificateData, MemoryStream documentTemplateStream)
        {
            _aggregateLogger.LogInfo("Merging fields in document ...");
            var document = MergeFieldsInDocument(certificateData, documentTemplateStream);
            _aggregateLogger.LogInfo("Merged fields in Document");

            return ConvertDocumentToStream(document);
        }

        private Document MergeFieldsInDocument(CertificateDataResponse certificateData, MemoryStream documentTemplateStream)
        {
            var document = new Document();

            _aggregateLogger.LogInfo("load Document from Stream ...");
            document.LoadFromStream(documentTemplateStream, FileFormat.Docx);
            _aggregateLogger.LogInfo($"Document Length = {document.Count}");

            _aggregateLogger.LogInfo($"Document Template Stream = {documentTemplateStream.Length}");

            var contactDetails = "";
            if (!string.IsNullOrEmpty(certificateData.Department))
            {
                contactDetails += certificateData.Department + System.Environment.NewLine;
            }

            if (!string.IsNullOrEmpty(certificateData.ContactAddLine1))
            {
                contactDetails += certificateData.ContactAddLine1 + System.Environment.NewLine;
            }

            if (!string.IsNullOrEmpty(certificateData.ContactAddLine2))
            {
                contactDetails += certificateData.ContactAddLine2 + System.Environment.NewLine;
            }

            if (!string.IsNullOrEmpty(certificateData.ContactAddLine3))
            {
                contactDetails += certificateData.ContactAddLine3 + System.Environment.NewLine;
            }

            if (!string.IsNullOrEmpty(certificateData.ContactPostCode))
            {
                contactDetails += certificateData.ContactPostCode + System.Environment.NewLine;
            }

            document.Replace("[Addressee Name]", string.IsNullOrEmpty(certificateData.ContactName) ? "" : certificateData.ContactName, false, true);
            document.Replace("[ContactDetails]", contactDetails, false, true);           

            document.Replace("[Inset employer name?]", certificateData.ContactName, false, true);
            return document;
        }

        private MemoryStream ConvertDocumentToStream(Document document)
        {
            var wordDocxStream = new MemoryStream();
            _aggregateLogger.LogInfo("Saving document to stream ...");
            document.SaveToStream(wordDocxStream, FileFormat.Docx);
            _aggregateLogger.LogInfo("Saved document to stream ...");
            return wordDocxStream;
        }

        private async Task CleanMergedDocumentContainer()
        {
            var containerName = "mergeddocuments";
            var container = await _initialiseContainer.GetContainer(containerName);

            Parallel.ForEach(container.ListBlobs(), x => ((CloudBlob)x).Delete());
        }

        private static string GetMonthYear()
        {
            var month = DateTime.Today.Month.ToString().PadLeft(2, '0');

            var year = DateTime.Now.Year;
            var monthYear = month + year.ToString().Substring(2, 2);
            return monthYear;
        }
    }
}
