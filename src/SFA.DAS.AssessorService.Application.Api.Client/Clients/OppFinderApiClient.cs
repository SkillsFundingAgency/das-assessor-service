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

        public async Task<GetOppFinderFilterStandardsResponse> GetFilterStandards(GetOppFinderFilterStandardsRequest filterStandardsRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/oppfinder/filter"))
            {
                return await PostPutRequestWithResponse<GetOppFinderFilterStandardsRequest, GetOppFinderFilterStandardsResponse>(request,
                    filterStandardsRequest);
            }
        }

        public async Task<GetOppFinderApprovedStandardsResponse> GetApprovedStandards(GetOppFinderApprovedStandardsRequest approvedStandardsRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/oppfinder/approved"))
            {
                return await PostPutRequestWithResponse<GetOppFinderApprovedStandardsRequest, GetOppFinderApprovedStandardsResponse>(request,
                    approvedStandardsRequest);
            }
        }

        public async Task<GetOppFinderNonApprovedStandardsResponse> GetNonApprovedStandards(GetOppFinderNonApprovedStandardsRequest nonApprovedStandradsRequest )
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/oppfinder/nonapproved"))
            {                    
                return await PostPutRequestWithResponse<GetOppFinderNonApprovedStandardsRequest, GetOppFinderNonApprovedStandardsResponse>(request,
                    nonApprovedStandradsRequest);
            }
        }
    }
}
