using SFA.DAS.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Infrastructure.ApiClients.Roatp
{
    public interface IRoatpApiClientFactory
    {
        HttpClient CreateHttpClient();
    }
}
