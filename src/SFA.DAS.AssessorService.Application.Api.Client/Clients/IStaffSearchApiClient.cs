using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AssessorService.Domain.Paging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IStaffSearchApiClient
    {
        Task<PaginatedList<StaffBatchLogResult>> BatchLog(int page);
        Task<StaffBatchSearchResponse> BatchSearch(int batchNumber, int page);
        Task<StaffSearchResult> SearchCertificates(string searchString, int page);
        Task<List<FrameworkLearnerSearchResponse>> SearchFrameworkLearners(FrameworkLearnerSearchRequest frameworkSearchRequest);
    }
}