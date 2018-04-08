using System.Net.Http;
using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.Extensions
{
    public static class RestClientResultExtensions
    {
        public static T Deserialise<T>(this HttpResponseMessage httpResponseMessage)
        {
            var content = httpResponseMessage.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}
