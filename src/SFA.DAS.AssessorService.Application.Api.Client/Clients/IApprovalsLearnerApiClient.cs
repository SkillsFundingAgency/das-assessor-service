using SFA.DAS.AssessorService.Api.Types.Models.Learner;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IApprovalsLearnerApiClient
    {
        Task<ApprovalsLearnerResult> GetLearnerRecord(int larsCode, long uln);
    }
}