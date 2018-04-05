//using System.IO;
//using System.Threading.Tasks;
//using Microsoft.WindowsAzure.Storage.Blob;
//using SFA.DAS.AssessorService.PrintFunctionProcessFlow.AzureStorage;

//namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.Data
//{
//    public class DocumentTemplateDataStream
//    {
//        private readonly InitialiseBlob _initialiseBlob;
//        private const string TemplateFile = "ReadTest.docx";

//        public DocumentTemplateDataStream(InitialiseBlob initialiseBlob)
//        {
//            _initialiseBlob = initialiseBlob;
//        }

//        public async Task<MemoryStream> Get()
//        {
//            var container = await _initialiseBlob.Execute();
//            if (! await container.GetBlockBlobReference(TemplateFile).ExistsAsync())
//            {
//                await CreateBlob(container);
//            }

//            var blob = container.GetBlockBlobReference(TemplateFile);
//            var memoryStream = new MemoryStream();
//            await blob.DownloadToStreamAsync(memoryStream);

//            return memoryStream;
//        }

//        private async Task CreateBlob(CloudBlobContainer container)
//        {
//            var blob = container.GetBlockBlobReference(TemplateFile);
//            using (Stream file = System.IO.File.OpenRead(TemplateFile))
//            {
//                await blob.UploadFromStreamAsync(file);
//            }
//        }
//    }
//}

using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.AzureStorage;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.Data
{
    public class DocumentTemplateDataStream
    {
        private readonly InitialiseBlob _initialiseBlob;
        private const string TemplateFile = "ReadTest.docx";

        public DocumentTemplateDataStream(InitialiseBlob initialiseBlob)
        {
            _initialiseBlob = initialiseBlob;
        }

        public async Task<MemoryStream> Get()
        {
            var container = await _initialiseBlob.Execute();
            if (!container.GetBlockBlobReference(TemplateFile).Exists())
            {
                CreateBlob(container);
            }

            var blob = container.GetBlockBlobReference(TemplateFile);
            var memoryStream = new MemoryStream();
            blob.DownloadToStream(memoryStream);

            return memoryStream;
        }

        private static void CreateBlob(CloudBlobContainer container)
        {
            var blob = container.GetBlockBlobReference(TemplateFile);
            using (Stream file = System.IO.File.OpenRead(TemplateFile))
            {
                blob.UploadFromStream(file);
            }
        }
    }
}

