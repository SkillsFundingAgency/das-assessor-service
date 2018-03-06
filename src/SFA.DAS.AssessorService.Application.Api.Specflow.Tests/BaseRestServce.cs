using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.consts;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests
{
    public class BaseRestServce
    {
        protected readonly HttpClient HttpClient;

        public BaseRestServce()
        {
           
            var baseAddress = ConfigurationManager.AppSettings[RestParameters.BaseAddress];

            HttpClient = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };

            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
