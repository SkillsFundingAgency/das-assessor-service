using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Dashboard;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/dashboard")]
    public class DashboardController : Controller
    {
        private readonly ILogger<DashboardController> _logger;
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator, ILogger<DashboardController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("{epaoId}", Name = "GetEpaoDashboard")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(GetEpaoDashboardResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetEpaoDashboard(string epaoId)
        {
            _logger.LogInformation($"Received request to retrieve Dashboard for EPAO: {epaoId}");
            return Ok(await _mediator.Send(new GetEpaoDashboardRequest { EndPointAssessorOrganisationId = epaoId }));
        }
    }
}
