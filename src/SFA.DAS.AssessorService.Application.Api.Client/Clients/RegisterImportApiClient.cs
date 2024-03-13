using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Common;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class RegisterImportApiClient : ApiClientBase
    {
        public RegisterImportApiClient(IAssessorApiClientFactory clientFactory, ILogger<RegisterImportApiClient> logger)
            : base(clientFactory.CreateHttpClient(), logger)
        {
        }

        public async Task Import()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/register-import/"))
            {
                await PostPutRequestAsync(request);
            }
        }
    }
}