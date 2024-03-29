﻿using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Domain.Exceptions;
using Swashbuckle.AspNetCore.Annotations;
using CreateOrganisationRequest = SFA.DAS.AssessorService.Api.Types.Models.CreateOrganisationRequest;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/organisations")]
    public class OrganisationController : Controller
    {
        private readonly ILogger<OrganisationController> _logger;
        private readonly IMediator _mediator;

        public OrganisationController(
            ILogger<OrganisationController> logger,
            IMediator mediator
        )
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost(Name = "CreateOrganisation")]
        [ValidateBadRequest]
        [SwaggerResponse((int) HttpStatusCode.Created, Type = typeof(OrganisationResponse))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> CreateOrganisation(
            [FromBody] CreateOrganisationRequest createOrganisationRequest)
        {
            _logger.LogInformation("Received Create Organisation Request");

            var organisation = await _mediator.Send(createOrganisationRequest);

            return CreatedAtRoute("CreateOrganisation",
                new {id = organisation.EndPointAssessorOrganisationId},
                organisation);
        }

        [HttpPut(Name = "UpdateOrganisation")]
        [ValidateBadRequest]
        [SwaggerResponse((int) HttpStatusCode.NoContent)]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> UpdateOrganisation(
            [FromBody] UpdateOrganisationRequest updateOrganisationRequest)
        {
            _logger.LogInformation("Received Update Organisation Request");

            await _mediator.Send(updateOrganisationRequest);

            return NoContent();
        }

        [HttpDelete(Name = "DeleteOrganisation")]
        [ValidateBadRequest]
        [SwaggerResponse((int) HttpStatusCode.NoContent)]
        [SwaggerResponse((int) HttpStatusCode.NotFound)]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> DeleteOrganisation(string endPointAssessorOrganisationId)
        {
            _logger.LogInformation("Received Delete Organisation Request");

            try
            {
                var deleteOrganisationRequest = new DeleteOrganisationRequest
                {
                    EndPointAssessorOrganisationId = endPointAssessorOrganisationId
                };

                await _mediator.Send(deleteOrganisationRequest);
            }
            catch (NotFoundException)
            {
                throw new ResourceNotFoundException();
            }

            return NoContent();
        }

        [HttpPut("NotifyUserManagementUsers", Name = "NotifyUserManagementUsers")]
        [ValidateBadRequest]
        [SwaggerResponse((int)HttpStatusCode.OK)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> NotifyUserManagementUsers([FromBody]NotifyUserManagementUsersRequest notifyUserManagementUsersRequest)
        {
            await _mediator.Send(notifyUserManagementUsersRequest, CancellationToken.None);

            return Ok();
        }

        [HttpPost("withdraw", Name = "WithdrawOrganisation")]
        [ValidateBadRequest]
        [SwaggerResponse((int)HttpStatusCode.NoContent)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> WithdrawalOrganisation(
           [FromBody] WithdrawOrganisationRequest request)
        {
            _logger.LogInformation("Received Withdrawal Organisation Request");

            await _mediator.Send(request);

            return NoContent();
        }
    }
}