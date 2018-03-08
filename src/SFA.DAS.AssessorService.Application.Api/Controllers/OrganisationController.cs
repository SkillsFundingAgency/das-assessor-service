using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Attributes;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Orchestrators;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize]
    [Route("api/v1/organisations")]
    public class OrganisationController : Controller
    {
        private readonly ILogger<OrganisationController> _logger;
        private readonly OrganisationOrchestrator _organisationOrchestrator;

        public OrganisationController(
            OrganisationOrchestrator organisationOrchestrator,
            ILogger<OrganisationController> logger
        )
        {
            _organisationOrchestrator = organisationOrchestrator;
            _logger = logger;
        }

        [HttpPost(Name = "CreateOrganisation")]
        [ValidateBadRequest]
        [SwaggerResponse((int) HttpStatusCode.Created, Type = typeof(OrganisationResponse))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> CreateOrganisation(
            [FromBody] CreateOrganisationRequest createOrganisationRequest)
        {
            _logger.LogInformation("Received Create Organisation Request");

            var organisation = await _organisationOrchestrator.CreateOrganisation(createOrganisationRequest);

            return CreatedAtRoute("CreateOrganisation",
                new {id = organisation.EndPointAssessorOrganisationId},
                organisation);
        }

        [HttpPut(Name = "UpdateOrganisation")]
        [ValidateBadRequest]
        [SwaggerResponse((int) HttpStatusCode.NoContent)]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> UpdateOrganisation(
            [FromBody] UpdateOrganisationRequest updateOrganisationRequest)
        {
            _logger.LogInformation("Received Update Organisation Request");

            await _organisationOrchestrator.UpdateOrganisation(updateOrganisationRequest);

            return NoContent();
        }

        [HttpDelete(Name = "DeleteOrganisation")]
        [ValidateBadRequest]
        [SwaggerResponse((int) HttpStatusCode.NoContent)]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.NotFound)]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> DeleteOrganisation(string endPointAssessorOrganisationId)
        {
            _logger.LogInformation("Received Delete Organisation Request");

            await _organisationOrchestrator.DeleteOrganisation(endPointAssessorOrganisationId);

            return NoContent();
        }
    }
}