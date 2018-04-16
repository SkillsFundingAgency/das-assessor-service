using System.IO;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.InfrastructureServices;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Logger;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.Data
{
    public class DocumentTemplateDataStream
    {
        private readonly BlobContainerHelper _initialiseContainer;
        private readonly IAggregateLogger _aggregateLogger;
        private const string TemplateFile = "IFATemplateDocument.docx";

        public DocumentTemplateDataStream(BlobContainerHelper initialiseContainer,
            IAggregateLogger aggregateLogger)
        {
            _initialiseContainer = initialiseContainer;
            _aggregateLogger = aggregateLogger;
        }

        public async Task<MemoryStream> Get()
        {
            var containerName = "printfunctionflow";

            var container = await _initialiseContainer.GetContainer(containerName);

            var blob = container.GetBlockBlobReference(TemplateFile);
            var memoryStream = new MemoryStream();
            blob.DownloadToStream(memoryStream);

            _aggregateLogger.LogInfo($"Downloaded memory stream length = {blob.Properties.Length}");

            return memoryStream;
        }
    }
}

