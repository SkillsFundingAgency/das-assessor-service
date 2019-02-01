using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types;
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

        [HttpGet("pipeline/count/{epaoId}", Name = "GetEpaoPipelineCount")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(int))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetEpaoPipelineCount(string epaoId)
        {
            _logger.LogInformation($"Received request to retrieve EPA Pipline count for Organisation {epaoId}");
            return Ok(await _mediator.Send(new GetEpaoPipelineCountRequest(epaoId)));
        }

        [HttpGet("count/{epaoId}", Name = "GetEpaoStandardsCount")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(int))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetEpaoStandardsCount(string epaoId)
        {
            _logger.LogInformation($"Received request to retrieve Standards count for Organisation {epaoId}");
            return Ok(await _mediator.Send(new GetEpaoStandardsCountRequest(epaoId)));
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

        [HttpGet("pipelines/{epaoId}", Name = "GetEpaoPipelineStandards")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(int))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetEpaoPipelineStandards(string epaoId, string orderBy, string orderDirection,int pageSize, int? pageIndex = null)
        {
            var normalisedPageIndex = (pageIndex == null || pageIndex == 0) ? 1 : pageIndex;
            _logger.LogInformation($"Received request to retrieve pipeline for standards of the organisation {epaoId}");
            return Ok(await _mediator.Send(new EpaoPipelineStandardsRequest(epaoId, orderBy, orderDirection,normalisedPageIndex, pageSize)));
        }
    }
}