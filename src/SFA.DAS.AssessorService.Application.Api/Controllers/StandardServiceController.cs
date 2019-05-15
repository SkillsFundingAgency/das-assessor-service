﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Interfaces;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

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

        [HttpGet("standards/summaries", Name = "StandardServiceGetAllStandardSummaries")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(IEnumerable<StandardSummary>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetAllStandards()
        {
            _logger.LogInformation($@"Get all standards from Standard Service");
            var standards = await _standardService.GetAllStandards();
            return Ok(standards);
        }

        [HttpGet("standards/{standardCode}", Name = "StandardServiceGetStandard")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(Standard))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetStandard(int standardCode)
        {
            _logger.LogInformation($@"Get Standard {standardCode} from Standard Service");
            var standard = await _standardService.GetStandard(standardCode);
            return Ok(standard);
        }
    }
}
