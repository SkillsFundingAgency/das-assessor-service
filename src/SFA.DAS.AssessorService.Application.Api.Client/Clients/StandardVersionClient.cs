using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class StandardVersionClient : ApiClientBase, IStandardVersionClient
    {
        public StandardVersionClient(string baseUri, ITokenService tokenService, ILogger<StandardServiceClient> logger) : base(baseUri, tokenService, logger)
        {
        }

        public StandardVersionClient(HttpClient httpClient, ITokenService tokenService, ILogger<ApiClientBase> logger) : base(httpClient, tokenService, logger)
        {
        }

        public async Task<IEnumerable<StandardVersion>> GetAllStandardVersions()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/standard-version/standards"))
            {
                return await RequestAndDeserialiseAsync<IEnumerable<StandardVersion>>(request, $"Could not get the list of standards");
            }
        }
        
        public async Task<IEnumerable<StandardVersion>> GetStandardVersions(int standardId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/standard-version/standards/{standardId}"))
            {
                return await RequestAndDeserialiseAsync<IEnumerable<StandardVersion>>(request, $"Could not find the standard {standardId}");
            }
        }
    }

    public interface IStandardVersionClient
    {
        Task<IEnumerable<StandardVersion>> GetAllStandardVersions();
        Task<IEnumerable<StandardVersion>> GetStandardVersions(int standardId);
    }
}