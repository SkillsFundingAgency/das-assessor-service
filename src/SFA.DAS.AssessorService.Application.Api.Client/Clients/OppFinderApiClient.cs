using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class OppFinderApiClient : ApiClientBase, IOppFinderApiClient
    {
        public OppFinderApiClient(string baseUri,ITokenService tokenService,
            ILogger<OppFinderApiClient> logger) : base(baseUri, tokenService, logger)
        {
        }

        public OppFinderApiClient(HttpClient httpClient, ITokenService tokenService, ILogger<ApiClientBase> logger) : base(httpClient, tokenService, logger)
        {
        }

        public async Task<GetOppFinderApprovedStandardsResponse> GetApprovedStandards(string sortColumn, int sortAscending, int pageSize, int? pageIndex, int pageSetSize)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/oppfinder/approved?sortColumn={sortColumn}&sortAscending={sortAscending}&pageSize={pageSize}&pageIndex={pageIndex}&pageSetSize={pageSetSize}"))
            {
                return await RequestAndDeserialiseAsync<GetOppFinderApprovedStandardsResponse>(request,
                    $"Could not find any approved standards");
            }
        }
    }
}
