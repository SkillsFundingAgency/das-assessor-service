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

        public Task<IEnumerable<SearchResult>> Search(SearchQuery searchQuery)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/search"))
            {
                return PostPutRequestWithResponse<SearchQuery, IEnumerable<SearchResult>>(request, searchQuery);
            }
        }
    }
}