using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace SFA.DAS.AssessorService.EpaoImporter.InfrastructureServices
{
    public class BlobContainerHelper
    {

        public async Task<CloudBlobContainer> GetContainer(string containerName)
        {
            var storageAccountName = CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString");
            var storageAccount = CloudStorageAccount.Parse(storageAccountName);

            var client = storageAccount.CreateCloudBlobClient();

            var blobContainer = client.GetContainerReference(containerName);

            var requestOptions = new BlobRequestOptions() { RetryPolicy = new NoRetry() };
            await blobContainer.CreateIfNotExistsAsync(requestOptions, null);

            return blobContainer;
        }
    }
}
