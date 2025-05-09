﻿using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Staff;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Handlers.Staff;
using SFA.DAS.AssessorService.Domain.Paging;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Controllers.Staff
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/staffsearch/")]
    [ValidateBadRequest]
    public class StaffSearchController : Controller
    {
        private readonly IMediator _mediator;
        

        public StaffSearchController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet(Name="StaffSearch")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(PaginatedList<StaffSearchItems>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> StaffSearch(string searchQuery, int? page = 1)
        {            
            return Ok(await _mediator.Send(new StaffSearchRequest(searchQuery, page.Value)));
        }

        [HttpGet("batch", Name = "StaffBatchSearch")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(StaffBatchSearchResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> StaffBatchSearch(int batchNumber, int? page = 1)
        {
            return Ok(await _mediator.Send(new StaffBatchSearchRequest(batchNumber, page.Value)));
        }

        [HttpGet("batchlog", Name = "StaffBatchLog")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(PaginatedList<StaffBatchLogResult>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> StaffBatchLog(int? page = 1)
        {
            return Ok(await _mediator.Send(new StaffBatchLogRequest(page.Value)));
        }

        [HttpPost("framework-learners", Name = "StaffFrameworkSearch")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<FrameworkLearnerSearchResponse>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> StaffFrameworkSearch([FromBody]FrameworkLearnerSearchRequest frameworkLearnerSearchRequest)
        {
            return Ok(await _mediator.Send(frameworkLearnerSearchRequest));
        }
    }
}