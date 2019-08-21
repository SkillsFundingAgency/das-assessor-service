using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;

namespace SFA.DAS.AssessorService.ExternalApiDataSync.Infrastructure
{
    public static class ConfigurationHelper
    {
        private static IWebConfiguration _webConfiguration;
        private static readonly object _lock = new object();

        public static IWebConfiguration GetConfiguration()
        {
            lock (_lock)
            {
                if (_webConfiguration == null)
                {
                    var serviceName = "SFA.DAS.AssessorService";
                    var version = "1.0";

                    var connection = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("ConfigurationStorageConnectionString"));
                    var tableClient = connection.CreateCloudTableClient();
                    var table = tableClient.GetTableReference("Configuration");

                    var operation = TableOperation.Retrieve(Environment.GetEnvironmentVariable("EnvironmentName"), $"{serviceName}_{version}");
                    var result = table.ExecuteAsync(operation).GetAwaiter().GetResult();
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

        public static string GetEnvironmentName()
        {
            return Environment.GetEnvironmentVariable("EnvironmentName");
        }
    }
}