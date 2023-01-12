using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class OppFinderApiClient : ApiClientBase, IOppFinderApiClient
    {
        public OppFinderApiClient(string baseUri, ITokenService tokenService,
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

        public async Task<GetOppFinderNonApprovedStandardsResponse> GetNonApprovedStandards(GetOppFinderNonApprovedStandardsRequest nonApprovedStandradsRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/oppfinder/nonapproved"))
            {
                return await PostPutRequestWithResponse<GetOppFinderNonApprovedStandardsRequest, GetOppFinderNonApprovedStandardsResponse>(request,
                    nonApprovedStandradsRequest);
            }
        }

        public async Task<GetOppFinderApprovedStandardDetailsResponse> GetApprovedStandardDetails(GetOppFinderApprovedStandardDetailsRequest approvedStandardDetailsRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/oppfinder/approved-details"))
            {
                return await PostPutRequestWithResponse<GetOppFinderApprovedStandardDetailsRequest, GetOppFinderApprovedStandardDetailsResponse>(request,
                    approvedStandardDetailsRequest);
            }
        }

        public async Task<GetOppFinderNonApprovedStandardDetailsResponse> GetNonApprovedStandardDetails(GetOppFinderNonApprovedStandardDetailsRequest nonApprovedStandardDetailsRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/oppfinder/nonapproved-details"))
            {
                return await PostPutRequestWithResponse<GetOppFinderNonApprovedStandardDetailsRequest, GetOppFinderNonApprovedStandardDetailsResponse>(request,
                    nonApprovedStandardDetailsRequest);
            }
        }

        public async Task<bool> RecordExpresionOfInterest(OppFinderExpressionOfInterestRequest oppFinderExpressionOfInterestRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/oppfinder/expression-of-interest"))
            {
                return await PostPutRequestWithResponse<OppFinderExpressionOfInterestRequest, bool>(request,
                    oppFinderExpressionOfInterestRequest);
            }
        }
    }
}
