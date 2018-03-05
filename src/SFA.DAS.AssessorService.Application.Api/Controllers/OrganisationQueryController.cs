using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Orchestrators;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
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
            _logger = logger;
            _getOrganisationsOrchestrator = getOrganisationsOrchestrator;
        }

        [HttpGet("{ukprn}", Name = "GetOrganisation")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(Organisation))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.NotFound, Type = typeof(string))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> SearchOrganisation(int ukprn)
        {
            _logger.LogInformation($"Received Search for an Organisation Request using ukprn {ukprn}");

            var organisation = await _getOrganisationsOrchestrator.SearchOrganisation(ukprn);
            return Ok(organisation);
        }

        [HttpGet(Name="GetAllOrganisations")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(List<Organisation>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetAllOrganisations()
        {
            _logger.LogInformation("Received request to retrieve All Organisations");

            var organisations = await _getOrganisationsOrchestrator.GetOrganisations();
            return Ok(organisations);
        }
    }
}