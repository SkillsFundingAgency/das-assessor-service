using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class SearchApiClient : ApiClientBase, ISearchApiClient
    {
        public SearchApiClient(string baseUri, ITokenService tokenService) : base(baseUri, tokenService)
        {
        }

        public async Task<SearchResult> Search(SearchQuery searchQuery)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/search"))
            {
                return await PostPutRequestWithResponse<SearchQuery, SearchResult>(request, searchQuery);
            }
        }
    }
}