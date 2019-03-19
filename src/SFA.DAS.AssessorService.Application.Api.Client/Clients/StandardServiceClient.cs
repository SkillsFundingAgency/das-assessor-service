using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.Api.Types;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class StandardServiceClient : ApiClientBase, IStandardServiceClient
    {
        public StandardServiceClient(string baseUri, ITokenService tokenService, ILogger<StandardServiceClient> logger) : base(baseUri, tokenService, logger)
        {
        }

        public StandardServiceClient(HttpClient httpClient, ITokenService tokenService, ILogger<ApiClientBase> logger) : base(httpClient, tokenService, logger)
        {
        }

        public async Task<IEnumerable<StandardSummary>> GetAllStandardSummaries()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/standard-service/standards/summaries"))
            {
                return await RequestAndDeserialiseAsync<IEnumerable<StandardSummary>>(request,
                    $"Could not get the list of standard summaries");
            }
        }

        public async Task<Standard> GetStandard(int standardCode)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/standard-service/standards/{standardCode}"))
            {
                return await RequestAndDeserialiseAsync<Standard>(request, $"Could not find the standard {standardCode}");
            }
        }
    }

    public interface IStandardServiceClient
    {
        Task<IEnumerable<StandardSummary>> GetAllStandardSummaries();
        Task<Standard> GetStandard(int standardCode);
    }
}