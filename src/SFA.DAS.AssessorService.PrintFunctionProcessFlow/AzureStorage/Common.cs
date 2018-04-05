using System;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.AzureStorage
{
    public  class Common
    {
        private readonly IConfiguration _configuration;

        public Common(IConfiguration configuration)
        {
            _configuration = configuration;
        }
       
        public  CloudStorageAccount CreateStorageAccountFromConnectionString()
        {
            CloudStorageAccount storageAccount;
            const string message = "Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.";

            try
            {
                var storageAccountName = _configuration["StorageConnectionString"];
                storageAccount = CloudStorageAccount.Parse(storageAccountName);
            }
            catch (FormatException)
            {
                Console.WriteLine(message);
                Console.ReadLine();
                throw;
            }
            catch (ArgumentException)
            {
                Console.WriteLine(message);
                Console.ReadLine();
                throw;
            }

            return storageAccount;
        }
    }
}
