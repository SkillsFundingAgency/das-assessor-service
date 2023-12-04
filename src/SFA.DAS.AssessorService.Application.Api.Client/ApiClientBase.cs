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

namespace SFA.DAS.AssessorService.Application.Api.Client
{
    public abstract class ApiClientBase : IDisposable
    {
        private readonly ILogger<ApiClientBase> _logger;
        private readonly ITokenService _tokenService;
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        protected ITokenService TokenService => _tokenService;
        protected HttpClient HttpClient => _httpClient;


        protected readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        protected ApiClientBase(HttpClient httpClient, ITokenService tokenService, ILogger<ApiClientBase> logger)
        {
            _tokenService = tokenService;
            _logger = logger;
            _httpClient = httpClient;

            _retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
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

        protected async Task<T> RequestAndDeserialiseAsync<T>(HttpRequestMessage request, string message = null, bool mapNotFoundToNull = false)
        {
            HttpRequestMessage clonedRequest = null;

            var result = await _retryPolicy.ExecuteAsync(async () =>
            {
                clonedRequest = new HttpRequestMessage(request.Method, request.RequestUri);
                clonedRequest.Headers.Add("Accept", "application/json");
                clonedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

                return await _httpClient.SendAsync(clonedRequest);

            });

            if (result.StatusCode == HttpStatusCode.OK)
            {
                // NOTE: Struct values are valid JSON. For example: 'True'
                var json = await result.Content.ReadAsStringAsync();
                return await Task.Factory.StartNew<T>(() => JsonConvert.DeserializeObject<T>(json, _jsonSettings));
            }
            else if (result.StatusCode == HttpStatusCode.NotFound)
            {
                if (!mapNotFoundToNull)
                {
                    if (message == null)
                    {
                        if (!request.RequestUri.IsAbsoluteUri)
                            message = "Could not find " + request.RequestUri;
                        else
                            message = "Could not find " + request.RequestUri.PathAndQuery;
                    }

                    RaiseResponseError(message, clonedRequest, result);
                }
            }
            else 
                RaiseResponseError(clonedRequest, result);

            return default(T);
        }

        protected async Task<U> PostPutRequestWithResponse<T, U>(HttpRequestMessage requestMessage, T model, JsonSerializerSettings setting = null)
        {
            var serializeObject = JsonConvert.SerializeObject(model);
            var content = new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json");
            return await PostPutRequestWithResponse<U>(requestMessage, content, "application/json", setting);
        }

        protected async Task<U> PostPutRequestWithResponse<U>(HttpRequestMessage requestMessage, JsonSerializerSettings setting)
        {
            return await PostPutRequestWithResponse<U>(requestMessage, null, null, setting);
        }

        protected async Task<U> PostPutRequestWithResponse<U>(HttpRequestMessage requestMessage, MultipartFormDataContent formDataContent, JsonSerializerSettings setting)
        {
            return await PostPutRequestWithResponse<U>(requestMessage, formDataContent, null, setting);
        }


        protected async Task<U> PostPutRequestWithResponse<U>(HttpRequestMessage requestMessage, HttpContent content, string mediaType, JsonSerializerSettings setting = null)
        {
            HttpRequestMessage clonedRequest = null;
            var response = await _retryPolicy.ExecuteAsync(async () =>
            {
                clonedRequest = new HttpRequestMessage(requestMessage.Method, requestMessage.RequestUri);
                if (!string.IsNullOrEmpty(mediaType))
                {
                    clonedRequest.Headers.Add("Accept", mediaType);
                }
                clonedRequest.Content = content;
                clonedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

                return await _httpClient.SendAsync(clonedRequest);

            });

            var json = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.OK
                || response.StatusCode == HttpStatusCode.Created
                || response.StatusCode == HttpStatusCode.NoContent)
            {

                return await Task.Factory.StartNew<U>(() => JsonConvert.DeserializeObject<U>(json, setting));
            }
            else
            {
                _logger.LogInformation($"HttpRequestException: Status Code: {response.StatusCode} Body: {json}");
                throw new HttpRequestException(json);
            }
        }


