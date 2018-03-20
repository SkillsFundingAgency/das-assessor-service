using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using SFA.DAS.AssessorService.DocumentConversion.Prototype.AzureStorage;

namespace SFA.DAS.AssessorService.DocumentConversion.Prototype.Data
{
    public class DocumentTemplateDataStream
    {
        private readonly InitialiseBlob _initialiseBlob;

        public DocumentTemplateDataStream(InitialiseBlob initialiseBlob)
        {
            _initialiseBlob = initialiseBlob;
        }

        public async Task<MemoryStream> Get()
        {
            var container = await _initialiseBlob.Execute();

            if (!container.GetBlockBlobReference("ReadTest.docx").Exists())
            {
                CreateBlob(container);
            }
           
            var blob = container.GetBlockBlobReference("ReadTest.docx");
            var memoryStream = new MemoryStream();
            blob.DownloadToStream(memoryStream);

            return memoryStream;

            //using (Stream outputFile = new FileStream("Downloaded.docx", FileMode.OpenOrCreate))
            //{
            //    blob.DownloadToStream(outputFile);
            //}
        }

        private static void CreateBlob(CloudBlobContainer container)
        {
            var blob = container.GetBlockBlobReference("ReadTest.docx");
            using (Stream file = System.IO.File.OpenRead("ReadTest.docx"))
            {
                blob.UploadFromStream(file);
            }
        }
    }
}
