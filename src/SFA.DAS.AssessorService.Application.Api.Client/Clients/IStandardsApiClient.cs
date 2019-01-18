﻿using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IStandardsApiClient
    {
        Task<EpaoStandardsCountResponse> GetEpaoStandardsCount(string epaoId);
        Task<EpaoPipelineCountResponse> GetEpaoPipelineCount(string epaoId);
        Task<PaginatedList<GetEpaoRegisteredStandardsResponse>> GetEpaoRegisteredStandards(string epaoId,
            int? pageIndex);
        Task<PaginatedList<GetEpaoPipelineStandardsResponse>> GetEpaoPipelineStandards(string epaoId,
            int? pageIndex = null);
    }
}