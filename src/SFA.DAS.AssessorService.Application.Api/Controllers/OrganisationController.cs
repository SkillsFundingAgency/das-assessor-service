﻿namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using AssessorService.Api.Types;
    using AssessorService.Api.Types.Models;
    using AssessorService.Domain.Exceptions;
    using Attributes;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Localization;
    using Microsoft.Extensions.Logging;
    using Middleware;
    using Swashbuckle.AspNetCore.SwaggerGen;

    [Authorize]
    [Route("api/v1/organisations")]
    public class OrganisationController : Controller
    {
        private readonly IStringLocalizer<OrganisationController> _localizer;
        private readonly ILogger<OrganisationController> _logger;
        private readonly IMediator _mediator;

        public OrganisationController(IMediator mediator,
            IStringLocalizer<OrganisationController> localizer,
            ILogger<OrganisationController> logger
        )
        {
            _mediator = mediator;
            _localizer = localizer;
            _logger = logger;
        }

        [HttpPost(Name = "CreateOrganisation")]
        [ValidateBadRequest]
        [SwaggerResponse((int) HttpStatusCode.Created, Type = typeof(Organisation))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> CreateOrganisation(
            [FromBody] CreateOrganisationRequest organisationCreateViewModel)
        {
            _logger.LogInformation("Received Create Organisation Request");

            var organisationQueryViewModel = await _mediator.Send(organisationCreateViewModel);

            return CreatedAtRoute("CreateOrganisation",
                new {id = organisationQueryViewModel.Id},
                organisationQueryViewModel);
        }

        [HttpPut(Name = "UpdateOrganisation")]
        [ValidateBadRequest]
        [SwaggerResponse((int) HttpStatusCode.NoContent)]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> UpdateOrganisation(
            [FromBody] UpdateOrganisationRequest organisationUpdateViewModel)
        {
            _logger.LogInformation("Received Update Organisation Request");

            var organisationQueryViewModel = await _mediator.Send(organisationUpdateViewModel);

            return NoContent();
        }

        [HttpDelete(Name = "DeleteOrganisation")]
        [ValidateBadRequest]
        [SwaggerResponse((int) HttpStatusCode.NoContent)]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.NotFound)]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> DeleteOrganisation(Guid id)
        {
            try
            {
                _logger.LogInformation("Received Delete Organisation Request");

                var organisationDeleteViewModel = new DeleteOrgananisationRequest
                {
                    Id = id
                };

                await _mediator.Send(organisationDeleteViewModel);

                return NoContent();
            }
            catch (NotFound)
            {
                return NotFound();
            }
        }
    }
}