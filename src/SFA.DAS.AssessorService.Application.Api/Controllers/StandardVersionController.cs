using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/v1/standard-version/")]
    [ValidateBadRequest]
    public class StandardVersionController : Controller
    {
        private readonly ILogger<StandardServiceController> _logger;
        private readonly IStandardService _standardService;

        public StandardVersionController(ILogger<StandardServiceController> logger, IStandardService standardService)
        {
            _logger = logger;
            _standardService = standardService;
        }

        [HttpGet("standards", Name = "GetAllStandardVersions")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<StandardVersion>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetAllStandardVersions()
        {
            _logger.LogInformation($@"Get all standards from Standard Service");
            var standards = await _standardService.GetAllStandardVersions();
            return Ok(standards.Select(s => (StandardVersion)s));
        }

        [HttpGet("standards/versions/{larsCode}", Name = "GetStandardVersionsByLarsCode")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<StandardVersion>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetStandardVersionsByLarsCode(int larsCode)
        {
            _logger.LogInformation($@"Get Standard Versions for LarsCode {larsCode} from Standard Service");
            var standards = await _standardService.GetStandardVersionsByLarsCode(larsCode);
            return Ok(standards.Select(s => (StandardVersion)s));
        }

        [HttpGet("standards/{standardUId}", Name = "GetStandardVersionByStandardUId")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(StandardVersion))]
        [SwaggerResponse((int)HttpStatusCode.NotFound)]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Type = typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetStandardVersionByStandardUId(string standardUId)
        {
            _logger.LogInformation($@"Get Standard Version for StandardUId {standardUId} from Standard Service");
            var standard = await _standardService.GetStandardVersionByStandardUId(standardUId);
            if(standard == null)
            {
                return NotFound();
            }

            return Ok((StandardVersion)standard);
        }

        [HttpGet("standards/epao/{epaoId}/{larsCode?}", Name = "GetEpaoRegisteredStandardVersions")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(int))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetEpaoRegisteredStandardVersions(string epaoId, int? larsCode = null)
        {
            _logger.LogInformation($"Received request to retrieve StandardVersions for Organisation {epaoId}");
            var standardVersions = await _standardService.GetEPAORegisteredStandardVersions(epaoId, larsCode);
            return Ok(standardVersions);
        }

    }
}