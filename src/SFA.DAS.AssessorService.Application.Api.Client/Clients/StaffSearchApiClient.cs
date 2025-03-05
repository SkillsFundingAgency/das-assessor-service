using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AssessorService.Domain.Paging;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class StaffSearchApiClient : ApiClientBase, IStaffSearchApiClient
    {
        public StaffSearchApiClient(IAssessorApiClientFactory clientFactory, ILogger<StaffSearchApiClient> logger) 
            : base(clientFactory.CreateHttpClient(), logger)
        {
        }

        public async Task<StaffSearchResult> Search(string searchString, int page)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/staffsearch?searchQuery={searchString}&page={page}"))
            {
                return await RequestAndDeserialiseAsync<StaffSearchResult>(request, $"Could not retrieve learner search result");
            }
        }

        public async Task<StaffBatchSearchResponse> BatchSearch(int batchNumber, int page)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/staffsearch/batch?batchNumber={batchNumber}&page={page}"))
            {
                return await RequestAndDeserialiseAsync<StaffBatchSearchResponse>(request, $"Could not retrieve batch search result");
            }
        }

        public async Task<PaginatedList<StaffBatchLogResult>> BatchLog(int page)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/staffsearch/batchlog?page={page}"))
            {
                return await RequestAndDeserialiseAsync<PaginatedList<StaffBatchLogResult>>(request, $"Could not retrieve batch search result");
            }
        }

        public async Task<List<FrameworkLearnerSearchResponse>> SearchFrameworkLearners(FrameworkLearnerSearchRequest frameworkLearnerSearchRequest)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/staffsearch/framework-learners"))
            {
                return await PostPutRequestWithResponseAsync<FrameworkLearnerSearchRequest, List<FrameworkLearnerSearchResponse>>(request, frameworkLearnerSearchRequest);
            }
        }
    }
}
