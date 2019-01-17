using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Paging;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IStandardsApiClient
    {
        Task<EpaOrganisationStandardsCountResponse> GetEpaoStandardsCount(string epaoId);
        Task<EpaOrganisationPipelineCountResponse> GetEpaoPipelineCount(string epaoId);
        Task<PaginatedList<GetEpaoRegisteredStandardsResponse>> GetEpaoRegisteredStandards(string epaoId,
            int? pageIndex);
    }
}