using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Settings
{
    public class ProviderEventsClientConfiguration: IProviderEventsClientConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public string ClientToken { get; set; }
        public string ApiVersion { get; set; }
    }
}
