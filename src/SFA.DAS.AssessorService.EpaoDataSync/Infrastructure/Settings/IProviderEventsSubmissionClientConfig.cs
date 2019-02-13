using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.EpaoDataSync.Infrastructure.Settings
{
    public interface IProviderEventsSubmissionClientConfig
    {
        string ApiBaseUrl { get; set; }
        string ClientToken { get; set; }
    }
}
