using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.DocumentConversion.Prototype.Data;
using SFA.DAS.AssessorService.DocumentConversion.Prototype.Utilities;
using Spire.Doc;
using CertificateData = SFA.DAS.AssessorService.Domain.JsonData.CertificateData;

namespace SFA.DAS.AssessorService.DocumentConversion.Prototype.Services
{
    public class CoverLetterTemplateService
    {
        private readonly IConfiguration _configuration;
        private readonly DocumentTemplateDataStream _documentTemplateDataStream;
        private readonly FileUtilities _fileUtilities;
        private readonly CertificatesRepository _certificatesRepository;

        public CoverLetterTemplateService(IConfiguration configuration,
            DocumentTemplateDataStream documentTemplateDataStream,
            FileUtilities fileUtilities,
            CertificatesRepository certificatesRepository)
        {
            _configuration = configuration;
            _documentTemplateDataStream = documentTemplateDataStream;
            _fileUtilities = fileUtilities;
            _certificatesRepository = certificatesRepository;
        }

        public async Task Create()
        {
            var documentTemplateDataStream = await _documentTemplateDataStream.Get();

            CleanUpLastRun();

            foreach (var certificate in _certificatesRepository.GetData())
            {
                var uuid = Guid.NewGuid();

                Console.WriteLine($"Processing Certificate - {certificate.Id} - {uuid}");
                var certificateData = JsonConvert.DeserializeObject<Domain.JsonData.CertificateData>(certificate.CertificateData);

                var pdfStream = CreatePdfStream(certificateData, documentTemplateDataStream);
                PersistCopyOfLetterHead(uuid, pdfStream);
            }

            documentTemplateDataStream.Close();
        }

        private MemoryStream CreatePdfStream(CertificateData certificateData, MemoryStream documentTemplateStream)
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

        private void PersistCopyOfLetterHead(Guid uuid, MemoryStream ms)
        {
            var outputDirectory = _configuration["OutputDirectory"];
            var fileName = $"{outputDirectory}\\sample-{uuid}.pdf";

            var file = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            ms.WriteTo(file);
            file.Close();
            ms.Close();

            LaunchDocument(fileName);
        }

        private void LaunchDocument(string fileName)
        {
            var launchDocument = _configuration["LaunchDocument"];
            if (launchDocument == "true")
                System.Diagnostics.Process.Start(fileName);
        }

        private void CleanUpLastRun()
        {
            var outputDirectory = _configuration["OutputDirectory"];
            var archiveDirectory = outputDirectory + "\\Archive";
            _fileUtilities.MoveDirectory(outputDirectory, archiveDirectory);
        }
    }
}
