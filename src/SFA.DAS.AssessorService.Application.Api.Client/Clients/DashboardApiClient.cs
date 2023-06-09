using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Dashboard;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class DashboardApiClient : ApiClientBase, IDashboardApiClient
    {
        public DashboardApiClient(HttpClient httpClient, IAssessorTokenService tokenService, ILogger<ApiClientBase> logger)
            : base(httpClient, tokenService, logger)
        {
        }

        public async Task<GetEpaoDashboardResponse> GetEpaoDashboard(string epaoId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/dashboard/{epaoId}"))
            {
                return await RequestAndDeserialiseAsync<GetEpaoDashboardResponse>(request, $"Could not get the epao dashboard for epao: {epaoId}");
            }
        }
    }
}