using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Dashboard;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Handlers.Dashboard
{
    public class GetEpaoDashboardHandler : IRequestHandler<GetEpaoDashboardRequest, GetEpaoDashboardResponse>
    {
        private readonly ILogger<GetEpaoDashboardHandler> _logger;
        private readonly IDashboardRepository _dashboardRespository;

        public GetEpaoDashboardHandler (IDashboardRepository dashboardRespository, ILogger<GetEpaoDashboardHandler> logger)
        {
            _dashboardRespository = dashboardRespository;
            _logger = logger;
        }

        public async Task<GetEpaoDashboardResponse> Handle(GetEpaoDashboardRequest request, CancellationToken cancellationToken)
        {
            var result = await _dashboardRespository.GetEpaoDashboard(request.EndPointAssessorOrganisationId);

            return new GetEpaoDashboardResponse { StandardsCount = result.Standards, PipelinesCount = result.Pipeline, AssessmentsCount = result.Assessments };
        }
    }
}
