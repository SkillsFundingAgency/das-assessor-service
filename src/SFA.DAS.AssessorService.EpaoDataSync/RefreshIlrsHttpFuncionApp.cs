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
    public static class RefreshIlrsHttpFuncionApp
    {
        [FunctionName("RefreshIlrsFromProviderEvents")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequestMessage req,
            TraceWriter log, 
            ExecutionContext executionContext)
        {
            var eventTime = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "sinceTime", StringComparison.OrdinalIgnoreCase) == 0)
                .Value;

            if (string.IsNullOrEmpty(eventTime))
                return req.CreateResponse(HttpStatusCode.BadRequest,
                    "Please pass an sinceTime on the query string - <domain-name>/api/RefreshIlrsFromProviderEvents?sinceTime=2018-09-09T11:07:19");

            Bootstrapper.StartUp(log, executionContext);
            var ilrRefresherService = Bootstrapper.Container.GetInstance<IIlrsRefresherService>();
            await ilrRefresherService.UpdateIlRsTable(eventTime);
            
            return req.CreateResponse(HttpStatusCode.OK, $"Finished updating ILRS table.");

        }
    }
}
