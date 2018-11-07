using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.AssessmentOrgs.Api.Client.Core.Types;
using SFA.DAS.AssessorService.Api.Types;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.AO;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.Validation;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Exceptions;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
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
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(List<DeliveryArea>))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetDeliveryAreas()
        {
            _logger.LogInformation("Get Delivery Areas");
            return Ok(await _mediator.Send(new GetDeliveryAreasRequest()));
        }

        [HttpGet("assessment-organisations", Name = "GetAssessmentOrganisations")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(List<AssessmentOrganisationSummary>))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetAssessmentOrganisations()
        {
            _logger.LogInformation("Get Assessment Organisations");
            return Ok(await _mediator.Send(new GetAssessmentOrganisationsRequest()));
        }

        [HttpGet("assessment-organisations/{organisationId}", Name = "GetAssessmentOrganisation")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(EpaOrganisation))]
        [SwaggerResponse((int) HttpStatusCode.NotFound, null)]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetAssessmentOrganisation(string organisationId)
        {
            _logger.LogInformation($@"Get Assessment Organisation [{organisationId}]");
            var result = await _mediator.Send(new GetAssessmentOrganisationRequest {OrganisationId = organisationId});
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("assessment-organisations/standards/{standardId}", Name = "GetAssessmentOrganisationsByStandard")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(List<EpaOrganisation>))]
        [SwaggerResponse((int) HttpStatusCode.NotFound, null)]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetAssessmentOrganisationsByStandard(int standardId)
        {
            _logger.LogInformation($@"Get Assessment Organisations by Standard [{standardId}]");
            var result = await _mediator.Send(new GetAssessmentOrganisationsbyStandardRequest
                {StandardId = standardId});
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("assessment-organisations/{organisationId}/standards", Name =
            "GetOrganisationStandardsByOrganisation")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(List<OrganisationStandardSummary>))]
        [SwaggerResponse((int) HttpStatusCode.NotFound, null)]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetOrganisationStandardsByOrganisation(string organisationId)
        {
            _logger.LogInformation($@"Get Organisations Standards by OrganisationId [{organisationId}]");
            var result = await _mediator.Send(new GetStandardsByOrganisationRequest {OrganisationId = organisationId});
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("assessment-organisations/contacts/{contactId}", Name = "GetContact")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(AssessmentOrganisationContact))]
        [SwaggerResponse((int) HttpStatusCode.NotFound, null)]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetContact(string contactId)
        {
            _logger.LogInformation($@"Get Contact from Id [{contactId}]");
            var result = await _mediator.Send(new GetContactRequest {ContactId = contactId});
            if (result == null) return BadRequest();
            return Ok(result);
        }
        
        [HttpGet("assessment-organisations/organisation-standard/{organisationStandardId}", Name = "GetOrganisationStandard")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(OrganisationStandard))]
        [SwaggerResponse((int) HttpStatusCode.NotFound, null)]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetOrganisationStandard(int organisationStandardId)
        {
            _logger.LogInformation($@"Get Organisation Standard from Id [{organisationStandardId}]");
            var result = await _mediator.Send(new GetOrganisationStandardRequest {OrganisationStandardId = organisationStandardId});
            if (result == null) return BadRequest();
            return Ok(result);
        }

        [HttpHead("assessment-organisations/{organisationId}", Name = "GetAssessmentOrganisationHead")]
        [SwaggerResponse((int) HttpStatusCode.NoContent)]
        [SwaggerResponse((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Head(string organisationId)
        {
            _logger.LogInformation($@"HEAD Assessment Organisation [{organisationId}]");
            var result = await _mediator.Send(new GetAssessmentOrganisationRequest {OrganisationId = organisationId});
            if (result == null) return NotFound();
            return NoContent();
        }

        [HttpGet("assessment-organisations/search/{searchstring}", Name = "SearchAssessmentOrganisations")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(List<AssessmentOrganisationSummary>))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> SearchAssessmentOrganisations(string searchstring)
        {
            _logger.LogInformation($@"Search Assessment Organisations for [{searchstring}]");
            return Ok(await _mediator.Send(new SearchAssessmentOrganisationsRequest {SearchTerm = searchstring}));
        }

        [HttpGet("assessment-organisations/email/{emailAddress}", Name = "GetAssessmentOrganisationFromEmail")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<AssessmentOrganisationSummary>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetAssessmentOrganisationFromEmail(string emailAddress)
        {
            _logger.LogInformation($@"Get organisation from email [{emailAddress}]");
            return Ok(await _mediator.Send(new GetAssessmentOrganisationByEmailRequest { Email = emailAddress }));
        }


        [HttpGet("assessment-organisations/standards/search/{searchstring}", Name = "SearchStandards")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(List<StandardSummary>))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> SearchStandards(string searchstring)
        {
            _logger.LogInformation($@"Search Standards for [{searchstring}]");
            return Ok(await _mediator.Send(new SearchStandardsRequest {SearchTerm = searchstring}));
        }
    }
}

