using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Certificates;

namespace SFA.DAS.AssessorService.Application.Api.ReprintTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var baseUri = "https://at-assessors-api.apprenticeships.sfa.bis.gov.uk/";
            var httpClient = new HttpClient { BaseAddress = new Uri($"{baseUri}") };
            IEnumerable<CertificateResponse> certificateResponses;

            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/certificates?statuses=Printed"))
            {
                certificateResponses = RequestAndDeserialiseAsync<IEnumerable<CertificateResponse>>(httpClient, request,
                    $"Could not find the certificates").GetAwaiter().GetResult();
                foreach (var certificateResponse in certificateResponses)
                {
                    Console.WriteLine($"We have found the following Certificate References with a status of Printed ==> {certificateResponse.CertificateReference}");
                }

                Console.WriteLine("Please enter a certificate reference you would like to update ...");
                var certificateReference = Console.ReadLine();

                CertificateReprintRequest certificateReprintRequest;
                var updateCertificateReference =
                    certificateResponses.FirstOrDefault(q => q.CertificateReference == certificateReference);
                if (updateCertificateReference == null)
                {
                    // Create record that will not be found should return 404
                    certificateReprintRequest = new CertificateReprintRequest
                    {
                        CertificateReference = certificateReference,
                        AchievementDate = DateTime.Now,
                        Username = "XXXX",
                        LastName = "XXX"
                    };
                }
                else
                {
                    certificateReprintRequest = new CertificateReprintRequest
                    {
                        CertificateReference = certificateReference,
                        AchievementDate = updateCertificateReference.CertificateData.AchievementDate,
                        Username = "rajb",
                        LastName = updateCertificateReference.CertificateData.LearnerFamilyName
                    };
                }

                using (var reprintRequest = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/certificates/requestreprint"))
                {
                    PostPutRequest(httpClient, reprintRequest, certificateReprintRequest).GetAwaiter().GetResult();
                    Console.WriteLine("Update request ws successfull");
                }
            }
        }

        protected static async Task<T> RequestAndDeserialiseAsync<T>(HttpClient httpClient, HttpRequestMessage request, string message = null) where T : class
        {
            request.Headers.Add("Accept", "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());

            using (var response = httpClient.SendAsync(request))
            {
                var result = await response;
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    var json = await result.Content.ReadAsStringAsync();
                    return await Task.Factory.StartNew<T>(() => JsonConvert.DeserializeObject<T>(json));
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

        protected static async Task PostPutRequest<T>(HttpClient httpClient, HttpRequestMessage requestMessage, T model)
        {
            var serializeObject = JsonConvert.SerializeObject(model);
            requestMessage.Content = new StringContent(serializeObject,
                System.Text.Encoding.UTF8, "application/json");

            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", GetToken());

            var response = await httpClient.SendAsync(requestMessage);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                var message = "Could not find " + requestMessage.RequestUri.PathAndQuery;
                RaiseResponseError(message, requestMessage, response);
            }

            if (response.StatusCode == HttpStatusCode.InternalServerError)
            {
                throw new HttpRequestException();
            }
        }


        public static string GetToken()
        {
            var tenantId = "1a92889b-8ea1-4a16-8132-347814051567";
            var clientId = "b29803ca-cf9f-4b9a-9f63-a0e2700c55d7";
            var appKey = "GYM1zqwiHXQrM7r7x22kuUdSCEnciTM9K/BAUlCQ9Bs=";
            var resourceId = "https://citizenazuresfabisgov.onmicrosoft.com/assessorservice-api";

            var authority = $"https://login.microsoftonline.com/{tenantId}";
            var clientCredential = new ClientCredential(clientId, appKey);
            var context = new AuthenticationContext(authority, true);
            var tokenResult = context.AcquireTokenAsync(resourceId, clientCredential).Result;
            var accessToken = tokenResult.AccessToken;

            return accessToken;
        }

        private static void RaiseResponseError(HttpRequestMessage failedRequest, HttpResponseMessage failedResponse)
        {
            throw CreateRequestException(failedRequest, failedResponse);
        }

        private static void RaiseResponseError(string message, HttpRequestMessage failedRequest, HttpResponseMessage failedResponse)
        {
            if (failedResponse.StatusCode == HttpStatusCode.NotFound)
            {
                throw new ApplicationException(message, CreateRequestException(failedRequest, failedResponse));
            }

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
    }
}
