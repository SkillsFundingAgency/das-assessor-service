using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.EpaoDataSync.Infrastructure
{
    public class WebConfiguration : IWebConfiguration
    {
        [JsonRequired] public string SqlConnectionString { get; set; }
        
        [JsonRequired] public string ProviderEventsClientBaseUrl { get; set; }
        [JsonRequired] public string SessionRedisConnectionString { get; set; }

    }
}
