using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Common;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Paging;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class MergeOrganisationsApiClient : ApiClientBase, IMergeOrganisationsApiClient
    {
        public MergeOrganisationsApiClient(IAssessorApiClientFactory clientFactory, ILogger<MergeOrganisationsApiClient> logger) 
            : base(clientFactory.CreateHttpClient(), logger)
        {
        }

        public async Task<object> MergeOrganisations(MergeOrganisationsRequest request)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/mergeorganisations"))
            {
                return await PostPutRequestWithResponseAsync<MergeOrganisationsRequest, object>(httpRequest, request);
            }
        }

        public async Task<PaginatedList<MergeLogEntry>> GetMergeLog(GetMergeLogRequest request)
        {
            var requestUri = $"api/v1/mergeorganisations/log?pageSize={request.PageSize.Value}&pageIndex={request.PageIndex.Value}&orderBy={request.SortColumn}&orderDirection={request.SortDirection}";

            if (!string.IsNullOrEmpty(request.PrimaryEPAOId))
            {
                requestUri += $"&primaryEPAOId={request.PrimaryEPAOId}";
            }

            if (!string.IsNullOrEmpty(request.SecondaryEPAOId))
            {
                requestUri += $"&secondaryEPAOId={request.SecondaryEPAOId}";
            }

            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, requestUri))
            {
                return await RequestAndDeserialiseAsync<PaginatedList<MergeLogEntry>>(httpRequest, $"Could not retrieve merge log list");
            }
        }

        public async Task<MergeLogEntry> GetMergeLogEntry(int mergeId)
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"api/v1/mergeorganisations?id={mergeId}"))
            {
                return await RequestAndDeserialiseAsync<MergeLogEntry>(httpRequest, $"Could not retrieve merge log");
            }
        }
    }
}
