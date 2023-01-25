using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class StandardsApiClient : ApiClientBase, IStandardsApiClient
    {
        public StandardsApiClient(HttpClient httpClient, ITokenService tokenService, ILogger<ApiClientBase> logger)
            : base(httpClient, tokenService, logger)
        {
        }

        public async Task<PaginatedList<GetEpaoRegisteredStandardsResponse>> GetEpaoRegisteredStandards(string epaoId, int? pageIndex = null, int? pageSize = null)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/standards/{epaoId}?pageIndex={pageIndex}&pageSize={pageSize}"))
            {
                return await RequestAndDeserialiseAsync<PaginatedList<GetEpaoRegisteredStandardsResponse>>(request,
                    $"Could not find the organisation {epaoId}");
            }
        }
        
        public async Task<PaginatedList<EpaoPipelineStandardsResponse>> GetEpaoPipelineStandards(string epaoId, string standardFilterId, string providerFilterId, string epaDateFilterId, string orderBy, string orderDirection, int pageSize, int? pageIndex = null)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/standards/pipelines/{epaoId}?standardFilterId={standardFilterId}&providerFilterId={providerFilterId}&epaDateFilterId={epaDateFilterId}&pageSize={pageSize}&pageIndex={pageIndex}&orderBy={orderBy}&orderDirection={orderDirection}"))
            {
                return await RequestAndDeserialiseAsync<PaginatedList<EpaoPipelineStandardsResponse>>(request,
                    $"Could not find the organisation {epaoId}");
            }
        }

        public async Task<EpaoPipelineStandardsFiltersResponse> GetEpaoPipelineStandardsFilters(string epaoId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/standards/pipelines/{epaoId}/filters"))
            {
                return await RequestAndDeserialiseAsync<EpaoPipelineStandardsFiltersResponse>(request,
                    $"Could not find the organisation {epaoId}");
            }
        }

        public async Task<List<EpaoPipelineStandardsExtractResponse>> GetEpaoPipelineStandardsExtract(string epaoId, string standardFilterId, string providerFilterId, string epaDateFilterId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/standards/pipelines/extract/{epaoId}?standardFilterId={standardFilterId}&providerFilterId={providerFilterId}&epaDateFilterId={epaDateFilterId}"))
            {
                return await RequestAndDeserialiseAsync<List<EpaoPipelineStandardsExtractResponse>>(request,
                    $"Could not find the organisation {epaoId}");
            }
        }
    }
}
