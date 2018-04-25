using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.EpaoImporter.InfrastructureServices
{
    public class BlobContainerHelper
    {
        private readonly IWebConfiguration _webConfiguration;

        public BlobContainerHelper(IWebConfiguration webConfiguration)
        {
            _webConfiguration = webConfiguration;
        }
        public async Task<CloudBlobContainer> GetContainer(string containerName)
        {         
            var storageAccount = CloudStorageAccount.Parse(_webConfiguration.IFATemplateStorageConnectionString);

            var client = storageAccount.CreateCloudBlobClient();

            var blobContainer = client.GetContainerReference(containerName);

            var requestOptions = new BlobRequestOptions() { RetryPolicy = new NoRetry() };
            await blobContainer.CreateIfNotExistsAsync(requestOptions, null);

            return blobContainer;
        }
    }
}
