using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using SFA.DAS.AssessorService.Application.Api.Client.Exceptions;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public abstract class ApiClientBase : IDisposable
    {
        protected readonly ITokenService TokenService;
        private readonly ILogger<ApiClientBase> _logger;
        protected HttpClient HttpClient;

        private readonly RetryPolicy<HttpResponseMessage> _retryPolicy;

        protected readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        protected ApiClientBase(string baseUri, ITokenService tokenService, ILogger<ApiClientBase> logger)
        {
            TokenService = tokenService;
            _logger = logger;

            HttpClient = new HttpClient { BaseAddress = new Uri($"{baseUri}") };

            _retryPolicy = HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                    retryAttempt)));
        }

        protected static void RaiseResponseError(string message, HttpRequestMessage failedRequest, HttpResponseMessage failedResponse)
        {
            if (failedResponse.StatusCode == HttpStatusCode.NotFound)
            {
                throw new EntityNotFoundException(message, CreateRequestException(failedRequest, failedResponse));
            }

            throw CreateRequestException(failedRequest, failedResponse);
        }

        protected static void RaiseResponseError(HttpRequestMessage failedRequest, HttpResponseMessage failedResponse)
        {
            throw CreateRequestException(failedRequest, failedResponse);
        }

        private static HttpRequestException CreateRequestException(HttpRequestMessage failedRequest, HttpResponseMessage failedResponse)
        {
            return new HttpRequestException(
                string.Format($"The Client request for {{0}} {{1}} failed. Response Status: {{2}}, Response Body: {{3}}",
                    failedRequest.Method.ToString().ToUpperInvariant(),
                    failedRequest.RequestUri,
                    (int)failedResponse.StatusCode,
                    failedResponse.Content.ReadAsStringAsync().Result));
        }

        protected async Task<T> RequestAndDeserialiseAsync<T>(HttpRequestMessage request, string message = null) where T : class
        {
            HttpRequestMessage clonedRequest = null;

            var result = await _retryPolicy.ExecuteAsync(async () =>
            {
                clonedRequest = new HttpRequestMessage(request.Method, request.RequestUri);
                clonedRequest.Headers.Add("Accept", "application/json");
                clonedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TokenService.GetToken());

                return await HttpClient.SendAsync(clonedRequest);

            });

            if (result.StatusCode == HttpStatusCode.OK)
            {
                var json = await result.Content.ReadAsStringAsync();
                return await Task.Factory.StartNew<T>(() => JsonConvert.DeserializeObject<T>(json, JsonSettings));
            }

            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                if (message == null)
                {
                    message = "Could not find " + request.RequestUri.PathAndQuery;
                }

                RaiseResponseError(message, clonedRequest, result);
            }

            RaiseResponseError(clonedRequest, result);

            return null;

        }

        protected async Task<U> PostPutRequestWithResponse<T, U>(HttpRequestMessage requestMessage, T model)
        {
            var serializeObject = JsonConvert.SerializeObject(model);
          
            HttpRequestMessage clonedRequest = null;
            var response = await _retryPolicy.ExecuteAsync(async () =>
            {
                clonedRequest = new HttpRequestMessage(requestMessage.Method, requestMessage.RequestUri);
                clonedRequest.Headers.Add("Accept", "application/json");
                clonedRequest.Content = new StringContent(serializeObject,
                    System.Text.Encoding.UTF8, "application/json");
                clonedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TokenService.GetToken());

                return await HttpClient.SendAsync(clonedRequest);

            });

            var json = await response.Content.ReadAsStringAsync();
            //var result = await response;
            if (response.StatusCode == HttpStatusCode.OK
                || response.StatusCode == HttpStatusCode.Created
                || response.StatusCode == HttpStatusCode.NoContent)
            {
                return await Task.Factory.StartNew<U>(() => JsonConvert.DeserializeObject<U>(json, JsonSettings));
            }
            else
            {
                _logger.LogInformation($"HttpRequestException: Status Code: {response.StatusCode} Body: {json}");
                throw new HttpRequestException(json);
            }
        }

        protected async Task PostPutRequest<T>(HttpRequestMessage requestMessage, T model)
        {
            var serializeObject = JsonConvert.SerializeObject(model);                     
            HttpRequestMessage clonedRequest = null;
            var response = await _retryPolicy.ExecuteAsync(async () =>
            {
                clonedRequest = new HttpRequestMessage(requestMessage.Method, requestMessage.RequestUri);
                clonedRequest.Headers.Add("Accept", "application/json");
                clonedRequest.Content = new StringContent(serializeObject,
                    System.Text.Encoding.UTF8, "application/json");
                clonedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TokenService.GetToken());

                return await HttpClient.SendAsync(clonedRequest);

            });
           
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new HttpRequestException();
            }
        }

        protected async Task PostPutRequest(HttpRequestMessage requestMessage)
        {           
            HttpRequestMessage clonedRequest = null;
            var response = await _retryPolicy.ExecuteAsync(async () =>
            {
                clonedRequest = new HttpRequestMessage(requestMessage.Method, requestMessage.RequestUri);               
                clonedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TokenService.GetToken());

                return await HttpClient.SendAsync(clonedRequest);

            });        

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new HttpRequestException();
            }
        }

        protected async Task Delete(HttpRequestMessage requestMessage)
        {
            HttpRequestMessage clonedRequest = null;
            var response = await _retryPolicy.ExecuteAsync(async () =>
            {
                clonedRequest = new HttpRequestMessage(requestMessage.Method, requestMessage.RequestUri);
                clonedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TokenService.GetToken());

                return await HttpClient.SendAsync(clonedRequest);

            });
            
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new HttpRequestException();
            }
        }

        public void Dispose()
        {
            HttpClient?.Dispose();
        }
    }
}