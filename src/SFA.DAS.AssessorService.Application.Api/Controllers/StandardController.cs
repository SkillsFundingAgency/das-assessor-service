using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Standards;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.ExternalApis.IFAStandards.Types;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize(Roles = "AssessorServiceInternalAPI")]
    [Route("api/ao/")]
    [ValidateBadRequest]
    public class StandardController : Controller
    {
        private readonly ILogger<StandardController> _logger;
        private readonly IMediator _mediator;
      
        public StandardController(ILogger<StandardController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }
        [HttpPost("update-standards",Name = "update-standards/GatherAndStoreStandards")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(GatherStandardsResponse))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.Conflict, Type = typeof(ApiResponse))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GatherAndStoreStandards([FromBody] GatherStandardsRequest request)
        {
            var processDetails = await _mediator.Send(request);
            _logger.LogInformation(processDetails.ToString());
            return Ok(new GatherStandardsResponse(processDetails));
        }

        [HttpGet("assessment-organisations/collated-standards", Name = "GetCollatedStandards")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(List<StandardCollation>))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetCollatedStandards()
        {

            await _mediator.Send(new GatherStandardsRequest());
            return Ok(await _mediator.Send(new GetCollatedStandardsRequest()));
        }

        [HttpGet("assessment-organisations/collated-standards/{standardId}", Name = "GetCollatedStandard")]
        [SwaggerResponse((int)HttpStatusCode.OK, Type = typeof(StandardCollation))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetCollatedStandard(int standardId)
        {
            return Ok(await _mediator.Send(new GetCollatedStandardRequest {StandardId = standardId}));
        }
    }
}