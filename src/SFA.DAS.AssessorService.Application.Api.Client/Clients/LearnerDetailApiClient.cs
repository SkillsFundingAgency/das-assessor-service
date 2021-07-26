using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class LearnerDetailApiClient : ApiClientBase, ILearnerDetailsApiClient
    {
        public LearnerDetailApiClient(string baseUri, ITokenService tokenService, ILogger<LearnerDetailApiClient> logger) 
            : base(baseUri, tokenService, logger)
        {
        }

        public LearnerDetailApiClient(HttpClient httpClient, ITokenService tokenService, ILogger<ApiClientBase> logger) 
            : base(httpClient, tokenService, logger)
        {
        }

        public async Task<LearnerDetailResult> GetLearnerDetail(int stdCode, long uln, bool allLogs)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/learnerDetails?stdCode={stdCode}&uln={uln}&alllogs={allLogs}"))
            {
                return await RequestAndDeserialiseAsync<LearnerDetailResult>(request,
                    $"Could not find the learner detail");
            }
        }

        public async Task<ImportLearnerDetailResponse> ImportLearnerDetail(ImportLearnerDetailRequest importLearnerDetailRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/v1/learnerdetails/import"))
            {
                return await PostPutRequestWithResponse<ImportLearnerDetailRequest, ImportLearnerDetailResponse>(request,
                    importLearnerDetailRequest);
            }
        }

        public async Task<int> GetPipelinesCount(string epaOrgId, int? stdCode)
        {
            if(stdCode.HasValue)
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/learnerdetails/pipelines-count/{epaOrgId}/{stdCode}"))
                {
                    return await RequestAndDeserialiseAsync<int>(request);
                }
            }
            else
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/learnerdetails/pipelines-count/{epaOrgId}"))
                {
                    return await RequestAndDeserialiseAsync<int>(request);
                }
            }
        }
    }
}
