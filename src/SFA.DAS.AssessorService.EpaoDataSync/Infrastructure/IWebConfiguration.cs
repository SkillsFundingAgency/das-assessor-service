using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Settings;

namespace SFA.DAS.AssessorService.EpaoDataSync.Infrastructure
{
    public interface IWebConfiguration
    {
        string SqlConnectionString { get; set; }
        string ProviderEventsClientBaseUrl { get; set; }
        string SessionRedisConnectionString { get; set; }
    }
}
