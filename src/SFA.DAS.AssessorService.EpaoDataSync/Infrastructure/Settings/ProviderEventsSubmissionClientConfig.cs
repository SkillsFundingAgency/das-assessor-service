using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.EpaoDataSync.Infrastructure.Settings
{
    public class ProviderEventsSubmissionClientConfig: IProviderEventsSubmissionClientConfig
    {
        public string ApiBaseUrl { get; set; }
        public string ClientToken { get; set; }
    }
}
