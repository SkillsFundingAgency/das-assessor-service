using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models;
using SFA.DAS.AssessorService.Application.Api.Middleware;
using SFA.DAS.AssessorService.Application.Api.Properties.Attributes;
using SFA.DAS.AssessorService.Application.Exceptions;
using SFA.DAS.AssessorService.Domain.Entities.ao;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.AssessorService.Application.Api.Controllers
{
    [Authorize]
    [Route("api/ao")]
    [ValidateBadRequest]
    public class RegisterQueryController : Controller
    {
        private readonly ILogger<RegisterQueryController> _logger;
        private readonly IMediator _mediator;

        public RegisterQueryController(IMediator mediator, ILogger<RegisterQueryController> logger
        )
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("organisation-types", Name = "GetOrganisationTypes")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(List<EpaOrganisationType>))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> GetOrganisationTypes()
        {
            _logger.LogInformation("Get Organisation Types");
            return Ok(await _mediator.Send(new GetOrganisationTypesRequest()));
        }
    }
}
