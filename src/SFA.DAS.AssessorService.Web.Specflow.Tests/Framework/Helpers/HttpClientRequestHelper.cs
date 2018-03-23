using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Web.Specflow.Tests.Framework.Helpers
{
    class HttpClientRequestHelper
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<String> ExecuteHttpPostRequest(String requestUri, String postData, String accessToken = "")
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = new StringContent(postData, Encoding.UTF8, "application/json")
            };

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            HttpResponseMessage postRequestResponse = await client.SendAsync(requestMessage);
            String content = await postRequestResponse.Content.ReadAsStringAsync();
            postRequestResponse.EnsureSuccessStatusCode();
            return content;
        }

        public static async Task<String> ExecuteHttpGetRequest(String requestUri, String accessToken = "")
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage getRequestResponse = await client.SendAsync(requestMessage);
            String content = await getRequestResponse.Content.ReadAsStringAsync();
            getRequestResponse.EnsureSuccessStatusCode();
            return content;
        }

        public static async Task<String> ExecuteHttpPutRequest(String requestUri, String putData, String accessToken = "")
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Put, requestUri)
            {
                Content = new StringContent(putData, Encoding.UTF8, "application/json")
            };

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            HttpResponseMessage putRequestResponse = await client.SendAsync(requestMessage);
            String content = await putRequestResponse.Content.ReadAsStringAsync();
            putRequestResponse.EnsureSuccessStatusCode();
            return content;
        }

        public static async Task ExecuteHttpDeleteRequest(String requestUri, String deleteData, String accessToken = "")
        {
            HttpRequestMessage requestMessage;
            if (String.IsNullOrEmpty(deleteData))
            {
                requestMessage = new HttpRequestMessage(HttpMethod.Delete, requestUri);
            }
            else
            {
                requestMessage = new HttpRequestMessage(HttpMethod.Delete, requestUri)
                {
                    Content = new StringContent(deleteData, Encoding.UTF8, "application/json")
                };

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                HttpResponseMessage deleteRequestResponse = await client.SendAsync(requestMessage);
                deleteRequestResponse.EnsureSuccessStatusCode();
            }
        }

        public static async Task<String> ExecuteHttpPatchRequest(String requestUri, String patchData, String accessToken = "")
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(new HttpMethod("PATCH"), requestUri)
            {
                Content = new StringContent(patchData, Encoding.UTF8, "application/json")
            };

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            HttpResponseMessage patchRequestResponse = await client.SendAsync(requestMessage);
            String content = await patchRequestResponse.Content.ReadAsStringAsync();
            patchRequestResponse.EnsureSuccessStatusCode();
            return content;
        }

        public static String ConvertTheObjectsToJsonFormat(object objectToSerialize)
        {
            return JsonConvert.SerializeObject(objectToSerialize);
        }
    }
}