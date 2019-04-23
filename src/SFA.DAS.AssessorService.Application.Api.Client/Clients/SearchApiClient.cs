using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class SearchApiClient : ApiClientBase, ISearchApiClient
    {
        public SearchApiClient(string baseUri, ITokenService tokenService, ILogger<SearchApiClient> logger) : base(baseUri, tokenService, logger)
        {
        }

        public SearchApiClient(HttpClient httpClient,ITokenService tokenService, ILogger<ApiClientBase> logger) : base(httpClient, tokenService, logger)
        {
        }

        public async Task<List<SearchResult>> Search(SearchQuery searchQuery)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/search"))
            {
                return await PostPutRequestWithResponse<SearchQuery, List<SearchResult>>(request, searchQuery);
            }
        }
    }
}