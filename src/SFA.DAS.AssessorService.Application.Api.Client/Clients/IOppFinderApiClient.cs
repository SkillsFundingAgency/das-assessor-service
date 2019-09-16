using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IOppFinderApiClient
    {
        Task<GetOppFinderApprovedStandardsResponse> GetApprovedStandards(string sortColumn, int sortAscending, int pageSize, int? pageIndex, int pageSetSize);
        Task<GetOppFinderNonApprovedStandardsResponse> GetNonApprovedStandards(string sortColumn, int sortAscending, int pageSize, int? pageIndex, int pageSetSize, string nonApprovedType);
    }
}