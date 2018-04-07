using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using LogLevel = NLog.LogLevel;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow
{
    public static class PrintFunctionFlow
    {
        [FunctionName("PrintFunctionFlow")]
        public static void Run([TimerTrigger("*/5 * * * * *")] TimerInfo myTimer, TraceWriter functionLogger,
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
                //LogError("Function Errored", e);
                throw;
            }
        }

    }
}