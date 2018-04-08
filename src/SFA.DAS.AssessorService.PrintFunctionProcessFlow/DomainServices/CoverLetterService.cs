using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Data;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Logger;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Sftp;
using Spire.Doc;
using CertificateData = SFA.DAS.AssessorService.Domain.JsonData.CertificateData;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.DomainServices
{
    public class CoverLetterService
    {
        private readonly IAggregateLogger _aggregateLogger;
        private readonly FileTransferClient _fileTransferClient;
        private readonly DocumentTemplateDataStream _documentTemplateDataStream;
        private readonly CertificatesRepository _certificatesRepository;

        public CoverLetterService(
            IAggregateLogger aggregateLogger,
            FileTransferClient fileTransferClient,
            DocumentTemplateDataStream documentTemplateDataStream,
            CertificatesRepository certificatesRepository)
        {
            _aggregateLogger = aggregateLogger;
            _fileTransferClient = fileTransferClient;
            _documentTemplateDataStream = documentTemplateDataStream;
            _certificatesRepository = certificatesRepository;
        }

        public async Task Create()
        {
            var documentTemplateDataStream = await _documentTemplateDataStream.Get();

            foreach (var certificate in _certificatesRepository.GetData())
            {
                var uuid = Guid.NewGuid();
                var fileName = $"output-{uuid}.pdf";

                _aggregateLogger.LogInfo($"Processing Certificate for Cover Letter - {certificate.Id} - {uuid}");
                var certificateData = JsonConvert.DeserializeObject<Domain.JsonData.CertificateData>(certificate.CertificateData);

                _aggregateLogger.LogInfo($"converted certifcate data - Contact Name = {certificateData.ContactName}");

                var pdfStream = CreatePdfStream(certificateData, documentTemplateDataStream);

                await _fileTransferClient.Send(pdfStream, fileName);

                pdfStream.Close();
            }

            documentTemplateDataStream.Close();
        }


        private MemoryStream CreatePdfStream(CertificateData certificateData, MemoryStream documentTemplateStream)
        {
            _aggregateLogger.LogInfo("Merging fields in docuument ...");
            var document = MergeFieldsInDocument(certificateData, documentTemplateStream);
            _aggregateLogger.LogInfo("Converting Document to PDF ...");
            return ConvertDocumentToPdf(document);
        }

        private Document MergeFieldsInDocument(CertificateData certificateData, MemoryStream documentTemplateStream)
        {
            var document = new Document();

            _aggregateLogger.LogInfo("load Document from Stream ...");
            document.LoadFromStream(documentTemplateStream, FileFormat.Docx);
            _aggregateLogger.LogInfo($"Document Length = {document.Count}");

            _aggregateLogger.LogInfo($"Document Template Stream = {documentTemplateStream.Length}");


            document.Replace("[Addressee Name]", string.IsNullOrEmpty(certificateData.ContactName) ? "" : certificateData.ContactName, false, true);
            document.Replace("[Department]",  string.IsNullOrEmpty(certificateData.Department) ? "" : certificateData.Department, false, true);
            document.Replace("[Address Line 1]", string.IsNullOrEmpty(certificateData.ContactAddLine1) ? "" : certificateData.ContactAddLine1, false, true);
            document.Replace("[Address Line 2]", string.IsNullOrEmpty(certificateData.ContactAddLine2) ? "" : certificateData.ContactAddLine2, false, true);
            document.Replace("[Address Line 3]", string.IsNullOrEmpty(certificateData.ContactAddLine3) ? "" : certificateData.ContactAddLine3, false, true);
            document.Replace("[Address Line 4]", string.IsNullOrEmpty(certificateData.ContactAddLine4) ? "" : certificateData.ContactAddLine4, false, true);
            document.Replace("[Address Line 5]", string.IsNullOrEmpty(certificateData.ContactPostCode) ? "" : certificateData.ContactPostCode, false, true);

            document.Replace("[Inset employer name?]", certificateData.ContactName, false, true);
            return document;
        }

        private MemoryStream ConvertDocumentToPdf(Document document)
        {
            var pdfStream = new MemoryStream();
            _aggregateLogger.LogInfo("Saving document to stream ...");
            document.SaveToStream(pdfStream, FileFormat.PDF);
            _aggregateLogger.LogInfo("Saved document to stream ...");
            return pdfStream;
        }
    }
}
