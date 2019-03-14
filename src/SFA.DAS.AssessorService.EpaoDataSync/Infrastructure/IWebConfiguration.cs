using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.EpaoDataSync.Infrastructure.Settings;
using SFA.DAS.AssessorService.Settings;
using ProviderEventsClientConfiguration = SFA.DAS.AssessorService.EpaoDataSync.Infrastructure.Settings.ProviderEventsClientConfiguration;

namespace SFA.DAS.AssessorService.EpaoDataSync.Infrastructure
{
    public interface IWebConfiguration
    {
        string SqlConnectionString { get; set; }
        ProviderEventsClientConfiguration ProviderEventsClientConfiguration { get; set; }
        string SessionRedisConnectionString { get; set; }
        ProviderEventsSubmissionClientConfig ProviderEventsSubmissionClientConfig { get; set; }
    }
}
