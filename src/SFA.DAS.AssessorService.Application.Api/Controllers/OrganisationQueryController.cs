namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using SFA.DAS.AssessorService.Application.Api.Middleware;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using Microsoft.AspNetCore.Authorization;
    using SFA.DAS.AssessorService.Application.Api.Orchesrators;
    using SFA.DAS.AssessorService.Api.Types;

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
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Organisation))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.NotFound, Type = typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetOrganisation(int ukprn)
        {
            var organisation = await _getOrganisationsOrchestrator.GetOrganisation(ukprn);
            return Ok(organisation);
        }

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(List<Organisation>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Get()
        {
            var organisations = await _getOrganisationsOrchestrator.GetOrganisations();
            return Ok(organisations);
        }
    }
}