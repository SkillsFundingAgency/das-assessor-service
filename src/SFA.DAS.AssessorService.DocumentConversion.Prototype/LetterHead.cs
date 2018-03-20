using System;
using System.IO;
using SFA.DAS.AssessorService.Domain.JsonData;
using Spire.Doc;

namespace SFA.DAS.AssessorService.DocumentConversion.Prototype
{
    public class LetterHead
    {
        public void Create(CertificateData certificateData, MemoryStream documentTemplateStream)
        {
            var pdfStream = CreatePdfStream(certificateData, documentTemplateStream);
            PersistCopyOfLetterHead(pdfStream);
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
