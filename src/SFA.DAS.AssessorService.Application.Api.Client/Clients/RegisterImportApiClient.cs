using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class RegisterImportApiClient : ApiClientBase
    {
        public RegisterImportApiClient(string baseUri, ITokenService tokenService, ILogger<RegisterImportApiClient> logger) : base(baseUri, tokenService, logger)
        {
        }

        public RegisterImportApiClient(HttpClient httpClient, ITokenService tokenService, ILogger<ApiClientBase> logger) : base(httpClient, tokenService, logger)
        {
        }

        public async Task Import()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/register-import/"))
            {
                await PostPutRequest(request);
            }
        }
    }
}