using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class RegisterImportApiClient : ApiClientBase
    {
        public RegisterImportApiClient(string baseUri, ITokenService tokenService) : base(baseUri, tokenService)
        {
        }

        public async Task Import(string userKey)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/register-import/"))
            {
                await PostPutRequest(userKey, request);
            }
        }
    }
}