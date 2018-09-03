using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using SFA.DAS.AssessorService.Application.Api.Specflow.Tests.consts;
using SFA.DAS.AssessorService.Settings;
using Configuration = SFA.DAS.AssessorService.Application.Api.Specflow.Tests.consts.Configuration;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests
{
    public class BaseRestServce
    {
        protected readonly HttpClient HttpClient;

        public BaseRestServce(IWebConfiguration webConfiguration)
        {       
            HttpClient = new HttpClient
            {
                BaseAddress = new Uri(webConfiguration.ClientApiAuthentication.ApiBaseAddress)
            };

            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
