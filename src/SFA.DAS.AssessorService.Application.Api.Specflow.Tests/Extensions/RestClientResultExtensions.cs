using Newtonsoft.Json;

namespace SFA.DAS.AssessorService.Application.Api.Specflow.Tests.Extensions
{
    public static class RestClientResultExtensions
    {
        public static T Deserialise<T>(this RestClientResult restClientResult)
        {
            return JsonConvert.DeserializeObject<T>(restClientResult.JsonResult);
        }
    }
}
