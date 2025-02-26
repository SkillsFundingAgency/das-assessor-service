using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Api.Types.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class SearchApiClient : ApiClientBase, ISearchApiClient
    {
        public SearchApiClient(IAssessorApiClientFactory clientFactory, ILogger<SearchApiClient> logger) 
            : base(clientFactory.CreateHttpClient(), logger)
        {
        }

        public async Task<List<SearchResult>> SearchStandards(SearchQuery searchQuery)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/search"))
            {
                return await PostPutRequestWithResponseAsync<SearchQuery, List<SearchResult>>(request, searchQuery);
            }
        }
    }
}