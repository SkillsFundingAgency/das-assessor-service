using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow
{
    public static class PrintFunctionFlow
    {
        [FunctionName("PrintFunctionFlow")]
        public static void Run([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, TraceWriter functionLogger,
            ExecutionContext context)
        {
            try
            {
                new Bootstrapper().StartUp(functionLogger, context);

                var command = Bootstrapper.Container.GetInstance<Command>();
                command.Execute().GetAwaiter().GetResult();

                //using (var client = new HttpClient())
                //{
                //    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                //    client.DefaultRequestHeaders.Add("accept", "application/json");

                //    var response = client.PostAsync($"{webConfig.ClientApiAuthentication.ApiBaseAddress}/api/v1/register-import", new StringContent("")).Result;
                //    var content = response.Content.ReadAsStringAsync().Result;

                //    if (response.IsSuccessStatusCode)
                //    {
                //        LogInfo($"Status code returned: {response.StatusCode}. Content: {content}");
                //    }
                //    else
                //    {
                //        LogInfo($"Status code returned: {response.StatusCode}. Content: {content}");
                //    }
                //}
            }
            catch (Exception e)
            {
                functionLogger.Error("Function Errored", e);
                throw;
            }
        }
    }
}