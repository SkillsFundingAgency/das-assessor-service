using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Interfaces;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using Swashbuckle.AspNetCore.Annotations;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/standard-service/")]
    [ValidateBadRequest]
    public class StandardServiceController : Controller
    {
        private readonly ILogger<StandardServiceController> _logger;
        private readonly IStandardService _standardService;

        public StandardServiceController(ILogger<StandardServiceController> logger, IStandardService standardService)
        {
            _logger = logger;
            _standardService = standardService;
        }

        [HttpGet("standards", Name = "GetAllStandards")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<StandardCollation>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetAllStandards()
        {
            _logger.LogInformation($@"Get all standards from Standard Service");
            var standards = await _standardService.GetAllStandards();
            return Ok(standards);
        }

        [HttpGet("standards/{standardCode}", Name = "GetStandard")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(StandardCollation))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetStandard(int standardCode)
        {
            _logger.LogInformation($@"Get Standard {standardCode} from Standard Service");
            var standard = await _standardService.GetStandard(standardCode);
            return Ok(standard);
        }

        [HttpGet("standard-options", Name = "GetStandardOptions")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<StandardOptions>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetStandardOptions()
        {
            _logger.LogInformation("Get all standard options from Standard Service");
            var standardOptions = await _standardService.GetStandardOptions();
            return Ok(standardOptions);
        }

        [HttpGet("standard-options/{id}", Name = "GetStandardOptionsByStandardId")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(StandardOptions))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetStandardOptionsForStandard(string id)
        {
            _logger.LogInformation($"Get standard options from Standard Service for standard with id {id}");
            var standardOptions = await _standardService.GetStandardOptionsByStandardId(id);
            return Ok(standardOptions);
        }

        [HttpGet("standard-options/{standardReference}/{version}", Name = "GetStandardOptionsByStandardReferenceAndVersion")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(StandardOptions))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetStandardOptionsForStandardReferenceAndVersion(string standardReference, string version)
        {
            _logger.LogInformation($"Get standard options from Standard Service for standard with standard reference {standardReference} and verion {version}");
            var standardOptions = await _standardService.GetStandardOptionsByStandardReferenceAndVersion(standardReference, version);
            return Ok(standardOptions);
        }
    }
}
