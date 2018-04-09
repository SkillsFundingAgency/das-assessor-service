using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> PostAsJsonAsync<TModel>(this HttpClient client, string requestUrl,
            TModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            return await client.PostAsync(requestUrl, stringContent);
        }

        public static async Task<HttpResponseMessage> PutAsJsonAsync<TModel>(this HttpClient client, string requestUrl,
            TModel model)
        {
            var json = JsonConvert.SerializeObject(model);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            return await client.PutAsync(requestUrl, stringContent);
        }

        public static async Task<HttpResponseMessage> DeleteAsJsonAsync(this HttpClient client, string requestUrl)
        {
            return await client.DeleteAsync(requestUrl);
        }
    }
}