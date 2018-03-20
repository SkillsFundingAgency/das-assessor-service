using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.DocumentConversion.Prototype.Data;
using Spire.Doc;
using CertificateData = SFA.DAS.AssessorService.Domain.JsonData.CertificateData;

namespace SFA.DAS.AssessorService.DocumentConversion.Prototype
{
    public class CreateLetterHeads
    {
        private readonly DocumentTemplateDataStream _documentTemplateDataStream;

        public CreateLetterHeads(DocumentTemplateDataStream documentTemplateDataStream)
        {
            _documentTemplateDataStream = documentTemplateDataStream;
        }

        public async Task Create()
        {
            var documentTemplateDataStream = await _documentTemplateDataStream.Get();

            foreach (var certificate in CertificatesRepository.GetData())
            {
                Console.WriteLine($"Processing Certificate - {certificate.Id}");
                var certificateData = JsonConvert.DeserializeObject<Domain.JsonData.CertificateData>(certificate.CertificateData);

                var pdfStream = CreatePdfStream(certificateData, documentTemplateDataStream);
                PersistCopyOfLetterHead(pdfStream);
            }

            documentTemplateDataStream.Close();
        }

        private static MemoryStream CreatePdfStream(CertificateData certificateData, MemoryStream documentTemplateStream)
        {
            //Load Document
            var document = new Document();
            //document.LoadFromFile(@"ReadTest.docx");
            document.LoadFromStream(documentTemplateStream, FileFormat.Docx);

            document.Replace("[Addressee Name]", certificateData.ContactName, false, true);
            document.Replace("[Address Line 1]", certificateData.ContactAddLine1, false, true);
            document.Replace("[Address Line 2]", certificateData.ContactAddLine2, false, true);
            document.Replace("[Address Line 3]", certificateData.ContactAddLine3, false, true);
            document.Replace("[Address Line 4]", certificateData.ContactAddLine4, false, true);
            document.Replace("[Address Line 5]", certificateData.ContactPostCode, false, true);

            document.Replace("[Inset employer name?]", certificateData.ContactName, false, true);

            //Convert Word to PDF
            MemoryStream pdfStream = new MemoryStream();
            document.SaveToStream(pdfStream, FileFormat.PDF);
            return pdfStream;
        }

        private static void PersistCopyOfLetterHead(MemoryStream ms)
        {
            var uuid = Guid.NewGuid();
            var fileName = $"C:\\OutputDirectory\\sample-{uuid}.pdf";

            var file = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            ms.WriteTo(file);
            file.Close();
            ms.Close();

            //Launch Document
            //System.Diagnostics.Process.Start(fileName);
        }
    }
}
