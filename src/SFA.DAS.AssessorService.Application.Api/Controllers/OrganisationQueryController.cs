namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using AssessorService.Api.Types;
    using AssessorService.Api.Types.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Middleware;
    using Orchestrators;
    using Swashbuckle.AspNetCore.SwaggerGen;

    [Authorize]
    [Route("api/v1/organisations")]
    public class OrganisationQueryController : Controller
    {
        private readonly GetOrganisationsOrchestrator _getOrganisationsOrchestrator;
        private readonly ILogger<OrganisationQueryController> _logger;

        public OrganisationQueryController(
            GetOrganisationsOrchestrator getOrganisationsOrchestrator,
            ILogger<OrganisationQueryController> logger
        )
        {
            _getOrganisationsOrchestrator = getOrganisationsOrchestrator;
            _logger = logger;
        }

        [HttpGet("{ukprn}", Name = "GetOrganisation")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(Organisation))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.NotFound, Type = typeof(string))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetOrganisation(int ukprn)
        {
            var organisation = await _getOrganisationsOrchestrator.GetOrganisation(ukprn);
            return Ok(organisation);
        }

        [HttpGet]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(List<Organisation>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Get()
        {
            var organisations = await _getOrganisationsOrchestrator.GetOrganisations();
            return Ok(organisations);
        }
    }
}