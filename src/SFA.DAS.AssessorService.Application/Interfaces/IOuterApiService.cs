using SFA.DAS.AssessorService.Application.Infrastructure.OuterApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Interfaces
{
    public interface IOuterApiService
    {
        Task<IEnumerable<StandardDetailResponse>> GetAllStandards();
        Task<IEnumerable<GetStandardsListItem>> GetActiveStandards();
        Task<IEnumerable<GetStandardsListItem>> GetDraftStandards();
        Task<GetAllLearnersResponse> GetAllLearners(DateTime? sinceTime, int batchNumber, int batchSize);
    }
}
