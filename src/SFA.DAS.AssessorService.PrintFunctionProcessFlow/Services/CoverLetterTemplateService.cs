using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Data;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Sftp;
using Spire.Doc;
using CertificateData = SFA.DAS.AssessorService.Domain.JsonData.CertificateData;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.Services
{
    public class CoverLetterTemplateService
    { 
        private readonly FileTransferClient _fileTransferClient;
        private readonly DocumentTemplateDataStream _documentTemplateDataStream;
        private readonly CertificatesRepository _certificatesRepository;

        public CoverLetterTemplateService(
            FileTransferClient fileTransferClient,
            DocumentTemplateDataStream documentTemplateDataStream,
            CertificatesRepository certificatesRepository)
        {
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

                Console.WriteLine($"Processing Certificate for Cover Letter - {certificate.Id} - {uuid}");
                var certificateData = JsonConvert.DeserializeObject<Domain.JsonData.CertificateData>(certificate.CertificateData);

                var pdfStream = CreatePdfStream(certificateData, documentTemplateDataStream);
              
                await _fileTransferClient.Send(pdfStream, fileName);
                
                pdfStream.Close();
            }

            documentTemplateDataStream.Close();
        }


        private MemoryStream CreatePdfStream(CertificateData certificateData, MemoryStream documentTemplateStream)
        {
            var document = MergeFieldsInDocument(certificateData, documentTemplateStream);
            return ConvertDocumentToPdf(document);
        }

        private static Document MergeFieldsInDocument(CertificateData certificateData, MemoryStream documentTemplateStream)
        {
            var document = new Document();
            document.LoadFromStream(documentTemplateStream, FileFormat.Docx);

            document.Replace("[Addressee Name]", certificateData.ContactName, false, true);
            document.Replace("[Address Line 1]", certificateData.ContactAddLine1, false, true);
            document.Replace("[Address Line 2]", certificateData.ContactAddLine2, false, true);
            document.Replace("[Address Line 3]", certificateData.ContactAddLine3, false, true);
            document.Replace("[Address Line 4]", certificateData.ContactAddLine4, false, true);
            document.Replace("[Address Line 5]", certificateData.ContactPostCode, false, true);

            document.Replace("[Inset employer name?]", certificateData.ContactName, false, true);
            return document;
        }

        private static MemoryStream ConvertDocumentToPdf(Document document)
        {
            var pdfStream = new MemoryStream();
            document.SaveToStream(pdfStream, FileFormat.PDF);
            return pdfStream;
        }
    }
}
