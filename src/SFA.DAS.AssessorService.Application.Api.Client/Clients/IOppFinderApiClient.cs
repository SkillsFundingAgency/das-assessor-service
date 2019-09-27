using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Domain.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IOppFinderApiClient
    {
        Task<GetOppFinderFilterStandardsResponse> GetFilterStandards(GetOppFinderFilterStandardsRequest filterStandardsRequest);
        Task<GetOppFinderApprovedStandardsResponse> GetApprovedStandards(GetOppFinderApprovedStandardsRequest approvedStandardsRequest);
        Task<GetOppFinderNonApprovedStandardsResponse> GetNonApprovedStandards(GetOppFinderNonApprovedStandardsRequest nonApprovedStandardsRequest);

        Task<GetOppFinderApprovedStandardDetailsResponse> GetApprovedStandardDetails(GetOppFinderApprovedStandardDetailsRequest approvedStandardDetailsRequest);
        Task<GetOppFinderNonApprovedStandardDetailsResponse> GetNonApprovedStandardDetails(GetOppFinderNonApprovedStandardDetailsRequest nonApprovedStandardDetailsRequest);
    }
}