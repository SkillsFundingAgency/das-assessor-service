using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.AssessorService.EpaoDataSync.Domain;
using SFA.DAS.AssessorService.EpaoDataSync.Startup;

namespace SFA.DAS.AssessorService.EpaoDataSync
{
    public static class RefreshIlrsFuncionApp
    {
        [FunctionName("RefreshIlrsFromProviderEvents")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequestMessage req,
            TraceWriter log, 
            ExecutionContext executionContext)
        {
            var tableName = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "tablename", StringComparison.OrdinalIgnoreCase) == 0)
                .Value;

            if (string.IsNullOrEmpty(tableName))
                return req.CreateResponse(HttpStatusCode.BadRequest,
                    "Please pass a Ilrs table name on the query string ");

            Bootstrapper.StartUp(log, executionContext);
            var ilrRefresherService = Bootstrapper.Container.GetInstance<IIlrsRefresherService>();
            var response = await ilrRefresherService.UpdateIlRsTable(tableName);
            
            return req.CreateResponse(HttpStatusCode.OK, $"Number of rows updated {response}");

        }
    }
}
