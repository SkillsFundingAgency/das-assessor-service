﻿using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class LearnerDetailsApiClient : ApiClientBase, ILearnerDetailsApiClient
    {
        public LearnerDetailsApiClient(IAssessorApiClientFactory clientFactory, ILogger<LearnerDetailsApiClient> logger)
            : base(clientFactory.CreateHttpClient(), logger)
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
                return await PostPutRequestWithResponseAsync<ImportLearnerDetailRequest, ImportLearnerDetailResponse>(request,
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

        public async Task<GetFrameworkLearnerResponse> GetFrameworkLearner(Guid frameworkLearnerId, bool allLogs)
        { 
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/learnerdetails/framework-learner/{frameworkLearnerId}?allLogs={allLogs}"))
            {
                return await RequestAndDeserialiseAsync<GetFrameworkLearnerResponse>(request, $"Could not retrieve framework learner");
            }
        }
    }
}
