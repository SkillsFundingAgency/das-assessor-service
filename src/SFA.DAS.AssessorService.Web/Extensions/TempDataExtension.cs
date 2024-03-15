using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Web.Infrastructure;

namespace SFA.DAS.AssessorService.Web.Extensions
{
    public static class TempDataExtensions
    {
        public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            object o;
            tempData.TryGetValue(key, out o);
            return o == null ? null : JsonConvert.DeserializeObject<T>((string)o);
        }
    }

    public static class TempDataAlertExtensions
    {
        public const string _ALERT = "D31B2AA5-6B8B-4332-ABBD-E16C7C1BEAFA";

        public static void SetAlert(this ITempDataDictionary tempData, Alert alert)
        {
            tempData.Put(_ALERT, alert);
        }

        public static Alert GetAlert(this ITempDataDictionary tempData)
        {
            return tempData.Get<Alert>(_ALERT);
        }
    }
}