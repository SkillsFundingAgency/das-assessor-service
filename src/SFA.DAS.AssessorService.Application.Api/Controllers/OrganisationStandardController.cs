using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.OrganisationStandards;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Domain.Exceptions;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1")]
    public class OrganisationStandardController : Controller
    {
        private readonly ILogger<OrganisationStandardController> _logger;
        private readonly IMediator _mediator;

        public OrganisationStandardController(
            ILogger<OrganisationStandardController> logger,
            IMediator mediator
        )
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost("organisationstandard", Name = "AddOrganisationStandard")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(EpaoStandardResponse))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> AddOrganisationStandard([FromBody] OrganisationStandardAddRequest request)
        {
            try
            {
                _logger.LogInformation("Adding new Organisation Standard and Versions");
                var result = await _mediator.Send(request);
                return Ok(new EpaoStandardResponse(result));
            }
            catch (NotFoundException ex)
            {
                _logger.LogError($@"Record is not available for organisation / standard: [{request.OrganisationId}, {request.StandardReference}]");
                return NotFound(new EpaoStandardResponse(ex.Message));
            }
            catch (AlreadyExistsException ex)
            {
                _logger.LogError($@"Record already exists for organisation/standard [{request.OrganisationId}, {request.StandardReference}]");
                return Conflict(new EpaoStandardResponse(ex.Message));
            }
            catch (BadRequestException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new EpaoStandardResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError($@"Bad request, Message: [{ex.Message}]");
                return BadRequest();
            }
        }

        [HttpPost("organisationstandardversion", Name = "CreateOrganisationStandardVersion")]
        [ValidateBadRequest]
        [SwaggerResponse((int) HttpStatusCode.Created, Type = typeof(OrganisationResponse))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> CreateOrganisationStandardVersion(
            [FromBody] OrganisationStandardVersionOptInRequest request)
        {
            _logger.LogInformation("Received Create Organisation Standard Version Request");

            var version = await _mediator.Send(request);

            return CreatedAtRoute("CreateOrganisationStandardVersion",
                new {id = version.StandardUId},
                version);
        }

        [HttpPut("organisationstandardversion/update")]
        [ValidateBadRequest]
        [SwaggerResponse((int)HttpStatusCode.Created, Type = typeof(EpaoStandardVersionResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task <IActionResult> UpdateOrganisationStandardVersion(
            [FromBody] UpdateOrganisationStandardVersionRequest request)
        {
            try
            {
                _logger.LogInformation("Recieved Update Organisation Standard Version Request");

                var updatedVersion = await _mediator.Send(request);

                return Ok(new EpaoStandardVersionResponse(updatedVersion.Version));
            }
            catch (Exception ex)
            {
                return BadRequest(new EpaoStandardVersionResponse(ex.Message));
            }
        }
    }
}