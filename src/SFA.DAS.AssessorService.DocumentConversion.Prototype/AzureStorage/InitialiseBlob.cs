using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace SFA.DAS.AssessorService.DocumentConversion.Prototype.AzureStorage
{
    public class InitialiseBlob
    {
        private readonly Common _common;
        private const string ContainerPrefix = "sample";

        public InitialiseBlob(Common common)
        {
            _common = common;
        }

        public async Task<CloudBlobContainer> Execute()
        {
            string containerName = ContainerPrefix; //+ Guid.NewGuid();

            //StorageCredentials creds = new StorageCredentials(accountName, accountKey);
            CloudStorageAccount storageAccount = _common.CreateStorageAccountFromConnectionString();

            CloudBlobClient client = storageAccount.CreateCloudBlobClient();

            // Create a container for organizing blobs within the storage account.
            Console.WriteLine("1. Creating Container");
            CloudBlobContainer sampleContainer = client.GetContainerReference(containerName);

            BlobRequestOptions requestOptions = new BlobRequestOptions() { RetryPolicy = new NoRetry() };
            await sampleContainer.CreateIfNotExistsAsync(requestOptions, null);

            return sampleContainer;
        }
    }
}
