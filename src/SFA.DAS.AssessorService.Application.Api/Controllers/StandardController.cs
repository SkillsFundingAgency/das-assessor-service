﻿using System;
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
using SFA.DAS.AssessorService.Web.Staff.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/ao/update-standards")]
    [ValidateBadRequest]
    public class StandardController : Controller
    {
        private readonly ILogger<StandardController> _logger;
        private readonly IMediator _mediator;
        //private readonly IAssessmentOrgsApiClient _assessmentOrgsApiClient;


        public StandardController(ILogger<StandardController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost(Name = "GatherAndStoreStandards")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(GatherStandardsResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GatherAndStoreStandards([FromBody] GatherStandardsRequest request)
        {

            var processDetails = await _mediator.Send(request);

            var res = processDetails + "; Processing of Standards Upsert complete";
            return Ok(new GatherStandardsResponse(res));
        }
    }
}