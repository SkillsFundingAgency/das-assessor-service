﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Handlers.ao.GetEpaOrganisationsByStandard;
using Swashbuckle.AspNetCore.Annotations;

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

        [HttpGet("{epaoId}", Name = "GetEpaoRegisteredStandards")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(int))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetEpaoRegisteredStandards(string epaoId, int? pageIndex = 1, int? pageSize = 10)
        {
            _logger.LogInformation($"Received request to retrieve Standards for Organisation {epaoId}");
            return Ok(await _mediator.Send(new GetEpaoRegisteredStandardsRequest(epaoId, pageIndex.Value, pageSize.Value)));
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

        [HttpGet("pipelines/extract/{epaoId}", Name = "GetEpaoPipelineStandardsExtract")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(int))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetEpaoPipelineStandardsExtract(string epaoId)
        {
            _logger.LogInformation($"Received request to extract pipeline for standards of the organisation {epaoId}");
            return Ok(await _mediator.Send(new EpaoPipelineStandardsExtractRequest(epaoId)));
        }

        [HttpGet("{standardCode}/organisations")]
        [ProducesResponseType(typeof(List<OrganisationStandardResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async  Task<IActionResult> GetEpaosByStandard(int standardCode)
        {
            if (standardCode == 0)
            {
                return BadRequest();
            }

            var epaOrganisationResponse = await _mediator.Send(new GetEpaOrganisationsByStandardQuery
            {
                Standard = standardCode
            });

            if (!epaOrganisationResponse.EpaOrganisations.Any())
            {
                return NotFound();
            }

            var result = epaOrganisationResponse.EpaOrganisations.Select(Mapper.Map<OrganisationStandardResponse>).ToList();

            return Ok(result);

        }
    }
}