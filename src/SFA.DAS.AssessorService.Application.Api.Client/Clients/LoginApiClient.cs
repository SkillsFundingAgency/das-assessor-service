using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Api.Types.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class LoginApiClient : ApiClientBase, ILoginApiClient
    {
        public LoginApiClient(IAssessorApiClientFactory clientFactory, ILogger<LoginApiClient> logger) 
            : base(clientFactory.CreateHttpClient(), logger)
        {
        }

        public async Task<LoginResponse> Login(LoginRequest searchQuery)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/login"))
            {
                return await PostPutRequestWithResponseAsync<LoginRequest, LoginResponse>(request, searchQuery);
            }
        }
    }
}