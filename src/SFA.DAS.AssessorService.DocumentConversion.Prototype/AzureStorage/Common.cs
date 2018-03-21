using System;
using Microsoft.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;

namespace SFA.DAS.AssessorService.DocumentConversion.Prototype.AzureStorage
{
    public  class Common
    {
        private readonly IConfiguration _configuration;

        public Common(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Validates the connection string information in app.config and throws an exception if it looks like 
        /// the user hasn't updated this to valid values. 
        /// </summary>
        /// <returns>CloudStorageAccount object</returns>
        public  CloudStorageAccount CreateStorageAccountFromConnectionString()
        {
            CloudStorageAccount storageAccount;
            const string Message = "Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.";

            try
            {
                var storageAccountName = _configuration["StorageConnectionString"];
                storageAccount = CloudStorageAccount.Parse(storageAccountName);
            }
            catch (FormatException)
            {
                Console.WriteLine(Message);
                Console.ReadLine();
                throw;
            }
            catch (ArgumentException)
            {
                Console.WriteLine(Message);
                Console.ReadLine();
                throw;
            }

            return storageAccount;
        }
    }
}
