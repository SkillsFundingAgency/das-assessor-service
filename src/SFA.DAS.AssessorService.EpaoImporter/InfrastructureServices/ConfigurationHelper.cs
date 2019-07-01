using System;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.EpaoImporter.InfrastructureServices
{
    public static class ConfigurationHelper
    {
        private static IWebConfiguration _webConfiguration;
        private static readonly Object _lock = new Object();

        public static IWebConfiguration GetConfiguration()
        {
            lock (_lock)
            {
                if (_webConfiguration == null)
                {
                    var serviceName = "SFA.DAS.AssessorService";
                    var version = "1.0";

                    var connection = CloudStorageAccount.Parse(
                         Environment.GetEnvironmentVariable("ConfigurationStorageConnectionString"));
                    var tableClient = connection.CreateCloudTableClient();
                    var table = tableClient.GetTableReference("Configuration");

                    var operation = TableOperation.Retrieve(
                        Environment.GetEnvironmentVariable("EnvironmentName"),
                        $"{serviceName}_{version}");
                    var result = table.ExecuteAsync(operation).Result;
                    if (result.Result is DynamicTableEntity dynResult)
                    {
                        var data = dynResult.Properties["Data"].StringValue;
                        _webConfiguration = JsonConvert.DeserializeObject<WebConfiguration>(data);
                    }
                    else
                    {
                        throw new ApplicationException("Cannot Deserialise Configuration Entry");
                    }
                }
            }

            return _webConfiguration;
        }
    }
}