using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class OppFinderApiClient : ApiClientBase, IOppFinderApiClient
    {
        public OppFinderApiClient(HttpClient httpClient, IAssessorTokenService tokenService, ILogger<ApiClientBase> logger)
            : base(httpClient, tokenService, logger)
        {
        }

        public async Task<GetOppFinderFilterStandardsResponse> GetFilterStandards(GetOppFinderFilterStandardsRequest filterStandardsRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/oppfinder/filter"))
            {
                return await PostPutRequestWithResponseAsync<GetOppFinderFilterStandardsRequest, GetOppFinderFilterStandardsResponse>(request,
                    filterStandardsRequest);
            }
        }

        public async Task<GetOppFinderApprovedStandardsResponse> GetApprovedStandards(GetOppFinderApprovedStandardsRequest approvedStandardsRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/oppfinder/approved"))
            {
                return await PostPutRequestWithResponseAsync<GetOppFinderApprovedStandardsRequest, GetOppFinderApprovedStandardsResponse>(request,
                    approvedStandardsRequest);
            }
        }

        public async Task<GetOppFinderNonApprovedStandardsResponse> GetNonApprovedStandards(GetOppFinderNonApprovedStandardsRequest nonApprovedStandradsRequest )
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/oppfinder/nonapproved"))
            {                    
                return await PostPutRequestWithResponseAsync<GetOppFinderNonApprovedStandardsRequest, GetOppFinderNonApprovedStandardsResponse>(request,
                    nonApprovedStandradsRequest);
            }
        }

        public async Task<GetOppFinderApprovedStandardDetailsResponse> GetApprovedStandardDetails(GetOppFinderApprovedStandardDetailsRequest approvedStandardDetailsRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/oppfinder/approved-details"))
            {
                return await PostPutRequestWithResponseAsync<GetOppFinderApprovedStandardDetailsRequest, GetOppFinderApprovedStandardDetailsResponse>(request,
                    approvedStandardDetailsRequest);
            }
        }

        public async Task<GetOppFinderNonApprovedStandardDetailsResponse> GetNonApprovedStandardDetails(GetOppFinderNonApprovedStandardDetailsRequest nonApprovedStandardDetailsRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/oppfinder/nonapproved-details"))
            {
                return await PostPutRequestWithResponseAsync<GetOppFinderNonApprovedStandardDetailsRequest, GetOppFinderNonApprovedStandardDetailsResponse>(request,
                    nonApprovedStandardDetailsRequest);
            }
        }

        public async Task<bool> RecordExpresionOfInterest(OppFinderExpressionOfInterestRequest oppFinderExpressionOfInterestRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/oppfinder/expression-of-interest"))
            {
                return await PostPutRequestWithResponseAsync<OppFinderExpressionOfInterestRequest, bool>(request,
                    oppFinderExpressionOfInterestRequest);
            }
        }
    }
}
