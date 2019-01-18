﻿using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class StandardsApiClient : ApiClientBase, IStandardsApiClient
    {
        public StandardsApiClient(string baseUri, ITokenService tokenService,
            ILogger<StandardsApiClient> logger) : base(baseUri, tokenService, logger)
        {
        }

        public StandardsApiClient(HttpClient httpClient, ITokenService tokenService, ILogger<ApiClientBase> logger) : base(httpClient, tokenService, logger)
        {
        }

        public async Task<EpaoStandardsCountResponse> GetEpaoStandardsCount(string epaoId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/standards/count/{epaoId}"))
            {
                return await RequestAndDeserialiseAsync<EpaoStandardsCountResponse>(request,
                    $"Could not find the organisation {epaoId}");
            }
        }

        public async Task<EpaoPipelineCountResponse> GetEpaoPipelineCount(string epaoId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/standards/pipeline/count/{epaoId}"))
            {
                return await RequestAndDeserialiseAsync<EpaoPipelineCountResponse>(request,
                    $"Could not find the organisation {epaoId}");
            }
        }

        public async Task<PaginatedList<GetEpaoRegisteredStandardsResponse>> GetEpaoRegisteredStandards(string epaoId, int? pageIndex = null)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/standards/{epaoId}?pageIndex={pageIndex}"))
            {
                return await RequestAndDeserialiseAsync<PaginatedList<GetEpaoRegisteredStandardsResponse>>(request,
                    $"Could not find the organisation {epaoId}");
            }
        }

        public async Task<PaginatedList<GetEpaoPipelineStandardsResponse>> GetEpaoPipelineStandards(string epaoId, int? pageIndex = null)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/standards/pipelines/{epaoId}?pageIndex={pageIndex}"))
            {
                return await RequestAndDeserialiseAsync<PaginatedList<GetEpaoPipelineStandardsResponse>>(request,
                    $"Could not find the organisation {epaoId}");
            }
        }

    }
}
