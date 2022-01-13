using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/mergeorganisations")]
    public class MergeOrganisationsController : Controller
    {
        private readonly ILogger<MergeOrganisationsController> _logger;
        private readonly IMediator _mediator;

        public MergeOrganisationsController(
            ILogger<MergeOrganisationsController> logger,
            IMediator mediator
        )
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost(Name = "MergeOrganisations")]
        [ValidateBadRequest]
        [SwaggerResponse((int)HttpStatusCode.Created, Type = typeof(MergeOrganisationsResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> MergeOrganisations(
            [FromBody] MergeOrganisationsRequest mergeOrganisationsRequest)
        {
            _logger.LogInformation("Received Merge Organisation Request");

            var mergeOrganisation = await _mediator.Send(mergeOrganisationsRequest);

            return CreatedAtRoute("GetMergeOrganisation",
                new { id = mergeOrganisation.Id },
                mergeOrganisation);
        }


        [HttpGet(Name = "GetMergeOrganisation")]
        [ValidateBadRequest]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(MergeOrganisationsResponse))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetMergeOrganisation(int id)
        {
            // We're just here because of CreatedAtRoute
            throw new NotImplementedException();
        }

        [HttpGet("log", Name = "GetMergeLog")]
        [ValidateBadRequest]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(GetMergeLogResponse))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetMergeLog()
        {
            var request = new GetMergeLogRequest();
            var response = await _mediator.Send(request);
            return new OkObjectResult(response);
        }
    }
}