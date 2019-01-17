using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/standards")]
    [ValidateBadRequest]
    public class StandardQueryController : Controller
    {

        private readonly ILogger<StandardQueryController> _logger;
        private readonly IMediator _mediator;

        public StandardQueryController(IMediator mediator, ILogger<StandardQueryController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("pipeline/count/{epaoId}", Name = "GetEpaOrganisationPipelineCount")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(int))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetEpaOrganisationPipelineCount(string epaoId)
        {
            _logger.LogInformation($"Received request to retrieve EPA Pipline count for Organisation {epaoId}");
            return Ok(await _mediator.Send(new GetEpaOrganisationPipelineCountRequest(epaoId)));
        }

        [HttpGet("count/{epaoId}", Name = "GetEpaOrganisationStandardsCount")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(int))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetEpaOrganisationStandardsCount(string epaoId)
        {
            _logger.LogInformation($"Received request to retrieve Standards count for Organisation {epaoId}");
            return Ok(await _mediator.Send(new GetEpaOrganisationStandardsCountRequest(epaoId)));
        }

        [HttpGet("{epaoId}", Name = "GetEpaoRegisteredStandards")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(int))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetEpaoRegisteredStandards(string epaoId, int? pageIndex = null)
        {
            var normalisedPageIndex = (pageIndex == null || pageIndex == 0) ? 1 : pageIndex;
            _logger.LogInformation($"Received request to retrieve Standards for Organisation {epaoId}");
            return Ok(await _mediator.Send(new GetEpaoRegisteredStandardsRequest(epaoId, normalisedPageIndex)));
        }
    }
}