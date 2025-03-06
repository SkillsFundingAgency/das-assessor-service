using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Handlers.Staff;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System;
using System.Net;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.FrameworkSearch;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/learnerdetails")]
    [ValidateBadRequest]
    public class LearnerDetailsQueryController : Controller
    {
        private readonly IMediator _mediator;

        public LearnerDetailsQueryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int stdCode, long uln, bool allLogs = false)
        {
            return Ok(await _mediator.Send(new GetLearnerDetailRequest(stdCode, uln, allLogs)));
        }

        [HttpGet("pipelines-count/{epaOrgId}", Name = "GetPipelinesCount")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(int))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetPipelinesCount(string epaOrgId)
        {
            return Ok(await _mediator.Send(new GetPipelinesCountRequest(epaOrgId, null)));
        }

        [HttpGet("pipelines-count/{epaOrgId}/{stdCode}", Name = "GetPipelinesCountForStandard")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(int))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetPipelinesCountForStandard(string epaOrgId, int? stdCode = null)
        {
            return Ok(await _mediator.Send(new GetPipelinesCountRequest(epaOrgId, stdCode)));
        }

        [HttpGet("{apprenticeshipId}", Name = "GetApprenticeLearner")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(int))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetApprenticeLearner(long apprenticeshipId)
        {
            return Ok(await _mediator.Send(new GetApprenticeLearnerRequest(apprenticeshipId)));
        }

        [HttpGet("framework-learner/{id}", Name = "GetFrameworkLearner")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(GetFrameworkLearnerResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetFrameworkLearner(Guid id)
        {
            return Ok(await _mediator.Send(new GetFrameworkLearnerRequest() { Id = id}));
        }
    }
}