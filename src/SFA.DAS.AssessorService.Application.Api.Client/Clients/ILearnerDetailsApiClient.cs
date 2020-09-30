using SFA.DAS.AssessorService.Api.Types.Models;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface ILearnerDetailsApiClient
    {
        Task<LearnerDetailResult> GetLearnerDetail(int stdCode, long uln, bool allLogs);
        Task<ImportLearnerDetailResponse> ImportLearnerDetail(ImportLearnerDetailRequest importLearnerDetailRequest);
    }
}