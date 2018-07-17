using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.AssessorService.EpaoImporter.Const;
using SFA.DAS.AssessorService.EpaoImporter.Interfaces;
using SFA.DAS.AssessorService.EpaoImporter.Logger;
using SFA.DAS.AssessorService.EpaoImporter.services;

namespace SFA.DAS.AssessorService.EpaoImporter
{
    public static class AssessmentOrgsImporterFunction
    {

       

       
        [FunctionName("AssessmentOrgsImporterFunction")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequestMessage req,
            TraceWriter functionLogger, ExecutionContext context)
        {
            var assessmentOrgsLogger = new AggregateLogger(FunctionName.AssessmentOrgsImporter, functionLogger, context);
            try
            {
                assessmentOrgsLogger.LogInfo("HTTP trigger to run Assessment Orgs Importer called");


                // parse query parameter
                string status = req.GetQueryNameValuePairs()
                    .FirstOrDefault(q => string.Compare(q.Key, "status", true) == 0)
                    .Value;

                //var webConfig = ConfigurationHelper.GetConfiguration();
                //assessmentOrgsLogger.LogInfo("Config Received");
                //var deets = webConfig.SqlConnectionString;
                //var x = webConfig.GitPassword;

                if (status == null)
                {
                    // Get request body
                    dynamic data = await req.Content.ReadAsAsync<object>();
                    status = data?.name.ToLower();
                }
                else
                {
                    status = status.ToLower();
                }


                if (status == null || !(status == "teardown" || status == "buildup"))
                {
                    assessmentOrgsLogger.LogInfo($"Valid status not passed in: [{status}]");

                    return req.CreateResponse(HttpStatusCode.BadRequest,
                        "Please pass a status parameter to 'status' as 'teardown' for teardown only, or 'rebuild' for teardown and rebuild");
                }


                // process the details      
               assessmentOrgsLogger.LogInfo($"Valid status passed in: [{status}]");
                
               return req.CreateResponse(HttpStatusCode.OK, status + " completed");
            }
            catch (Exception e)
            {
                assessmentOrgsLogger.LogError("Function Errored", e);
                throw;
            }
        }
    }

    //public static class AssessmentOrgsImporterFunction2
    //{
    //    [FunctionName("AssessmentOrgsImporterFunction2")]
    //    public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "AssessmentOrgsImporterFunction2/name/{name}")]HttpRequestMessage req, string name, TraceWriter log)
    //    {
    //        log.Info("C# HTTP trigger function processed a request.");

    //        // Fetching the name from the path parameter in the request URL
    //        return req.CreateResponse(HttpStatusCode.OK, "Hello " + name);
    //    }
    //}
}
