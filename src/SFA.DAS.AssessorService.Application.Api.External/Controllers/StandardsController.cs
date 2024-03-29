﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.External.Infrastructure;
using SFA.DAS.AssessorService.Application.Api.External.Middleware;
using SFA.DAS.AssessorService.Application.Api.External.Models.Response.Standards;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;


namespace SFA.DAS.AssessorService.Application.Api.External.Controllers
{
    [Route("api/v1/standards")]
    [ApiController]
    [SwaggerTag("Standards")]
    public class StandardsController : ControllerBase
    {
        private readonly ILogger<StandardsController> _logger;
        private readonly IHeaderInfo _headerInfo;
        private readonly IApiClient _apiClient;

        public StandardsController(ILogger<StandardsController> logger, IHeaderInfo headerInfo, IApiClient apiClient)
        {
            _logger = logger;
            _headerInfo = headerInfo;
            _apiClient = apiClient;
        }

        [HttpGet("options")]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(SwaggerHelpers.Examples.GetOptionsForAllStandardResponseExample))]
        [SwaggerResponse((int)HttpStatusCode.OK, "The list of options for each Standard.", typeof(IEnumerable<StandardOptions>))]
        [SwaggerOperation("Get Options", "Gets the latest list of course options by Standard.")]
        public async Task<IActionResult> GetStandardOptionsForLatestStandardVersions()
        {
            var standards = await _apiClient.GetStandardOptionsForLatestStandardVersions();

            if(standards is null)
            {
                return StatusCode((int)HttpStatusCode.ServiceUnavailable);
            }

            return Ok(standards);
        }

        [HttpGet("options/{*standard}")]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(SwaggerHelpers.Examples.GetOptionsForStandardResponseExample))]
        [SwaggerResponse((int)HttpStatusCode.OK, "The list of options.", typeof(StandardOptions))]
        [SwaggerResponse((int)HttpStatusCode.NoContent, "The Standard was found, however it has no options.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "The Standard was not found.")]
        [SwaggerOperation("Get Options for Standard", "Gets the latest list of course options for the specified Standard.")]
        public async Task<IActionResult> GetOptionsForStandard([SwaggerParameter("Standard Code or Standard Reference Number")] string standard)
        {
            var requestedStandard = await _apiClient.GetStandardOptionsByStandard(standard);

            if (requestedStandard is null)
            {
                return NotFound();
            }
            else if (requestedStandard.CourseOption is null || requestedStandard.CourseOption.Any() == false)
            {
                return NoContent();
            }

            return Ok(requestedStandard);
        }

        [HttpGet("options/{standard}/{version}")]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(SwaggerHelpers.Examples.GetStandardVersionOptionsResponseExample))]
        [SwaggerResponse((int)HttpStatusCode.OK, "The list of options.", typeof(StandardOptions))]
        [SwaggerResponse((int)HttpStatusCode.NoContent, "The standard version was found, however it has no options.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "The standard version was not found.")]
        [SwaggerOperation("Get Options for a standard version", "Gets the latest list of course options for the specified Standard version.")]
        public async Task<IActionResult> GetOptionsForStandardVersion([SwaggerParameter("Standard Code or Standard Reference Number")] string standard, string version)
        {
            var standardVersion = await _apiClient.GetStandardOptionsByStandardIdAndVersion(standard, version);

            if (standardVersion is null)
            {
                return NotFound();
            }
            else if (standardVersion.CourseOption is null || standardVersion.CourseOption.Any() == false)
            {
                return NoContent();
            }

            return Ok(standardVersion);
        }
    }
}