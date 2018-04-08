using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.AzureStorage;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Logger;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.Data
{
    public class DocumentTemplateDataStream
    {
        private readonly InitialiseContainer _initialiseContainer;
        private readonly IAggregateLogger _aggregateLogger;
        private const string TemplateFile = "ReadTest.docx";

        public DocumentTemplateDataStream(InitialiseContainer initialiseContainer,
            IAggregateLogger aggregateLogger)
        {
            _initialiseContainer = initialiseContainer;
            _aggregateLogger = aggregateLogger;
        }

        public async Task<MemoryStream> Get()
        {
            var containerName = "printfunctionflow";

            var container = await _initialiseContainer.Execute(containerName);
            if (!container.GetBlockBlobReference(TemplateFile).Exists())
            {
                CreateBlob(container);
            }

            var blob = container.GetBlockBlobReference(TemplateFile);
            var memoryStream = new MemoryStream();
            blob.DownloadToStream(memoryStream);

            _aggregateLogger.LogInfo($"Downloaded memory stream length = {blob.Properties.Length}");

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

