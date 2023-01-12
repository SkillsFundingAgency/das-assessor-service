using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface ILearnerDetailsApiClient
    {
        Task<LearnerDetailResult> GetLearnerDetail(int stdCode, long uln, bool allLogs);
        Task<ImportLearnerDetailResponse> ImportLearnerDetail(ImportLearnerDetailRequest importLearnerDetailRequest);
        Task<int> GetPipelinesCount(string epaOrgId, int? stdCode);
    }
}