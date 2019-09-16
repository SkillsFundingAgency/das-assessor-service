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
    [Route("api/v1/oppfinder")]
    [ValidateBadRequest]
    public class OppFinderQueryController : Controller
    {

        private readonly ILogger<OppFinderQueryController> _logger;
        private readonly IMediator _mediator;

        public OppFinderQueryController(IMediator mediator, ILogger<OppFinderQueryController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("approved", Name = "GetApprovedStandards")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(int))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetApprovedStandards(string sortColumn, int sortAscending, int pageSize, int? pageIndex, int pageSetSize)
        {
            var normalisedPageIndex = (pageIndex == null || pageIndex == 0) ? 1 : pageIndex;
            _logger.LogInformation($"Received request to retrieve approved standards");
            return Ok(await _mediator.Send(new GetOppFinderApprovedStandardsRequest(sortColumn, sortAscending, pageSize, normalisedPageIndex, pageSetSize)));
        }

        [HttpGet("nonapproved", Name = "GetNonApprovedStandards")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(int))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetNonApprovedStandards(string sortColumn, int sortAscending, int pageSize, int? pageIndex, int pageSetSize, string nonApprovedType)
        {
            var normalisedPageIndex = (pageIndex == null || pageIndex == 0) ? 1 : pageIndex;
            _logger.LogInformation($"Received request to retrieve non approved standards {nonApprovedType}");
            return Ok(await _mediator.Send(new GetOppFinderNonApprovedStandardsRequest(sortColumn, sortAscending, pageSize, normalisedPageIndex, pageSetSize, nonApprovedType)));
        }
    }
}