using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Dashboard;
using SFA.DAS.AssessorService.Data.Interfaces;
using SFA.DAS.AssessorService.Settings;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Dashboard
{
    public class GetEpaoDashboardHandler : IRequestHandler<GetEpaoDashboardRequest, GetEpaoDashboardResponse>
    {
        private readonly IApiConfiguration _config;
        private readonly IDashboardRepository _dashboardRespository;
        private readonly ILogger<GetEpaoDashboardHandler> _logger;

        public GetEpaoDashboardHandler(IApiConfiguration config, IDashboardRepository dashboardRespository, ILogger<GetEpaoDashboardHandler> logger)
        {
            _config = config;
            _dashboardRespository = dashboardRespository;
            _logger = logger;
        }

        public async Task<GetEpaoDashboardResponse> Handle(GetEpaoDashboardRequest request, CancellationToken cancellationToken)
        {
            var result = await _dashboardRespository.GetEpaoDashboard(request.EpaoId, _config.PipelineCutoff);

            return new GetEpaoDashboardResponse { StandardsCount = result.Standards, PipelinesCount = result.Pipeline, AssessmentsCount = result.Assessments };
        }
    }
}
