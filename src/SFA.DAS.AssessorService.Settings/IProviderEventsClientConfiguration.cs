using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.AssessorService.Settings
{
    public interface IProviderEventsClientConfiguration
    {
        string ApiBaseUrl { get; set; }
        string ClientToken { get; set; }
        string ApiVersion { get; set; }
    }
}
