using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.Application.Api.Client.Exceptions;

namespace SFA.DAS.AssessorService.Application.Api.Client.Azure
{
    public abstract class AzureApiClientBase : IDisposable
    {
        private readonly ILogger<AzureApiClientBase> _logger;
        protected readonly IAzureTokenService _azureTokenService;
        protected readonly HttpClient HttpClient;

        protected readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        protected AzureApiClientBase(string baseUri, IAzureTokenService tokenService, ILogger<AzureApiClientBase> logger)
        {
            _azureTokenService = tokenService;
            _logger = logger;
            HttpClient = new HttpClient { BaseAddress = new Uri($"{baseUri}") };
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
            request.Headers.Add("Accept", "application/json");

            request.Headers.Authorization = new AuthenticationHeaderValue("SharedAccessSignature", _azureTokenService.GetToken());

            using (var response = HttpClient.SendAsync(request))
            {
                var result = await response;
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    var json = await result.Content.ReadAsStringAsync();
                    return await Task.Factory.StartNew<T>(() => JsonConvert.DeserializeObject<T>(json, _jsonSettings));
                }
                if (result.StatusCode == HttpStatusCode.NotFound)
                {
                    if (message == null)
                    {
                        message = "Could not find " + request.RequestUri.PathAndQuery;
                    }

                    RaiseResponseError(message, request, result);
                }

                RaiseResponseError(request, result);
            }

            return null;
        }

        protected async Task<U> PostPutRequestWithResponse<T, U>(HttpRequestMessage requestMessage, T model)
        {
            var serializeObject = JsonConvert.SerializeObject(model);
            requestMessage.Content = new StringContent(serializeObject,
                System.Text.Encoding.UTF8, "application/json");

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("SharedAccessSignature", _azureTokenService.GetToken());

            using (var response = await HttpClient.SendAsync(requestMessage))
            {
                var json = await response.Content.ReadAsStringAsync();
                //var result = await response;
                if (response.StatusCode == HttpStatusCode.OK
                    || response.StatusCode == HttpStatusCode.Created
                    || response.StatusCode == HttpStatusCode.NoContent)
                {
                    return await Task.Factory.StartNew<U>(() => JsonConvert.DeserializeObject<U>(json, _jsonSettings));
                }
                else
                {
                    _logger.LogInformation($"HttpRequestException: Status Code: {response.StatusCode} Body: {json}");
                    throw new HttpRequestException(json);
                }
            }
        }

        protected async Task PostPutRequest<T>(HttpRequestMessage requestMessage, T model)
        {
            var serializeObject = JsonConvert.SerializeObject(model);
            requestMessage.Content = new StringContent(serializeObject,
                System.Text.Encoding.UTF8, "application/json");

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("SharedAccessSignature", _azureTokenService.GetToken());

            var response = await HttpClient.SendAsync(requestMessage);

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new HttpRequestException();
            }
        }

        protected async Task PostPutRequest(HttpRequestMessage requestMessage)
        {
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("SharedAccessSignature", _azureTokenService.GetToken());

            var response = await HttpClient.SendAsync(requestMessage);

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new HttpRequestException();
            }
        }

        protected async Task Delete(HttpRequestMessage requestMessage)
        {
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("SharedAccessSignature", _azureTokenService.GetToken());

            var response = await HttpClient.SendAsync(requestMessage);

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
