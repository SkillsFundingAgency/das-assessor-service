using System;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using SFA.DAS.AssessorService.PrintFunctionProcessFlow.Logger;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.AzureStorage
{
    public class Common
    {
        private readonly IAggregateLogger _aggregateLogger;

        public Common(
            IAggregateLogger aggregateLogger)
        {

            _aggregateLogger = aggregateLogger;
        }

        public CloudStorageAccount CreateStorageAccountFromConnectionString()
        {
            CloudStorageAccount storageAccount;
            const string message = "Invalid storage account information provided. Please confirm the AccountName and AccountKey are valid in the app.config file - then restart the sample.";

            try
            {
                var storageAccountName = CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString");
                storageAccount = CloudStorageAccount.Parse(storageAccountName);
            }
            catch (FormatException e)
            {
                _aggregateLogger.LogError(message, e);
                throw;
            }
            catch (ArgumentException e)
            {
                _aggregateLogger.LogError(message, e);
                throw;
            }

            return storageAccount;
        }
    }
}
