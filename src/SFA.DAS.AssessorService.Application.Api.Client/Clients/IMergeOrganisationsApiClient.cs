using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Paging;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IMergeOrganisationsApiClient
    {
        Task<object> MergeOrganisations(MergeOrganisationsRequest request);
        Task<PaginatedList<MergeLogEntry>> GetMergeLog(GetMergeLogRequest request);
        Task<MergeLogEntry> GetMergeLogEntry(int mergeId);
    }
}