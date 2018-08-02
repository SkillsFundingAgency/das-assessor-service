﻿using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessmentOrgs.Api.Client.Core.Types;
using SFA.DAS.AssessorService.Api.Types;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Exceptions;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize]
    [Route("api/ao")]
    [ValidateBadRequest]
    public class RegisterQueryController : Controller
    {
        private readonly ILogger<RegisterQueryController> _logger;
        private readonly IMediator _mediator;

        public RegisterQueryController(IMediator mediator, ILogger<RegisterQueryController> logger
        )
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("organisation-types", Name = "GetOrganisationTypes")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(List<OrganisationType>))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetOrganisationTypes()
        {
            _logger.LogInformation("Get Organisation Types");
            return Ok(await _mediator.Send(new GetOrganisationTypesRequest()));
        }

        [HttpGet("delivery-areas", Name = "GetDeliveryAreas")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<DeliveryArea>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetDeliveryAreas()
        {
            _logger.LogInformation("Get Delivery Areas");
            return Ok(await _mediator.Send(new GetDeliveryAreasRequest()));
        }

        [HttpGet("assessment-organisations", Name = "GetAssessmentOrganisations")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<AssessmentOrganisationSummary>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetAssessmentOrganisations()
        {
            _logger.LogInformation("Get Assessment Organisations");
            return Ok(await _mediator.Send(new GetAssessmentOrganisationsRequest()));
        }


        [HttpGet("assessment-organisations/{organisationId}", Name = "GetAssessmentOrganisation")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(AssessmentOrganisationSummary))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, null)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetAssessmentOrganisation(string organisationId)
        {
            _logger.LogInformation($@"Get Assessment Organisation [{organisationId}]");
            var res = await _mediator.Send(new GetAssessmentOrganisationRequest {OrganisationId = organisationId });
            if (res == null) return NotFound();
            return Ok(res);
        }

        [HttpGet("assessment-organisations/standards/{standardId}", Name = "GetAssessmentOrganisationsByStandard")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<AssessmentOrganisationSummary>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, null)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetAssessmentOrganisationsByStandard(int standardId)
        {
            _logger.LogInformation($@"Get Assessment Organisations by Standard [{standardId}]");
            var res = await _mediator.Send(new GetAssessmentOrganisationsbyStandardRequest { StandardId = standardId });
            if (res == null) return NotFound();
            return Ok(res);
        }

        [HttpHead("assessment-organisations/{organisationId}", Name = "GetAssessmentOrganisationHead")]
        [SwaggerResponse((int)HttpStatusCode.NoContent, Type = typeof(AssessmentOrganisationSummary))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Head(string organisationId)
        {
            _logger.LogInformation($@"HEAD Assessment Organisation [{organisationId}]");
            var res = await _mediator.Send(new GetAssessmentOrganisationRequest { OrganisationId = organisationId });
            if (res == null) return NotFound();
            return NoContent();
        }

    }
}
