using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IStandardsApiClient
    {
        Task<PaginatedList<GetEpaoRegisteredStandardsResponse>> GetEpaoRegisteredStandards(string epaoId, int? pageIndex, int? pageSize);

        Task<PaginatedList<GetEpaoRegisteredStandardsResponse>> GetEpaoRegisteredStandards(string epaoId, bool requireAtLeastOneVersion, int? pageIndex, int? pageSize);

        Task<PaginatedList<EpaoPipelineStandardsResponse>> GetEpaoPipelineStandards(string epaoId, string standardFilterId, string providerFilterId, string epaDateFilterId, string orderBy, string orderDirection, int pageSize, int? pageIndex = null);

        Task<EpaoPipelineStandardsFiltersResponse> GetEpaoPipelineStandardsFilters(string epaoId);

        Task<List<EpaoPipelineStandardsExtractResponse>> GetEpaoPipelineStandardsExtract(string epaoId, string standardFilterId, string providerFilterId, string epaDateFilterId);
    }
}