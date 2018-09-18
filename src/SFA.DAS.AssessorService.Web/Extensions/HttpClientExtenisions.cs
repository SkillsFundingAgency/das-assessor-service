using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;

namespace SFA.DAS.AssessorService.Web.Extensions
{
    public static class HttpClientExtenisions
    {
        public static void AddHttpClientServiceWithRetry<T, U>(this IServiceCollection services, string baseAddress)
            where T : class
            where U : ApiClientBase, T
        {
            services.AddHttpClient<T, U>(config =>
                {
                    config.BaseAddress = new Uri(baseAddress);
                    config.DefaultRequestHeaders.Add("Accept", "Application/json");
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))  //Set lifetime to five minutes
                .AddPolicyHandler(GetRetryPolicy());
        }

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                    retryAttempt)));
        }
    }
}
