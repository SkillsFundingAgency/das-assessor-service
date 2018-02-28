﻿namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
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
        private ILogger<OrganisationQueryController> _logger;

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
        public async Task<IActionResult> GetOrganisation(int ukprn)
        {
            _logger.LogInformation($"Received Search for an Organisation Request using ukprn {ukprn}");

            var organisation = await _getOrganisationsOrchestrator.GetOrganisation(ukprn);
            return Ok(organisation);
        }

        [HttpGet]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(List<Organisation>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Received request to retrieve All Organisations");

            var organisations = await _getOrganisationsOrchestrator.GetOrganisations();
            return Ok(organisations);
        }
    }
}