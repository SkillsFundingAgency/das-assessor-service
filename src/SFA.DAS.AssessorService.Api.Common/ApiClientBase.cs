using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Polly;
using Polly.Retry;
using SFA.DAS.AssessorService.Api.Common.Exceptions;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Api.Common
{
    public abstract class ApiClientBase : IDisposable
    {
        private readonly ILogger<ApiClientBase> _logger;
        private readonly HttpClient _httpClient;
        private readonly ResiliencePipeline<HttpResponseMessage> _resiliencePipeline;

        protected readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        protected ApiClientBase(HttpClient httpClient, ILogger<ApiClientBase> logger)
        {
            _logger = logger;
            _httpClient = httpClient;

            var options = new RetryStrategyOptions<HttpResponseMessage>
            {
                ShouldHandle = async args => await PollyUtils.HandleTransientHttpError(args.Outcome),
                MaxRetryAttempts = 3,
                DelayGenerator = static opt =>
                    ValueTask.FromResult((TimeSpan?)TimeSpan.FromSeconds(Math.Pow(2, opt.AttemptNumber)))
            };

            _resiliencePipeline = new ResiliencePipelineBuilder<HttpResponseMessage>()
                .AddRetry(options)
                .Build();
        }

        protected static HttpRequestException RaiseResponseError(string message, HttpRequestMessage failedRequest, HttpResponseMessage failedResponse)
        {
            if (failedResponse.StatusCode == HttpStatusCode.NotFound)
            {
                throw new EntityNotFoundException(message, CreateRequestException(failedRequest, failedResponse));
            }

            return CreateRequestException(failedRequest, failedResponse);
        }

        protected static HttpRequestException RaiseResponseError(HttpRequestMessage failedRequest, HttpResponseMessage failedResponse)
        {
            return CreateRequestException(failedRequest, failedResponse);
        }

        private static HttpRequestException CreateRequestException(HttpRequestMessage failedRequest, HttpResponseMessage failedResponse)
        {
            return new HttpRequestException(
                string.Format($"The Client request for {{0}} {{1}} failed. Response Status: {{2}}, Response Body: {{3}}",
                    failedRequest.Method.ToString().ToUpperInvariant(),
                    failedRequest.RequestUri,
                    (int)failedResponse.StatusCode,
                    failedResponse.Content?.ReadAsStringAsync().Result));
        }

        protected async Task<T> RequestAndDeserialiseAsync<T>(HttpRequestMessage requestMessage, string message = null, bool mapNotFoundToNull = false)
        {
            HttpRequestMessage clonedRequest = null;

            var response = await _resiliencePipeline.ExecuteAsync(async token =>
            {
                clonedRequest = new HttpRequestMessage(requestMessage.Method, requestMessage.RequestUri);
                return await _httpClient.SendAsync(clonedRequest, token);
            }, CancellationToken.None);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var json = await response.Content.ReadAsStringAsync();

                if (IsValidJson(json))
                {
                    return JsonConvert.DeserializeObject<T>(json, _jsonSettings);
                }
            }
            else if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return default;
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                if (mapNotFoundToNull)
                    return default;

                if (message == null)
                {
                    if (!requestMessage.RequestUri.IsAbsoluteUri)
                        message = "Could not find " + requestMessage.RequestUri;
                    else
                        message = "Could not find " + requestMessage.RequestUri.PathAndQuery;
                }

                throw RaiseResponseError(message, clonedRequest, response);
            }
            
            throw RaiseResponseError(clonedRequest, response);
        }

        protected async Task<T> PostPutRequestWithResponseAsync<M, T>(HttpRequestMessage requestMessage, M model, JsonSerializerSettings setting = null)
        {
            var serializeObject = JsonConvert.SerializeObject(model);
            var content = new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json");
            return await PostPutRequestWithResponseAsync<T>(requestMessage, content, "application/json", setting);
        }

        protected async Task<T> PostPutRequestWithResponseAsync<T>(HttpRequestMessage requestMessage, JsonSerializerSettings setting)
        {
            return await PostPutRequestWithResponseAsync<T>(requestMessage, null, null, setting);
        }

        protected async Task<T> PostPutRequestWithResponseAsync<T>(HttpRequestMessage requestMessage, MultipartFormDataContent formDataContent, JsonSerializerSettings setting)
        {
            return await PostPutRequestWithResponseAsync<T>(requestMessage, formDataContent, null, setting);
        }

        protected async Task<T> PostPutRequestWithResponseAsync<T>(HttpRequestMessage requestMessage, HttpContent content, string mediaType, JsonSerializerSettings setting = null)
        {
            HttpRequestMessage clonedRequest = null;
            var response = await _resiliencePipeline.ExecuteAsync(async token =>
            {
                clonedRequest = new HttpRequestMessage(requestMessage.Method, requestMessage.RequestUri);
                if (!string.IsNullOrEmpty(mediaType))
                {
                    clonedRequest.Headers.Add("Accept", mediaType);
                }
                clonedRequest.Content = content;

                return await _httpClient.SendAsync(clonedRequest, token);
            }, CancellationToken.None);

            if (response.StatusCode == HttpStatusCode.OK
                || response.StatusCode == HttpStatusCode.Created
                || response.StatusCode == HttpStatusCode.NoContent)
            {
                var json = await response.Content.ReadAsStringAsync();

                if (IsValidJson(json))
                {
                    return JsonConvert.DeserializeObject<T>(json, _jsonSettings);
                }
            }

            throw RaiseResponseError(clonedRequest, response);
        }

        protected async Task<HttpResponseMessage> RequestToDownloadFileAsync(HttpRequestMessage requestMessage, string message = null)
        {
            HttpRequestMessage clonedRequest = null;
            var response = await _resiliencePipeline.ExecuteAsync(async token =>
            {
                clonedRequest = new HttpRequestMessage(requestMessage.Method, requestMessage.RequestUri);
                return await _httpClient.SendAsync(clonedRequest, token);
            }, CancellationToken.None);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response;
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                if (message == null)
                {
                    if (!requestMessage.RequestUri.IsAbsoluteUri)
                        message = "Could not find " + requestMessage.RequestUri;
                    else
                        message = "Could not find " + requestMessage.RequestUri.PathAndQuery;
                }

                throw RaiseResponseError(message, clonedRequest, response);
            }

            throw RaiseResponseError(clonedRequest, response);;
        }

        protected async Task PostPutRequestAsync<T>(HttpRequestMessage requestMessage, T model)
        {
            HttpRequestMessage clonedRequest = null;
            var response = await _resiliencePipeline.ExecuteAsync(async token =>
            {
                clonedRequest = new HttpRequestMessage(requestMessage.Method, requestMessage.RequestUri);
                clonedRequest.Content = new StringContent(JsonConvert.SerializeObject(model), System.Text.Encoding.UTF8, "application/json");
                return await _httpClient.SendAsync(clonedRequest, token);
            }, CancellationToken.None);
           
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw RaiseResponseError(clonedRequest, response);
            }
        }

        protected async Task PostPutRequestAsync(HttpRequestMessage requestMessage)
        {
            if (requestMessage.Method != HttpMethod.Post && requestMessage.Method != HttpMethod.Put)
                throw new ArgumentException("The HttpMethod must be either Post or Put");

            HttpRequestMessage clonedRequest = null;
            var response = await _resiliencePipeline.ExecuteAsync(async token =>
            {
                clonedRequest = new HttpRequestMessage(requestMessage.Method, requestMessage.RequestUri);
                return await _httpClient.SendAsync(clonedRequest, token);
            }, CancellationToken.None);

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw RaiseResponseError(clonedRequest, response);
            }
        }

        protected async Task DeleteAsync(HttpRequestMessage requestMessage)
        {
            if (requestMessage.Method != HttpMethod.Delete)
                throw new ArgumentException("The HttpMethod must be Delete");
            
            HttpRequestMessage clonedRequest = null;
            var response = await _resiliencePipeline.ExecuteAsync(async token =>
            {
                clonedRequest = new HttpRequestMessage(requestMessage.Method, requestMessage.RequestUri);
                return await _httpClient.SendAsync(clonedRequest, token);
            }, CancellationToken.None);
            
            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw RaiseResponseError(clonedRequest, response);
            }
        }

        protected async Task<T> DeleteAsync<T>(HttpRequestMessage requestMessage)
        {
            if (requestMessage.Method != HttpMethod.Delete)
                throw new ArgumentException("The HttpMethod must be Delete");

            HttpRequestMessage clonedRequest = null;
            var response = await _resiliencePipeline.ExecuteAsync(async token =>
            {
                clonedRequest = new HttpRequestMessage(requestMessage.Method, requestMessage.RequestUri);
                return await _httpClient.SendAsync(clonedRequest, token);
            }, CancellationToken.None);

            if (response.StatusCode == HttpStatusCode.OK || 
                response.StatusCode == HttpStatusCode.NoContent)
            {
                var json = await response.Content.ReadAsStringAsync();

                if (IsValidJson(json))
                {
                    return JsonConvert.DeserializeObject<T>(json, _jsonSettings);
                }
            }

            throw RaiseResponseError(clonedRequest, response);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _httpClient?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool IsValidJson(string jsonString)
        {
            try
            {
                JToken.Parse(jsonString);
                return true;
            }
            catch (JsonReaderException)
            {
                _logger.LogError($"Unable to parse Json object: {jsonString}");
                return false;
            }
        }
    }
}