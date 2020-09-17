using System;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Application.Handlers.ExternalApi.DataSync;

namespace SFA.DAS.AssessorService.Application.Api.Controllers.ExternalApi
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/externalapidatasync")]
    [ValidateBadRequest]
    public class RebuildExternalApiDataSyncController : Controller
    {
        private readonly IMediator _mediator;

        public RebuildExternalApiDataSyncController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("rebuild-sandbox", Name = "RebuildExternalApiSandbox")]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> RebuildExternalApiSandbox()
        {
            var request = new RebuildExternalApiSandboxRequest();
            await _mediator.Send(request);
            return Ok();
        }
    }
}
