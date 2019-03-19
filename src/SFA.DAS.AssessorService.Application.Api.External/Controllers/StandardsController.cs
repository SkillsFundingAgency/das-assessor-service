using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.External.Infrastructure;
using SFA.DAS.AssessorService.Application.Api.External.Middleware;
using SFA.DAS.AssessorService.Application.Api.External.Models.Standards;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Examples;
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
        [SwaggerOperation("Get options", "Gets the latest list of course options by Standard.", Produces = new string[] { "application/json" })]
        public async Task<IActionResult> GetOptionsForAllStandards()
        {
            var standards = await _apiClient.GetStandards();

            if(standards is null)
            {
                return StatusCode((int)HttpStatusCode.ServiceUnavailable);
            }

            return Ok(standards.Where(s => s.CourseOption != null));
        }

        [HttpGet("options/{*standard}")]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(SwaggerHelpers.Examples.GetOptionsForStandardResponseExample))]
        [SwaggerResponse((int)HttpStatusCode.OK, "The list of options.", typeof(StandardOptions))]
        [SwaggerResponse((int)HttpStatusCode.NoContent, "The Standard was found, however it has no options.")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, "The Standard was not found.")]
        [SwaggerOperation("Get options for Standard", "Gets the latest list of course options for the specified Standard.", Produces = new string[] { "application/json" })]
        public async Task<IActionResult> GetOptionsForStandard([SwaggerParameter("Standard Code or Standard Reference Number")] string standard)
        {
            var requestedStandard = await _apiClient.GetStandard(standard);

            if (requestedStandard is null)
            {
                return NotFound();
            }
            else if (requestedStandard.CourseOption is null)
            {
                return NoContent();
            }

            return Ok(requestedStandard);
        }
    }
}