        protected async Task<HttpResponseMessage> RequestToDownloadFile(HttpRequestMessage request, string message = null)
        {
            HttpRequestMessage clonedRequest = null;

            var result = await _retryPolicy.ExecuteAsync(async () =>
            {
                clonedRequest = new HttpRequestMessage(request.Method, request.RequestUri);
                clonedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

                return await _httpClient.SendAsync(clonedRequest);

            });

            if (result.StatusCode == HttpStatusCode.OK)
            {
                return result;
            }

            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                if (message == null)
                {
                    if (!request.RequestUri.IsAbsoluteUri)
                        message = "Could not find " + request.RequestUri;
                    else
                        message = "Could not find " + request.RequestUri.PathAndQuery;
                }

                RaiseResponseError(message, clonedRequest, result);
            }

            RaiseResponseError(clonedRequest, result);

            return result;
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
                clonedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

                return await _httpClient.SendAsync(clonedRequest);

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
                clonedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

                return await _httpClient.SendAsync(clonedRequest);

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
                clonedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

                return await _httpClient.SendAsync(clonedRequest);

            });
            
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new HttpRequestException();
            }
        }

        protected async Task<U> Delete<U>(HttpRequestMessage requestMessage)
        {
            HttpRequestMessage clonedRequest = null;
            var response = await _retryPolicy.ExecuteAsync(async () =>
            {
                clonedRequest = new HttpRequestMessage(requestMessage.Method, requestMessage.RequestUri);
                clonedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

                return await _httpClient.SendAsync(clonedRequest);

            });

            var json = await response.Content.ReadAsStringAsync();
            //var result = await response;
            if (response.StatusCode == HttpStatusCode.OK
                || response.StatusCode == HttpStatusCode.NoContent)
            {

                return await Task.Factory.StartNew<U>(() => JsonConvert.DeserializeObject<U>(json));
            }
            else
            {
                _logger.LogInformation($"HttpRequestException: Status Code: {response.StatusCode} Body: {json}");
                throw new HttpRequestException(json);
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        protected async Task<T> RequestAndDeserialiseAsyncWithAsyncToken<T>(HttpRequestMessage request, string message = null, bool mapNotFoundToNull = false)
        {
            HttpRequestMessage clonedRequest = null;

            var result = await _retryPolicy.ExecuteAsync(async () =>
            {
                clonedRequest = new HttpRequestMessage(request.Method, request.RequestUri);
                clonedRequest.Headers.Add("Accept", "application/json");
                clonedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await _tokenService.GetTokenAsync());

                return await _httpClient.SendAsync(clonedRequest);

            });

            if (result.StatusCode == HttpStatusCode.OK)
            {
                // NOTE: Struct values are valid JSON. For example: 'True'
                var json = await result.Content.ReadAsStringAsync();
                return await Task.Factory.StartNew<T>(() => JsonConvert.DeserializeObject<T>(json, _jsonSettings));
            }
            else if (result.StatusCode == HttpStatusCode.NotFound)
            {
                if (!mapNotFoundToNull)
                {
                    if (message == null)
                    {
                        if (!request.RequestUri.IsAbsoluteUri)
                            message = "Could not find " + request.RequestUri;
                        else
                            message = "Could not find " + request.RequestUri.PathAndQuery;
                    }

                    RaiseResponseError(message, clonedRequest, result);
                }
            }
            else
                RaiseResponseError(clonedRequest, result);

            return default(T);
        }

        protected async Task<U> PostPutRequestWithResponseAsync<T, U>(HttpRequestMessage requestMessage, T model, JsonSerializerSettings setting = null)
        {
            var serializeObject = JsonConvert.SerializeObject(model);
            var content = new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json");
            return await PostPutRequestWithResponseAsync<U>(requestMessage, content, "application/json", setting);
        }

        protected async Task<U> PostPutRequestWithResponseAsync<U>(HttpRequestMessage requestMessage, JsonSerializerSettings setting)
        {
            return await PostPutRequestWithResponseAsync<U>(requestMessage, null, null, setting);
        }

        protected async Task<U> PostPutRequestWithResponseAsync<U>(HttpRequestMessage requestMessage, MultipartFormDataContent formDataContent, JsonSerializerSettings setting)
        {
            return await PostPutRequestWithResponseAsync<U>(requestMessage, formDataContent, null, setting);
        }
        protected async Task<U> PostPutRequestWithResponseAsync<U>(HttpRequestMessage requestMessage, HttpContent content, string mediaType, JsonSerializerSettings setting = null)
        {
            HttpRequestMessage clonedRequest = null;
            var response = await _retryPolicy.ExecuteAsync(async () =>
            {
                clonedRequest = new HttpRequestMessage(requestMessage.Method, requestMessage.RequestUri);
                if (!string.IsNullOrEmpty(mediaType))
                {
                    clonedRequest.Headers.Add("Accept", mediaType);
                }
                clonedRequest.Content = content;
                clonedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await _tokenService.GetTokenAsync());

                return await _httpClient.SendAsync(clonedRequest);

            });

            var json = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.OK
                || response.StatusCode == HttpStatusCode.Created
                || response.StatusCode == HttpStatusCode.NoContent)
            {

                return await Task.Factory.StartNew<U>(() => JsonConvert.DeserializeObject<U>(json, setting));
            }
            else
            {
                _logger.LogInformation($"HttpRequestException: Status Code: {response.StatusCode} Body: {json}");
                throw new HttpRequestException(json);
            }
        }


        protected async Task<HttpResponseMessage> RequestToDownloadFileAsync(HttpRequestMessage request, string message = null)
        {
            HttpRequestMessage clonedRequest = null;

            var result = await _retryPolicy.ExecuteAsync(async () =>
            {
                clonedRequest = new HttpRequestMessage(request.Method, request.RequestUri);
                clonedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await _tokenService.GetTokenAsync());

                return await _httpClient.SendAsync(clonedRequest);

            });

            if (result.StatusCode == HttpStatusCode.OK)
            {
                return result;
            }

            if (result.StatusCode == HttpStatusCode.NotFound)
            {
                if (message == null)
                {
                    if (!request.RequestUri.IsAbsoluteUri)
                        message = "Could not find " + request.RequestUri;
                    else
                        message = "Could not find " + request.RequestUri.PathAndQuery;
                }

                RaiseResponseError(message, clonedRequest, result);
            }

            RaiseResponseError(clonedRequest, result);

            return result;
        }

        protected async Task PostPutRequestAsync<T>(HttpRequestMessage requestMessage, T model)
        {
            var serializeObject = JsonConvert.SerializeObject(model);
            HttpRequestMessage clonedRequest = null;
            var response = await _retryPolicy.ExecuteAsync(async () =>
            {
                clonedRequest = new HttpRequestMessage(requestMessage.Method, requestMessage.RequestUri);
                clonedRequest.Headers.Add("Accept", "application/json");
                clonedRequest.Content = new StringContent(serializeObject,
                    System.Text.Encoding.UTF8, "application/json");
                clonedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await _tokenService.GetTokenAsync());

                return await _httpClient.SendAsync(clonedRequest);

            });

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new HttpRequestException();
            }
        }


        protected async Task PostPutRequestAsync(HttpRequestMessage requestMessage)
        {
            HttpRequestMessage clonedRequest = null;
            var response = await _retryPolicy.ExecuteAsync(async () =>
            {
                clonedRequest = new HttpRequestMessage(requestMessage.Method, requestMessage.RequestUri);
                clonedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await _tokenService.GetTokenAsync());

                return await _httpClient.SendAsync(clonedRequest);

            });

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new HttpRequestException();
            }
        }

        protected async Task DeleteAsync(HttpRequestMessage requestMessage)
        {
            HttpRequestMessage clonedRequest = null;
            var response = await _retryPolicy.ExecuteAsync(async () =>
            {
                clonedRequest = new HttpRequestMessage(requestMessage.Method, requestMessage.RequestUri);
                clonedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await _tokenService.GetTokenAsync());

                return await _httpClient.SendAsync(clonedRequest);

            });

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new HttpRequestException();
            }
        }

        protected async Task<U> DeleteAsync<U>(HttpRequestMessage requestMessage)
        {
            HttpRequestMessage clonedRequest = null;
            var response = await _retryPolicy.ExecuteAsync(async () =>
            {
                clonedRequest = new HttpRequestMessage(requestMessage.Method, requestMessage.RequestUri);
                clonedRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await _tokenService.GetTokenAsync());

                return await _httpClient.SendAsync(clonedRequest);

            });

            var json = await response.Content.ReadAsStringAsync();
            //var result = await response;
            if (response.StatusCode == HttpStatusCode.OK
                || response.StatusCode == HttpStatusCode.NoContent)
            {

                return await Task.Factory.StartNew<U>(() => JsonConvert.DeserializeObject<U>(json));
            }
            else
            {
                _logger.LogInformation($"HttpRequestException: Status Code: {response.StatusCode} Body: {json}");
                throw new HttpRequestException(json);
            }
        }

    }
}