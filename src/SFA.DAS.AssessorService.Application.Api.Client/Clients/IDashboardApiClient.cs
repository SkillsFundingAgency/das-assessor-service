using SFA.DAS.AssessorService.Api.Types.Models.Dashboard;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public interface IDashboardApiClient
    {
        System.Threading.Tasks.Task<GetEpaoDashboardResponse> GetEpaoDashboard(string epaoId);
    }
}