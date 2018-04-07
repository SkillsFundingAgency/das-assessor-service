//using System.Threading.Tasks;
//using Microsoft.WindowsAzure.Storage.Blob;
//using Microsoft.WindowsAzure.Storage.RetryPolicies;

//namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.AzureStorage
//{
//    public class InitialiseBlob
//    {
//        private readonly Common _common;

//        public InitialiseBlob(Common common)
//        {
//            _common = common;
//        }

//        public async Task<CloudBlobContainer> Execute()
//        {
//            var containerName = "printfunction-prototype";

//            var storageAccount = _common.CreateStorageAccountFromConnectionString();
//            var client = storageAccount.CreateCloudBlobClient();

//            var sampleContainer = client.GetContainerReference(containerName);

//            var requestOptions = new BlobRequestOptions() { RetryPolicy = new NoRetry() };
//            await sampleContainer.CreateIfNotExistsAsync(requestOptions, null);

//            return sampleContainer;
//        }
//    }
//}
