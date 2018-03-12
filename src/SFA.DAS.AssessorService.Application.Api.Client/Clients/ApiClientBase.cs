using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SFA.DAS.AssessorService.Application.Api.Client.Exceptions;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public abstract class ApiClientBase : IDisposable
    {
        protected readonly ITokenService TokenService;
        private readonly ILogger<ApiClientBase> _logger;
        protected readonly HttpClient HttpClient;

        protected readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        protected ApiClientBase(string baseUri, ITokenService tokenService, ILogger<ApiClientBase> logger)
        {
            TokenService = tokenService;
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
            
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TokenService.GetToken());

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

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TokenService.GetToken());

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
                    _logger.LogInformation($"HttpRequestException: Status COde: {response.StatusCode} Body: {json}");
                    throw new HttpRequestException();
                }
            }
        }


        //protected bool Exists(HttpRequestMessage request)
        //{
        //    using (var response = HttpClient.SendAsync(request))
        //    {
        //        var result = response.Result;
        //        if (result.StatusCode == HttpStatusCode.NoContent)
        //        {
        //            return true;
        //        }
        //        if (result.StatusCode == HttpStatusCode.NotFound)
        //        {
        //            return false;
        //        }

        //        RaiseResponseError(request, result);
        //    }

        //    return false;
        //}

        //protected async Task<bool> ExistsAsync(HttpRequestMessage request)
        //{
        //    using (var response = HttpClient.SendAsync(request))
        //    {
        //        var result = await response;
        //        if (result.StatusCode == HttpStatusCode.NoContent)
        //        {
        //            return true;
        //        }
        //        if (result.StatusCode == HttpStatusCode.NotFound)
        //        {
        //            return false;
        //        }

        //        RaiseResponseError(request, result);
        //    }

        //    return false;
        //}

        protected async Task PostPutRequest<T>(HttpRequestMessage requestMessage, T model)
        {
            var serializeObject = JsonConvert.SerializeObject(model);
            requestMessage.Content = new StringContent(serializeObject,
                System.Text.Encoding.UTF8, "application/json");

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TokenService.GetToken());

            var response = await HttpClient.SendAsync(requestMessage);

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new HttpRequestException();
            }
        }

        protected async Task PostPutRequest(HttpRequestMessage requestMessage)
        {
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TokenService.GetToken());

            var response = await HttpClient.SendAsync(requestMessage);

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new HttpRequestException();
            }
        }

        protected async Task Delete(HttpRequestMessage requestMessage)
        {
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", TokenService.GetToken());

            var response = await HttpClient.SendAsync(requestMessage);

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new HttpRequestException();
            }
        }

        //internal T RequestAndDeserialise<T>(HttpRequestMessage request, string missing = null) where T : class

        //{
        //    request.Headers.Add("Accept", "application/json");

        //    using (var response = HttpClient.SendAsync(request))
        //    {
        //        var result = response.Result;
        //        if (result.StatusCode == HttpStatusCode.OK)
        //        {
        //            return JsonConvert.DeserializeObject<T>(result.Content.ReadAsStringAsync().Result, _jsonSettings);
        //        }
        //        if (result.StatusCode == HttpStatusCode.NotFound)
        //        {
        //            RaiseResponseError(missing, request, result);
        //        }

        //        RaiseResponseError(request, result);
        //    }

        //    return null;
        //}

        public void Dispose()
        {
            HttpClient?.Dispose();
        }
    }
}