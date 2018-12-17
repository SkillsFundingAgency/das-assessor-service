using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SFA.DAS.AssessorService.ExternalApis
{
    public abstract class ApiClientBase : IDisposable
    {
        protected readonly HttpClient _httpClient;

        protected readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        protected ApiClientBase(string baseUri = null)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(baseUri ?? "http://das-prd-apprenticeshipinfoservice.cloudapp.net") };
        }

        protected ApiClientBase(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        protected ApiClientBase(string baseUri , string bearerToken, string apiVersion = null)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUri)
            };
            if (!string.IsNullOrEmpty(bearerToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", bearerToken);
            }
            if (!string.IsNullOrEmpty(apiVersion))
            {
                _httpClient.DefaultRequestHeaders.Add("api-version", apiVersion);
            }
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

            using (var response = _httpClient.SendAsync(request))
            {
                var result = await response;
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    var json = await result.Content.ReadAsStringAsync();
                    return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<T>(json, _jsonSettings));
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

        protected bool Exists(HttpRequestMessage request)
        {
            using (var response = _httpClient.SendAsync(request))
            {
                var result = response.Result;
                if (result.StatusCode == HttpStatusCode.NoContent)
                {
                    return true;
                }
                if (result.StatusCode == HttpStatusCode.NotFound)
                {
                    return false;
                }

                RaiseResponseError(request, result);
            }

            return false;
        }

        protected async Task<bool> ExistsAsync(HttpRequestMessage request)
        {
            using (var response = _httpClient.SendAsync(request))
            {
                var result = await response;
                if (result.StatusCode == HttpStatusCode.NoContent)
                {
                    return true;
                }
                if (result.StatusCode == HttpStatusCode.NotFound)
                {
                    return false;
                }

                RaiseResponseError(request, result);
            }

            return false;
        }

        internal T RequestAndDeserialise<T>(HttpRequestMessage request, string missing = null) where T : class

        {
            request.Headers.Add("Accept", "application/json");

            using (var response = _httpClient.SendAsync(request))
            {
                var result = response.Result;
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<T>(result.Content.ReadAsStringAsync().Result, _jsonSettings);
                }
                if (result.StatusCode == HttpStatusCode.NotFound)
                {
                    RaiseResponseError(missing, request, result);
                }

                RaiseResponseError(request, result);
            }

            return null;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}