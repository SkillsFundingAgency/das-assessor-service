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
        private readonly FileTransferClient _fileTransferClient;
        private readonly DocumentTemplateDataStream _documentTemplateDataStream;
        private readonly BlobContainerHelper _initialiseContainer;

        public CoverLetterService(
            IAggregateLogger aggregateLogger,
            FileTransferClient fileTransferClient,
            DocumentTemplateDataStream documentTemplateDataStream,
            BlobContainerHelper initialiseContainer)
        {
            _aggregateLogger = aggregateLogger;
            _fileTransferClient = fileTransferClient;
            _documentTemplateDataStream = documentTemplateDataStream;
            _initialiseContainer = initialiseContainer;
        }

        public async Task Create(int batchNumber, IEnumerable<CertificateResponse> certificates)
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

            foreach (var groupedCertificate in groupedCertificates)
            {
                sequenceNumber++;
                var certificate = groupedCertificate.Result[0];
                var wordDocumentFileName = $"IFA-Certificate-{GetMonthYear()}-{batchNumber.ToString().PadLeft(9,'0')}-{certificate.CertificateData.ContactOrganisation}-{sequenceNumber}.docx";

                _aggregateLogger.LogInfo($"Processing Certificate for Cover Letter - {certificate.CertificateReference} - {wordDocumentFileName}");
                var wordStream = await CreateWordDocumentStream(wordDocumentFileName, certificate.CertificateData, documentTemplateDataStream);

                _aggregateLogger.LogInfo($"converted certifcate data - Contact Name = {certificate.CertificateData.ContactName}");

                await _fileTransferClient.Send(wordStream, wordDocumentFileName);

                wordStream.Close();
            }

            documentTemplateDataStream.Close();
        }

        private async Task<MemoryStream> CreateWordDocumentStream(string mergedFileName, CertificateDataResponse certificateData, MemoryStream documentTemplateStream)
        {
            _aggregateLogger.LogInfo("Merging fields in docuument ...");
            var document = MergeFieldsInDocument(certificateData, documentTemplateStream);
            _aggregateLogger.LogInfo("Converting Document to PDF ...");

            await WriteCopyOfMergedDocumentToBlob(mergedFileName, document);

            return ConvertDocumentToStream(document);
        }

        private Document MergeFieldsInDocument(CertificateDataResponse certificateData, MemoryStream documentTemplateStream)
        {
            var document = new Document();

            _aggregateLogger.LogInfo("load Document from Stream ...");
            document.LoadFromStream(documentTemplateStream, FileFormat.Docx);
            _aggregateLogger.LogInfo($"Document Length = {document.Count}");

            _aggregateLogger.LogInfo($"Document Template Stream = {documentTemplateStream.Length}");

            document.Replace("[Addressee Name]", string.IsNullOrEmpty(certificateData.ContactName) ? "" : certificateData.ContactName, false, true);
            document.Replace("[Department]", string.IsNullOrEmpty(certificateData.Department) ? "" : certificateData.Department, false, true);
            document.Replace("[Address Line 1]", string.IsNullOrEmpty(certificateData.ContactAddLine1) ? "" : certificateData.ContactAddLine1, false, true);
            document.Replace("[Address Line 2]", string.IsNullOrEmpty(certificateData.ContactAddLine2) ? "" : certificateData.ContactAddLine2, false, true);
            document.Replace("[Address Line 3]", string.IsNullOrEmpty(certificateData.ContactAddLine3) ? "" : certificateData.ContactAddLine3, false, true);
            document.Replace("[Address Line 4]", string.IsNullOrEmpty(certificateData.ContactAddLine4) ? "" : certificateData.ContactAddLine4, false, true);
            document.Replace("[Address Line 5]", string.IsNullOrEmpty(certificateData.ContactPostCode) ? "" : certificateData.ContactPostCode, false, true);

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

        private async Task WriteCopyOfMergedDocumentToBlob(string mergedFileName, Document document)
        {
            var memoryStream = new MemoryStream();
            document.SaveToStream(memoryStream, FileFormat.Docx);

            memoryStream.Position = 0;

            var containerName = "mergeddocuments";
            var container = await _initialiseContainer.GetContainer(containerName);

            var blob = container.GetBlockBlobReference(mergedFileName);
            blob.UploadFromStream(memoryStream);

            memoryStream.Position = 0;
            //await _fileTransferClient.Send(memoryStream, mergedFileName);
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
            var monthYear = month + "-" + year;
            return monthYear;
        }
    }
}
